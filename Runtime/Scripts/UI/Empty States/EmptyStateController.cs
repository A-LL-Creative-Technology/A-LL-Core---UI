using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyStateController : MonoBehaviour
{
    [SerializeField] private GameObject pageToRefresh;
    // Start is called before the first frame update
    public void OnCallToAction()
    {
        //Refresh Page
        pageToRefresh.SetActive(false);
        pageToRefresh.SetActive(true);
    }
}
