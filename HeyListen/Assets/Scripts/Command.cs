using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Command
{
    [SerializeField, DisableInPlayMode]
    private Button button;
    [SerializeField, DisableInPlayMode]
    private CMD cmd;

    public void Init(Action<CMD> onCommandCallback)
    {
        var command = cmd;
        button.onClick.AddListener(() =>
        {
            
            onCommandCallback?.Invoke(command);
        });
    }
}
