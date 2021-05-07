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

#pragma warning restore 0649

    private void Awake()
    {
        instance = this;
    }
}
