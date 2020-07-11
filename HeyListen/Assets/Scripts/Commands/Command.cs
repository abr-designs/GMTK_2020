using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct Command
{
    [SerializeField, DisableInPlayMode]
    private Button button;
    [SerializeField, DisableInPlayMode]
    private CMD cmd;

    [SerializeField]
    private string CommandText;

    public void Init(Action<CMD> onCommandCallback)
    {
        var command = cmd;
        var text = CommandText;
        button.onClick.AddListener(() =>
        {
            onCommandCallback?.Invoke(command);
            
            CommandLine.SubmitCommand("Admin", "cmd",text);
        });
    }
}
