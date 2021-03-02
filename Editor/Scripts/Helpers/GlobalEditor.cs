using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GlobalEditor : MonoBehaviour
{

    static public void ClearLog() //you can copy/paste this code to the bottom of your script
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }

}
