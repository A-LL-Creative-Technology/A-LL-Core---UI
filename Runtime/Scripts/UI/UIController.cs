using System;
using MoreMountains.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private static UIController instance;

    public static UIController GetInstance()
    {
        return instance;
    }

#pragma warning disable 0649
    private readonly float LOADER_UPDATE_THRESHOLD = -300f;
    private readonly float LOADER_UPDATE_THRESHOLD_TO_FADE = -50f;
    private readonly float LOADER_THRESHOLD_TO_INFINITE_SCROLL = 1000f;

    public Texture2D imagePlaceholder;
    
    [SerializeField] private GameObject overCanvas;

    [SerializeField] private CanvasGroup updateLoaderCanvasGroup;
    [SerializeField] private Animator updateLoaderAnimator;

    [NonSerialized] public GameObject infiniteScrollLoaderContainer;

    private bool isLoadingUpdatesScroll = false; // to prevent from multiple loading
    private bool isInLoadingUpdatesScrollZone = false; // to prevent from multiple loading
    private bool isLoadingInfiniteScroll = false;
    private bool isInLoadingInfiniteScrollZone = false;

    private RectTransform scrollViewContainerRectTransform;

    [NonSerialized] public RectTransform overCanvasRectTransform;

#pragma warning restore 0649

    private void Awake()
    {
        instance = this;

        updateLoaderCanvasGroup.gameObject.SetActive(false);

        overCanvasRectTransform = overCanvas.GetComponent<RectTransform>();
       
        // make sure loader is hidden
        updateLoaderCanvasGroup.alpha = 0;

    }

    private void Start()
    {
        scrollViewContainerRectTransform = NavigationController.GetInstance().scrollViewContainer.GetComponent<RectTransform>();

    }

    public void StartLoader(Action callbackUpdate, Action callbackInfiniteScroll)
    {

        float scrollLocalYPosition = scrollViewContainerRectTransform.localPosition.y;
        float scrollViewContainerHeight = scrollViewContainerRectTransform.sizeDelta.y;

        // it enters the updating zone
        if (!isInLoadingUpdatesScrollZone)
        {
            // safe zone: we make sure everything is back to normal when we quit the update area (due to float imprecise values)
            if (scrollLocalYPosition > 0.9f * LOADER_UPDATE_THRESHOLD_TO_FADE && scrollLocalYPosition < 0)
            {
                //Debug.Log("Safe Zone");
                updateLoaderCanvasGroup.gameObject.SetActive(false);
            }else if (scrollLocalYPosition < 0.9f * LOADER_UPDATE_THRESHOLD_TO_FADE && scrollLocalYPosition >= LOADER_UPDATE_THRESHOLD) // entering updates
            {
                //Debug.Log("Entering Updates");
                AroundUpdateScroll(scrollLocalYPosition, true);
            }
            else if (scrollLocalYPosition < LOADER_UPDATE_THRESHOLD && !isLoadingUpdatesScroll) // starting updates
            {
                //Debug.Log("Starting Updates");
                StartUpdateScroll(callbackUpdate);

            }
            else if (scrollViewContainerHeight - scrollLocalYPosition < LOADER_THRESHOLD_TO_INFINITE_SCROLL && !isLoadingInfiniteScroll && !isInLoadingInfiniteScrollZone) // starting infinite
            {
                //Debug.Log("Entering Infinite");
                StartInfiniteScroll(callbackInfiniteScroll);
            }
        }
        else if (isInLoadingUpdatesScrollZone && (scrollLocalYPosition < 0.9f * LOADER_UPDATE_THRESHOLD_TO_FADE && scrollLocalYPosition >= LOADER_UPDATE_THRESHOLD)) // leaving updates
        {
            //Debug.Log("Leaving Updates");
            AroundUpdateScroll(scrollLocalYPosition, false);
        }

        if ((isInLoadingUpdatesScrollZone && !isLoadingUpdatesScroll && scrollLocalYPosition >= 0.9f * LOADER_UPDATE_THRESHOLD_TO_FADE) || (isInLoadingInfiniteScrollZone && !isLoadingInfiniteScroll && scrollViewContainerHeight - scrollLocalYPosition >= LOADER_THRESHOLD_TO_INFINITE_SCROLL)) // exiting scroll triggers
        {
            //Debug.Log("Exiting both");
            ExitingBothScroll();
        }
    }

    private void AroundUpdateScroll(float scrollLocalYPosition, bool isEntering)
    {

        float ratioToLoading = (scrollLocalYPosition - LOADER_UPDATE_THRESHOLD_TO_FADE) / (LOADER_UPDATE_THRESHOLD - LOADER_UPDATE_THRESHOLD_TO_FADE);
        updateLoaderCanvasGroup.alpha = ratioToLoading;

        if (isEntering)
        {
            updateLoaderCanvasGroup.gameObject.SetActive(true);

            updateLoaderAnimator.speed = 0;
            updateLoaderAnimator.Play("Playing", -1, ratioToLoading);
        }
    }

    private void StartUpdateScroll(Action callbackUpdate)
    {
        updateLoaderAnimator.speed = 1.5f;

        isLoadingUpdatesScroll = true;
        isInLoadingUpdatesScrollZone = true;

        MMVibrationManager.Haptic(HapticTypes.LightImpact);

        callbackUpdate?.Invoke();
    }



    public void StoppingUpdateScroll()
    {
        //Debug.Log("Stopping Updates");
        updateLoaderCanvasGroup.gameObject.SetActive(false);

        isLoadingUpdatesScroll = false;
    }

    private void ExitingBothScroll()
    {
        
        isInLoadingUpdatesScrollZone = false;
        isInLoadingInfiniteScrollZone = false;
    }


    private void StartInfiniteScroll(Action callbackInfiniteScroll)
    {
        infiniteScrollLoaderContainer.SetActive(true);

        isLoadingInfiniteScroll = true;
        isInLoadingInfiniteScrollZone = true;

        callbackInfiniteScroll?.Invoke();
    }

    public void StoppingInfiniteScroll()
    {
        //Debug.Log("Stopping Infinite");

        infiniteScrollLoaderContainer.SetActive(false);

        isLoadingInfiniteScroll = false;
    }


    public void OnOpenLink(string url)
    {
        GlobalController.LogMe(url);

        //Log open_link in Analytics
        AnalyticsController.GetInstance()?.Log(AnalyticsController.LOGS.OPEN_LINK, url);

        Application.OpenURL(url);
    }

    public void OnOpenBecomeMemberLink()
    {
        string url = "https://www.ccif.ch/la-ccif/demande-d-adhesion-fr.html";

        if (CacheController.GetInstance().appConfig.lang == "de-DE")
            url = "https://www.hikf.ch/die-hikf/beitrittsgesuch.html";

        OnOpenLink(url);
    }

    public void OnReloadEmptyPage()
    {
        // we reload the cache to trigger the initial setup of the app
        CacheController.ClearAllCache();

    }

}
