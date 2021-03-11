using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Localization;
using UnityEditor.Localization.Editor;

public class FindMissingAnimationsEditor : EditorWindow
{
    [MenuItem("A-LL/Find Missing.../Find Missing Animations")]
    public static void FindMissingAnimations()
    {
        GlobalEditor.ClearLog();

        //Find all gameobjects in the scene including hiding ones
        GameObject[] allGameObjectsInScene = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach(GameObject currentGameObject in allGameObjectsInScene)
        {
            if (!currentGameObject.scene.IsValid()) // we make sure it is in the scene and not prefab
                continue;

            // check the AnimationController component (cannot be accessed directly via Editor script
            AnimationController currentAnimationController = currentGameObject.GetComponent<AnimationController>();

            if(currentAnimationController != null && currentAnimationController.animations.Count == 0) {
                Debug.Log("<color=red>This gameobject has a AnimationController component but no animations set. Is it missing its animation? " + currentGameObject.name + "</color>", currentGameObject);
            }
        }
    }

}
