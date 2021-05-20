using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class ManageViewsPostProcessBuild
{
    [PostProcessBuildAttribute(0)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        //PlayModeStateChanged.ResetViewsAfterPlay();
    }

}
