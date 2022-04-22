using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppNavigationController : MonoBehaviour
{
    private void Start()
    {
        ALLCoreConfig.OnALLCoreReady += OnALLCoreReadyCallback;
    }

    private void OnDestroy()
    {
        ALLCoreConfig.OnALLCoreReady -= OnALLCoreReadyCallback;
    }

    private void OnALLCoreReadyCallback(object sender, EventArgs e)
    {
        NavigationController.GetInstance().currentNavigationMode = NavigationController.NavigationMode.DisplayOpen;
        StartCoroutine(NavigationController.GetInstance().QueueToNavigate(() => NavigationController.GetInstance().Navigate(NavigationController.GetInstance().currentView.name)));
    }
}