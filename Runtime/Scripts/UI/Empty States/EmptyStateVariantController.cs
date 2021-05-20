using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyStateVariantController : MonoBehaviour
{
    [HideInInspector]
    public enum BackType
    {
        Page,
        Scene,
    }

#pragma warning disable 0649
    [SerializeField] private BackType backTo;
#pragma warning restore 0649

    public void OnCallToAction()
    {
        switch (backTo)
        {
            case BackType.Page :
                NavigationController.GetInstance().OnStackClose();
                break;
            case BackType.Scene:
                NavigationController.GetInstance().OnLoadNextSceneAsync("Main");
                break;
        }
    }
}
