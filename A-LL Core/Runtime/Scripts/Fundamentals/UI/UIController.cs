using System;
using System.Collections;
using MoreMountains.NiceVibrations;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private static UIController instance;

    public static UIController GetInstance()
    {
        return instance;
    }
#pragma warning disable 0649
    //readonly private float ANIMATION_NOTIFICATION_DURATION = 0.4f;

    private readonly float LOADER_UPDATE_THRESHOLD = -300f;
    private readonly float LOADER_UPDATE_THRESHOLD_TO_FADE = -50f;
    private readonly float LOADER_THRESHOLD_TO_INFINITE_SCROLL = 1000f;

    private ScrollRect scrollRect;

    [SerializeField] private GameObject headerContainer;
    [SerializeField] private GameObject footerContainer;
    [SerializeField] private GameObject viewport;

    [SerializeField] private OnlineMaps map;

    public Texture2D imagePlaceholder;
    
    [SerializeField] private GameObject overCanvas;
    //[SerializeField] private GameObject overScrollView;
    //public GameObject overScrollViewContainer; // container for the Over Canvas

    

    [SerializeField] private CanvasGroup updateLoaderCanvasGroup;
    [SerializeField] private Animator updateLoaderAnimator;

    [NonSerialized] public GameObject infiniteScrollLoaderContainer;

    private bool isLoadingUpdatesScroll = false; // to prevent from multiple loading
    private bool isInLoadingUpdatesScrollZone = false; // to prevent from multiple loading
    private bool isLoadingInfiniteScroll = false;
    private bool isInLoadingInfiniteScrollZone = false;

    private RectTransform scrollViewContainerRectTransform;

    [NonSerialized] public RectTransform overCanvasRectTransform;
    //[NonSerialized] public ScrollRectFaster overScrollViewScrollRect;

    private OnlineMapsMarker marker;

    private RectTransform mapRectTransform;
    private OnlineMapsUIRawImageControl mapControl;

    private Rect safeArea;

