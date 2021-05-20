using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System;

public class ResetViewsAfterBuildEditor : EditorWindow
{
    [MenuItem("A-LL/Scene Management.../Reset Views After Build")]
    public static void ResetViewsAfterBuild()
    {
        GlobalEditor.ClearLog();

        PlayModeStateChanged.ResetViewsAfterPlay();
    }
}

