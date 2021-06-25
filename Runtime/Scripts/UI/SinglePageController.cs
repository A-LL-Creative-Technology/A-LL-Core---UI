using System;
using System.Collections;
using System.Collections.Generic;
using Proyecto26;
using UnityEngine;

public abstract class SinglePageController : MonoBehaviour
{
#pragma warning disable 0649

    public bool hasTransparentHeader;

    protected RectTransform rectTransform;

#pragma warning restore 0649


    protected void OnContentLoadedDone(Action callback = null)
    {
        StartCoroutine(GlobalController.RebuildLayout(rectTransform, () =>
        {
            // fires the event to notify who is interested in content loading
            if (NavigationController.singlePageContentLoadedDelegate != null)
                NavigationController.singlePageContentLoadedDelegate(this, EventArgs.Empty);

            callback?.Invoke();
        }));
    }
}
