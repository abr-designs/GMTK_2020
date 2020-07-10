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
}
