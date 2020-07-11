using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour, IHealth
{
    [SerializeField, Range(0f,1f)]
    private float logic = 1f;

   [SerializeField, Range(0.05f, 3f)]
   private float chooseFrequencyMax = 0.1f;
   [SerializeField, Range(0.05f, 3f), ReadOnly]
   private float chooseFrequency;
   private float chooseTimer;
   
   private CMD[] _commands = {
       CMD.LEFT,
       CMD.RIGHT,
       CMD.UP,
       CMD.DOWN,
       CMD.STOP,
       CMD.GO
   };
   
    [ShowInInspector, ReadOnly]
    private Dictionary<CMD, float> decisionWeights = new Dictionary<CMD, float>
    {
        { CMD.LEFT, 0.5f },
        { CMD.RIGHT, 0.5f },
        { CMD.UP, 0.5f },
        { CMD.DOWN, 0.5f },
        { CMD.STOP, 0.5f },
        { CMD.GO, 0.5f },
    };
    
    [ShowInInspector, ReadOnly]
    private Dictionary<CMD, float> suggestions = new Dictionary<CMD, float>
    {
        { CMD.LEFT, 0f },
        { CMD.RIGHT, 0f },
        { CMD.UP, 0f },
        { CMD.DOWN, 0f },
        { CMD.STOP, 0f },
        { CMD.GO, 0f },
    };

    [SerializeField]
    private float speed;
    
    private new Rigidbody2D rigidbody;
    private new Transform transform;
    private Vector2 _targetPosition;
    
    private CMD _currentCommand = CMD.STOP;

    private bool moving;
    private bool waitingForCommand = false;
    
    //================================================================================================================//

        
    public float startingHealth { get; set; }
    public float currentHealth { get; set; }
    
    //================================================================================================================//

    private void Start()
    {
        chooseFrequency = chooseFrequencyMax;
        
        rigidbody = GetComponent<Rigidbody2D>();
        transform = gameObject.transform;
        _targetPosition = transform.position;
        
        waitingForCommand = true;
        
        SetupHealth(100f);

        StartCoroutine(DegradeDecisionWeightsCoroutine());
        StartCoroutine(TakeAKneeCoroutine());
        
        CommandLine.SubmitCommand("OS","sys", "Booting OS...");
    }

    private void Update()
    {
        if(waitingForCommand)
            DecisionUpdate();
    }

    private void FixedUpdate()
    {
        if(moving) 
            MovementUpdate();
    }

    
    //================================================================================================================//

    
    public void SetupHealth(float health)
    {
        currentHealth = startingHealth = health;
    }

    public void ChangeHealth(float amount)
    {
        currentHealth += amount;
        
        //This should change the logic based on the amount of health lost
        if (amount < 0f)
        {
            var offset = amount / startingHealth;
            AdjustLogicLevel(offset);
            ReduceOpinionWeight(_currentCommand, offset);
            
            ForceCommand(CMD.STOP);
            CommandLine.SubmitCommand("OS","msg", "OW!");
        }
        else if (amount > 0f)
        {
            var offset = amount / startingHealth;
            AdjustLogicLevel(offset);

            AddOpinionWeight(_currentCommand, offset);
            CommandLine.SubmitCommand("OS","msg", "msg I feel way better!");
        }
    }
    
    //================================================================================================================//

    #region Movement
    
    public void HitWall()
    {
        foreach (var cmd in _commands)
        {
            if(cmd ==_currentCommand)
                continue;
            
            AddOpinionWeight(cmd, 0.25f);
        }
        
        ReduceOpinionWeight(_currentCommand, 1f);
        ReduceSuggestionWeight(_currentCommand, 10f);

        ChangeHealth(-5f);

        ForceCommand(CMD.STOP);
    }

    private void MovementUpdate()
    {
        var dist = Vector2.Distance(rigidbody.position, _targetPosition);

        if (dist <= 0.01f)
        {
            waitingForCommand = true;
            moving = false;
            rigidbody.position = _targetPosition;
            
            AddOpinionWeight(_currentCommand, 0.1f);
            //PerformCommand(WeighOptions());
            return;
        }
        
        var newPosition = Vector2.MoveTowards(rigidbody.position, _targetPosition, speed * Time.fixedDeltaTime);
        rigidbody.MovePosition(newPosition);
        
        
    }
    
    #endregion //Movement
    
    //================================================================================================================//
    
    #region Reasoning

    private void ForceCommand(CMD command)
    {
        PerformCommand(command, false);
    }

    

    public void AdjustLogicLevel(float amount)
    {
        logic = Mathf.Clamp01(logic + amount);
        AdjustDecisionFrequency(amount);
    }
    
    private void AdjustDecisionFrequency(float amount)
    {
        chooseFrequency = Mathf.Clamp(chooseFrequency + (chooseFrequency * amount), 0.1f, chooseFrequencyMax);
    }

    private void DecisionUpdate()
    {
        if (chooseTimer < chooseFrequency)
        {
            chooseTimer += Time.deltaTime;
            return;
        }
    
        chooseTimer = 0f;

        CommandLine.SubmitCommand("OS","msg", "I can't wait, I should keep moving");
        PerformCommand(WeighOptions());
    }

    public void ReceiveCommandSuggestion(CMD command)
    {
        if (IgnoreSuggestions())
        {
            Debug.Log("Ignored");
            CommandLine.SubmitCommand("OS","err", "CMD IGNORED");
            return;
        }

        foreach (var cmd in _commands)
        {
            if(cmd == command)
                AddSuggestionWeight(cmd, 0.5f);
            else
                ReduceSuggestionWeight(cmd, 1f);
        }

        
        //AddCommandWeight(command, 0.2f);
        if(waitingForCommand) PerformCommand(WeighOptions());
    }

    private CMD WeighOptions()
    {
        
        var bestSuggestion = suggestions.GetBestOption(logic);
        var bestOpinion = decisionWeights.GetBestOption(logic);

        if (bestSuggestion.value > bestOpinion.value)
            return bestSuggestion.command;
        
        
        return bestOpinion.command;


        //var cmd = CMD.STOP;
        //var max = -999f;
        //
        //foreach (var decisionWeight in decisionWeights)
        //{
        //    
        //    var value = decisionWeight.Value;
        //    
        //    if (value <= max || value == 0f)
        //        continue;
//
        //    max = decisionWeight.Value;
        //    cmd = decisionWeight.Key;
        //}
//
        //return cmd;
    }

    private bool IgnoreSuggestions()
    {
        var role = Random.value;

        return role > logic;
    }
    
    private void PerformCommand(CMD command, bool displayMsg = true)
    {
        _currentCommand = command;
        
        //Debug.Log($"Performing Command: {command}");
        
        waitingForCommand = false;
        chooseTimer = 0f;
        
        switch (command)
        {
            case CMD.LEFT:
            case CMD.RIGHT:
            case CMD.UP:
            case CMD.DOWN:
                _targetPosition = rigidbody.position + command.ToVector2();
                moving = true;
                CommandLine.SubmitCommand("OS","msg", $"I think I'll go {command}");
                break;
            case CMD.STOP:
                //TODO I will want to stop at the nearest grid position
                _targetPosition = rigidbody.position;
                waitingForCommand = true;
                if(displayMsg)
                    CommandLine.SubmitCommand("OS","msg", $"I should {command}");
                //suggestions.ResetDecisions();
                break;
            case CMD.GO:
                PerformCommand(WeighOptions());
                if(displayMsg)
                    CommandLine.SubmitCommand("OS","msg", $"I should {command}");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(command), command, null);
        }
    }
    
    //Adjust Decision Weights
    //================================================================================================================//

    private void AddOpinionWeight(CMD command, float amount)
    {
        decisionWeights[command] = Mathf.Clamp01(decisionWeights[command] + Mathf.Abs(amount));
    }
    
    private void ReduceOpinionWeight(CMD command, float amount)
    {
        decisionWeights[command] = Mathf.Clamp01(decisionWeights[command] - Mathf.Abs(amount));
    }
    
    private void AddSuggestionWeight(CMD command, float amount)
    {
        suggestions[command] = Mathf.Clamp(suggestions[command] + Mathf.Abs(amount), 0f, 10f);
    }
    
    private void ReduceSuggestionWeight(CMD command, float amount)
    {
        suggestions[command] = Mathf.Clamp(suggestions[command] - Mathf.Abs(amount), 0f, 10f);
    }
    
    //================================================================================================================//

    private IEnumerator DegradeDecisionWeightsCoroutine()
    {
        while (true)
        {
            foreach (var cmd in _commands)
            {
                //ReduceOpinionWeight(cmd, 0.05f);
                
                ReduceSuggestionWeight(cmd, 0.1f);
            }

            yield return new WaitForSeconds(chooseFrequency);
        }
    }
    
    private IEnumerator TakeAKneeCoroutine()
    {
        while (true)
        {
            if (_currentCommand == CMD.STOP)
            {
                AdjustLogicLevel(0.1f);
                //AdjustDecisionFrequency(0.05f);
                
                ReduceOpinionWeight(CMD.STOP, 0.1f);
            }
            yield return new WaitForSeconds(chooseFrequency);
        }
    }
    
    #endregion //Reasoning
    
    //================================================================================================================//

}
