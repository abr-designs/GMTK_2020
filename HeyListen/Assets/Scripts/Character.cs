using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour
{
    [SerializeField, Range(0f,1f)]
    private float logic = 1f;

   [SerializeField, Range(0.05f, 1f)]
   private float chooseFrequency = 0.1f;
   private float chooseTimer = 0f;
   
    [ShowInInspector, ReadOnly]
    private Dictionary<CMD, float> decisionWeights = new Dictionary<CMD, float>
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

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        transform = gameObject.transform;
        _targetPosition = transform.position;
        
        waitingForCommand = true;
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

    #region Movement

    private void MovementUpdate()
    {
        var dist = Vector2.Distance(rigidbody.position, _targetPosition);

        if (dist <= 0.01f)
        {
            waitingForCommand = true;
            moving = false;
            rigidbody.position = _targetPosition;
            
            PerformCommand(WeighOptions());
            return;
        }
        
        var newPosition = Vector2.MoveTowards(rigidbody.position, _targetPosition, speed * Time.fixedDeltaTime);
        rigidbody.MovePosition(newPosition);
        
        
    }
    
    #endregion //Movement
    
    //================================================================================================================//
    
    #region Reasoning

    private void AdjustLogicLevel(float amount)
    {
        logic = Mathf.Clamp01(logic + amount);
    }

    private void DecisionUpdate()
    {
        if (chooseTimer < chooseFrequency)
        {
            chooseTimer += Time.deltaTime;
            return;
        }
    
        chooseTimer = 0f;
        PerformCommand(WeighOptions());
    }

    public void ReceiveCommand(CMD command)
    {
        var role = Random.value;

        if (role > logic)
        {
            //var rand = Random.Range(0, 5);
            Debug.Log("Ignored");
            
            //decisionWeights[(CMD)rand] += 1f - logic;
            
            return;
        }

        Debug.Log(command);
        decisionWeights[command] += 0.2f;
        
        if(waitingForCommand) PerformCommand(WeighOptions());
    }

    private CMD WeighOptions()
    {
        var cmd = CMD.STOP;
        var max = -999f;
        
        foreach (var decisionWeight in decisionWeights)
        {
            
            var value = decisionWeight.Value;
            
            if (value <= max || value == 0f)
                continue;

            max = decisionWeight.Value;
            cmd = decisionWeight.Key;
        }

        return cmd;
    }
    
    private void PerformCommand(CMD command)
    {
        _currentCommand = command;
        
        Debug.Log($"Received Command: {command}");
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
                break;
            case CMD.STOP:
                //TODO I will want to stop at the nearest grid position
                _targetPosition = rigidbody.position;
                waitingForCommand = true;
                ResetDecisions();
                break;
            case CMD.GO:
                PerformCommand(WeighOptions());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(command), command, null);
        }
    }

    private void ResetDecisions()
    {
        var keys = decisionWeights.Keys.ToList();
        
        foreach (var key in keys)
        {
            //var val = decisionWeights[key];
            decisionWeights[key] = 0f; //Mathf.Clamp01(val - 0.2f);
        }
    }
    
    private void ReduceCommandWeight(CMD command)
    {
        decisionWeights[command] = Mathf.Clamp01(decisionWeights[command] - 0.2f);
    }
    
    #endregion //Reasoning
    
    //================================================================================================================//
}
