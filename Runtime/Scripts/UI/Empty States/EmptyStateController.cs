using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyStateController : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GameObject pageToRefresh;
#pragma warning restore 0649


    // Start is called before the first frame update
    public void OnCallToAction()
    {
        //Refresh Page
        pageToRefresh.SetActive(false);
        pageToRefresh.SetActive(true);
    }
}