#pragma warning restore 0649

    private void Awake()
    {
        instance = this;

        //Set the status bar visible for Android
        //ApplicationChrome.statusBarState = ApplicationChrome.States.Visible;

        updateLoaderCanvasGroup.gameObject.SetActive(false);

        mapRectTransform = map.transform.GetComponent<RectTransform>();
        mapControl = map.GetComponent<OnlineMapsUIRawImageControl>();

        // get scroll view rect
        //overScrollViewScrollRect = overScrollView.GetComponent<ScrollRectFaster>();
        overCanvasRectTransform = overCanvas.GetComponent<RectTransform>();

       
        // make sure loader is hidden
        updateLoaderCanvasGroup.alpha = 0;

    }

    private void Start()
    {
        // adjust wrt Safe Area
        AdjustSafeArea();

        scrollRect = NavigationController.GetInstance().scrollView.GetComponent<ScrollRect>();
        marker = OnlineMapsMarkerManager.CreateItem(0, 0);

        scrollViewContainerRectTransform = NavigationController.GetInstance().scrollViewContainer.GetComponent<RectTransform>();

    }

    private void AdjustSafeArea()
    {
        safeArea = Screen.safeArea;

        // adjust header
        RectTransform headerContainerRectTransform = headerContainer.GetComponent<RectTransform>();
        AdjustSafeAreaHeader(headerContainerRectTransform);

        // adjust footer
        RectTransform footerContainerRectTransform = footerContainer.GetComponent<RectTransform>();
        footerContainerRectTransform.anchorMin = new Vector2(footerContainerRectTransform.anchorMin.x, safeArea.position.y / Screen.height);
        footerContainerRectTransform.anchorMax = new Vector2(footerContainerRectTransform.anchorMax.x, (footerContainerRectTransform.anchorMax.y + (safeArea.position.y / Screen.height)));

        footerContainerRectTransform.offsetMin = Vector2.zero;
        footerContainerRectTransform.offsetMax = Vector2.zero;

        // adjust viewport
        RectTransform viewportRectTransform = viewport.GetComponent<RectTransform>();
        viewportRectTransform.anchorMin = new Vector2(viewportRectTransform.anchorMin.x, footerContainerRectTransform.anchorMax.y);
        viewportRectTransform.anchorMax = new Vector2(viewportRectTransform.anchorMax.x, headerContainerRectTransform.anchorMin.y);

        viewportRectTransform.offsetMin = Vector2.zero;
        viewportRectTransform.offsetMax = Vector2.zero;

    }

    public void AdjustSafeAreaHeader(RectTransform gameObjectToAdjustRectTransform)
    {
        gameObjectToAdjustRectTransform.anchorMin = new Vector2(gameObjectToAdjustRectTransform.anchorMin.x, gameObjectToAdjustRectTransform.anchorMin.y - (Screen.height - safeArea.position.y - safeArea.size.y) / Screen.height);
        gameObjectToAdjustRectTransform.anchorMax = new Vector2(gameObjectToAdjustRectTransform.anchorMax.x, (safeArea.position.y + safeArea.size.y) / Screen.height);

        gameObjectToAdjustRectTransform.offsetMin = Vector2.zero;
        gameObjectToAdjustRectTransform.offsetMax = Vector2.zero;
    }

    public void StartLoader(Action callbackUpdate, Action callbackInfiniteScroll)
    {

        float scrollLocalYPosition = scrollViewContainerRectTransform.localPosition.y;
        float scrollViewContainerHeight = scrollViewContainerRectTransform.sizeDelta.y;

        // it enters the updating zone
        if (!isInLoadingUpdatesScrollZone && !isInLoadingInfiniteScrollZone)
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
            else if (scrollViewContainerHeight - scrollLocalYPosition < LOADER_THRESHOLD_TO_INFINITE_SCROLL && !isLoadingInfiniteScroll) // starting infinite
            {
                //Debug.Log("Entering Infinite");
                StartInfiniteScroll(callbackInfiniteScroll);
            }
        }
        else if (isInLoadingUpdatesScrollZone && !isInLoadingInfiniteScrollZone && (scrollLocalYPosition < 0.9f * LOADER_UPDATE_THRESHOLD_TO_FADE && scrollLocalYPosition >= LOADER_UPDATE_THRESHOLD)) // leaving updates
        {
            //Debug.Log("Leaving Updates");
            AroundUpdateScroll(scrollLocalYPosition, false);
        }
        else if ((isInLoadingUpdatesScrollZone && !isLoadingUpdatesScroll && scrollLocalYPosition >= 0.9f * LOADER_UPDATE_THRESHOLD_TO_FADE) || (isInLoadingInfiniteScrollZone && !isLoadingInfiniteScroll && scrollViewContainerHeight - scrollLocalYPosition >= LOADER_THRESHOLD_TO_INFINITE_SCROLL)) // exiting scroll triggers
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
        AnalyticsController.GetInstance().Log(AnalyticsController.LOGS.OPEN_LINK, url);

        Application.OpenURL(url);
    }

    public void OnOpenBecomeMemberLink()
    {
        string url = "https://www.ccif.ch/la-ccif/demande-d-adhesion-fr.html";

        if (CacheController.globalConfig.lang == "de-DE")
            url = "https://www.hikf.ch/die-hikf/beitrittsgesuch.html";

        OnOpenLink(url);
    }

    public void OnReloadEmptyPage()
    {
        // we reload the cache to trigger the initial setup of the app
        CacheController.ClearAllCache();

    }

    public void InitMap(GameObject mapContainer, double longitude, double latitude, bool is_localization_set = true)
    {
        //Set parent
        mapContainer.transform.parent.gameObject.SetActive(is_localization_set);
        map.transform.SetParent(mapContainer.transform);
        map.transform.SetAsFirstSibling();

        //Change position to match parent
        mapRectTransform.offsetMin = Vector2.zero;
        mapRectTransform.offsetMax = Vector2.zero;

        //Change Map center and marker position
        marker?.SetPosition(longitude, latitude);
        map.SetPosition(longitude, latitude);
        map.Redraw();

        //Init button
        GameObject callToAction = mapContainer.transform.Find("Call To Action")?.gameObject;
        Button directions = callToAction?.GetComponent<Button>();
        directions?.onClick.AddListener(delegate () { Application.OpenURL("https://www.google.com/maps/dir//" + latitude + "," + longitude); });

    }

    public void ActivateMap()
    {
        if (Input.touchCount == 2)
        {
            mapControl.allowUserControl = true;
            scrollRect.vertical = false;
        }
        else
        {
            mapControl.allowUserControl = false;
            scrollRect.vertical = true;

        }
    }

}
