using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class FindMissingLocalizationsRuntime : MonoBehaviour
{

    private void Awake()
    {
        //Find all gameobjects in the scene including hiding ones
        GameObject[] allGameObjectsInScene = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject currentGameObject in allGameObjectsInScene)
        {
            if (!currentGameObject.scene.IsValid()) // we make sure it is in the scene and not prefab
                continue;

            if (currentGameObject.GetComponent<LocalizeStringEvent>() && currentGameObject.GetComponent<LocalizeStringEvent>().StringReference.IsEmpty)
            {
                Debug.Log("<color=red>This gameobject is missing a Localization: " + currentGameObject.name + "</color>", currentGameObject);
            }

        }

        Debug.Log("<color=orange>Finish running FindMissingLocalizationsRuntime. The application will quit when compiled. Please remove this script from the scene.</color>");
        Application.Quit();
    }
}
