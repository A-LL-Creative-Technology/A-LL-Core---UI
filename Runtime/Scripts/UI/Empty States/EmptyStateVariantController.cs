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

    [SerializeField] private BackType backTo;

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
