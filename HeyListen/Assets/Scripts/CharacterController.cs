using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    private List<Command> commands;

    [SerializeField]
    private Character _character;
    
    //================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        foreach (var command in commands)
        {
            command.Init(_character.ReceiveCommand);
        }
    }

    //================================================================================================================//

    
    //================================================================================================================//

}
