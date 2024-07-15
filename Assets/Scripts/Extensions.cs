using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    
    public static IAction ToIAction(this ResourceType type)
    {
        TextAsset script = type.Load() as TextAsset;
        string name = script.name.Split('.')[0];
        Type actionType = Type.GetType(name);
        return Activator.CreateInstance(actionType) as IAction;
    }

}
