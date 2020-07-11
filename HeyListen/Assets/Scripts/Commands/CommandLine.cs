using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class CommandLine : MonoBehaviour
{
    
    #region Instance
    
    //public static CommandLine Instance => _instance;
    private static CommandLine _instance;

    private void Awake()
    {
        _instance = this;
    }
    
    #endregion //Instance

    [SerializeField, Required]
    private TMP_Text textArea;

    private List<string> commands;

    [SerializeField, ReadOnly]
    private int messages;

    private void Start()
    {
        commands = new List<string>();
        DisplayCommands();
    }

    public static void SubmitCommand(string from, string type, string message)
    {
        _instance.commands.Add($"[{from}]{type}> {message}");
        _instance.messages = _instance.commands.Count;
        _instance.DisplayCommands();
    }
    public static void SubmitCommand(string from, string type, string message, string color)
    {
        _instance.commands.Add($"<color={color}>[{from}]{type}> {message}</color>");
        _instance.messages = _instance.commands.Count;
        _instance.DisplayCommands();
    }


    private void DisplayCommands()
    {
        if (commands.Count == 0)
            textArea.text = string.Empty;
        
        if(commands.Count >= 15)
            commands.RemoveAt(0);
        
        var display = commands.Aggregate(string.Empty, (current, command) => current + $"{command}\n\n");

        textArea.text = display;
    }
}
