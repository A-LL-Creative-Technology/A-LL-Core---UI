using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ALLCoreConfig : MonoBehaviour
{
    //private static APIController instance;
    public static ALLCoreConfig instance;

    public static ALLCoreConfig GetInstance()
    {
        return instance;
    }

#pragma warning disable 0649

    public bool activateSafeArea;

    public static event EventHandler OnALLCoreReady;
    [NonSerialized] public bool isALLCoreReady = false;

#pragma warning restore 0649

    private void Awake()
    {
        instance = this;
    }

    public void ALLCoreReady()
    {
        isALLCoreReady = true;

        if (OnALLCoreReady != null)
            OnALLCoreReady(this, EventArgs.Empty);
    }
}
