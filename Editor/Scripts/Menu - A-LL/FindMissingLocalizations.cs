using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Localization;
using UnityEditor.Localization.Editor;

public class FindMissingLocalizationsEditor : EditorWindow
{
    [MenuItem("A-LL/Find Missing.../Find Missing Localizations")]
    public static void FindMissingLocalizations()
    {
        GlobalEditor.ClearLog();

        //Find all gameobjects in the scene including hiding ones
        GameObject[] allGameObjectsInScene = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach(GameObject currentGameObject in allGameObjectsInScene)
        {
            if (!currentGameObject.scene.IsValid()) // we make sure it is in the scene and not prefab
                continue;

            // check the localize component (cannot be accessed directly via Editor script
            Component[] components = currentGameObject.GetComponents<Component>();

            bool hasTMPComponent = false;
            bool hasLocalizeComponent = false;
            foreach (Component currentComponent in components)
            {
                if(currentComponent.GetType().FullName == "TMPro.TextMeshProUGUI")
                {
                    hasTMPComponent = true;
                } 

                if (currentComponent.GetType().FullName == "UnityEngine.Localization.Components.LocalizeStringEvent")
                {
                    hasLocalizeComponent = true;
                }
            }

            if(hasTMPComponent && !hasLocalizeComponent)
            {
                Debug.Log("<color=red>This gameobject has a TextMeshPro component bur no Localize component. Is it missing Localization? " + currentGameObject.name + "</color>", currentGameObject);
            }

            //if(hasTMPComponent && hasLocalizeComponent)
            //{
            //    Debug.Log("<color=green>This gameobject has a Localize component to be checked: " + currentGameObject.name + "</color>", currentGameObject);
            //}
        }
    }

}
