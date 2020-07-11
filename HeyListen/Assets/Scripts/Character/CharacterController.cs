using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    private List<Command> commands;

    [SerializeField]
    private Character _character;


    [SerializeField, Range(0.01f, 2f)]
    private float timeScale = 1f;
    //================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        foreach (var command in commands)
        {
            command.Init(_character.ReceiveCommandSuggestion);
        }

        Time.timeScale = timeScale;
        
        var randValue = Random.Range(int.MinValue, int.MaxValue);
        Debug.Log($"Seed: {randValue}");
        Random.InitState(randValue);
    }

    //================================================================================================================//

    
    //================================================================================================================//

}
