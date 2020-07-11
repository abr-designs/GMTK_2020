using System;
using UnityEngine;

public static class CMDExtensions
{
    public static Vector2 ToVector2(this CMD command)
    {
        switch (command)
        {
            case CMD.LEFT:
                return Vector2.left;
            case CMD.RIGHT:
                return Vector2.right;
            case CMD.UP:
                return Vector2.up;
            case CMD.DOWN:
                return Vector2.down;
            case CMD.STOP:
            case CMD.GO:
                return Vector2.zero;
            default:
                throw new ArgumentOutOfRangeException(nameof(command), command, null);
        }
    }
    
    public static CMD Reflect(this CMD command)
    {
        switch (command)
        {
            case CMD.LEFT:
                return CMD.RIGHT;
            case CMD.RIGHT:
                return CMD.LEFT;
            case CMD.UP:
                return CMD.DOWN;
            case CMD.DOWN:
                return CMD.UP;
            case CMD.STOP:
                return CMD.GO;
            case CMD.GO:
                return CMD.STOP;
            default:
                throw new ArgumentOutOfRangeException(nameof(command), command, null);
        }
    }
}
