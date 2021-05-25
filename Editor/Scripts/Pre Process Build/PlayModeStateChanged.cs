﻿using UnityEngine;
using UnityEditor;
using System.Linq;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoadAttribute]
public static class PlayModeStateChanged
{

    static Transform viewsContent;
    static Transform overViewsContent;

    // register an event handler when the class is initialized
    static PlayModeStateChanged()
    {
        EditorApplication.playModeStateChanged += ManageViews;

    }

    private static void ManageViews(PlayModeStateChange state)
    {
        // set all views to be active (otherwise not considered by Unity equally in the Awake - Start lifecycle)
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            GlobalEditor.ClearLog();

            PrepareViewsForPlay();

            Debug.Log("EDITOR SCRIPT: Prepared for Play Mode");

        }

        // reset the views when finished playing
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            ResetViewsAfterPlay();

            Debug.Log("EDITOR SCRIPT: Reset after Play Mode");
        }

    }

    public static void PrepareViewsForPlay()
    {
        // needs to be done here to make sure objects exist when called from another class
        viewsContent = Resources.FindObjectsOfTypeAll<GameObject>()?.FirstOrDefault(g => g.CompareTag("Normal Views Container"))?.transform;
        overViewsContent = Resources.FindObjectsOfTypeAll<GameObject>()?.FirstOrDefault(g => g.CompareTag("Over Views Container"))?.transform;

        if (!viewsContent || !overViewsContent)
        {
            Debug.LogWarning("EDITOR SCRIPT: Cannot find the views GameObjects");
        }

        LoopToActivateViews(viewsContent);
        LoopToActivateViews(overViewsContent);

    }



    public static void ResetViewsAfterPlay()
    {
        // needs to be done here to make sure objects exist when called from another class
        viewsContent = Resources.FindObjectsOfTypeAll<GameObject>()?.FirstOrDefault(g => g.CompareTag("Normal Views Container"))?.transform;
        overViewsContent = Resources.FindObjectsOfTypeAll<GameObject>()?.FirstOrDefault(g => g.CompareTag("Over Views Container"))?.transform;

        if (!viewsContent || !overViewsContent)
        {
            Debug.LogWarning("EDITOR SCRIPT: Cannot find the views GameObjects");
        }


        LoopToResetViews(viewsContent);
        LoopToResetViews(overViewsContent);
    }

    private static void LoopToActivateViews(Transform parent)
    {
        foreach (Transform currentChild in parent)
        {
            GameObject currentChildGO = currentChild.gameObject;

            // store the current value to reset after play
            EditorPrefs.SetBool(parent.name + "_" + currentChildGO.name, currentChildGO.activeSelf);

            currentChildGO.SetActive(true);
        }
    }

    private static void LoopToResetViews(Transform parent)
    {
        foreach (Transform currentChild in parent)
        {
            GameObject currentChildGO = currentChild.gameObject;

            currentChildGO.SetActive(EditorPrefs.GetBool(parent.name + "_" + currentChildGO.name));

        }
    }

}