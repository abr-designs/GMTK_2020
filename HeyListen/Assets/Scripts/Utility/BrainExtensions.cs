using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BrainExtensions
{
    public static (CMD command, float value) GetBestOption(this Dictionary<CMD, float> options, float multiplier = 1f)
    {
        var cmd = (CMD)Random.Range(0,6);
        var max = -999f;
        
        foreach (var decisionWeight in options)
        {
            var value = decisionWeight.Value * multiplier;
            
            if (value <= max || value == 0f)
                continue;

            max = decisionWeight.Value;
            cmd = decisionWeight.Key;
        }

        return (cmd, max);
    }
    
    public static void ResetDecisions(this Dictionary<CMD, float> options)
    {
        var keys = options.Keys.ToList();
        
        foreach (var key in keys)
        {
            options[key] = 0f;
        }
    }
}
