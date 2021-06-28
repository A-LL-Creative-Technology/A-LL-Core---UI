using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NavigationController : MonoBehaviour
{
    private static NavigationController instance;

    public static NavigationController GetInstance()
    {
        return instance;
    }
#pragma warning disable 0649
    readonly private float ANIMATION_STACK_DURATION = 0.4f;
    readonly private float ANIMATION_OVER_DURATION = 0.4f;
    readonly private float ANIMATION_POP_UP_DURATION = 0.4f;

    readonly private float ANIMATION_NOTIFICATION_DURATION = 0.4f;

    readonly private float ANIMATION_RESET_SCROLL_TOP_DURATION = 0.4f;

    public delegate void OnSinglePageContentLoaded(object sender, EventArgs e);
    public static OnSinglePageContentLoaded singlePageContentLoadedDelegate;

    public GameObject currentView;

    public GameObject scrollView;
    [SerializeField] private RectTransform scrollViewViewportRectTransform;
    public GameObject scrollViewContainer; // container for the Views Canvas

    public GameObject headerCanvas;
    public GameObject viewsCanvas;
    public GameObject footerCanvas;
    public GameObject overCanvas;
    public GameObject overlayCanvas;

    [SerializeField] private CanvasGroup headerContainerBackgroundCanvasGroup;

    [SerializeField] private RectTransform footerContainerBackgroundRectTransform;
    [SerializeField] private RectTransform footerContainerRectTransform;

    // save navigation initial values
    private Vector2 scrollViewViewportAnchorMin;
    private float footerContainerBackgroundHeight;
    private float footerContainerInitialPositionY;
    private float footerContainerBackgroundInitialPositionY;
    private float headerContainerBackgroundCanvasGroupAlpha;
    private Vector2 scrollViewViewportAnchorMax;

    public GameObject headerBackButton;
    public GameObject headerTitle;

    public EventSystem eventSystem;

    private RectTransform viewsCanvasRectTransform;

    private RectTransform overCanvasRectTransform;
    private RawImage overCanvasRawImage;

    [SerializeField] private GameObject popUpContainer;
    private GameObject currentActivePopUp = null;
    [SerializeField] GameObject popUpGeneral;

    [HideInInspector] public string notificationStringPrefix = "string:"; // prefix "string:" to be added at the beginning of the variable to use Localization as a string and not key

    [SerializeField] private GameObject notificationContainer;
    [SerializeField] private RawImage notificationBackground;
    [SerializeField] private LocalizeStringEvent notificationTitle;
    [SerializeField] private LocalizeStringEvent notificationBody;
    [SerializeField] private LocalizeStringEvent notificationCTA;

    private RectTransform notificationContainerRectTransform;
    private float notificationContainerInitialYPosition;
    private bool isNotificationInProgress = false;

    [SerializeField] private GameObject opaqueHeaderContainer;
    [SerializeField] private GameObject transparentHeaderContainer;


    [SerializeField] private GameObject globalAppLoaderContainer;
    [SerializeField] private GameObject globalSceneLoaderContainer;

    [SerializeField] private GameObject creaTechContainer;

    public enum NotificationActionLink
    {
        None,
        GeneratePassword,
        CreativeTechnologyEnabler,
    }

    [NonSerialized] public float overViewsInitialYPosition;

    [NonSerialized] public string currentSinglePageID = "-1";

    private string singlePageSuffix = "/{id}";

    private float headerStackAmount;

    private TextMeshProUGUI navigationBarTitle;

    // for Over mode
    private GameObject currentViewBackupOver;
    private List<NavigationConfig> viewsStackBackupOver;
    ////

    private bool isNewStack = false;
    private bool isClosingStack = false;

    [NonSerialized] public bool isNavigationInitializationInProgress = false;
    private bool isNavigationInProgress = false;

    private bool isPushToStackInProgress = false;

    private bool firstCacheLoad = true; // to prevent reexecuting the cache callback for the navigation when it is not the initial launch of the app

    [NonSerialized] public ScrollRectFaster scrollViewRect;
    //[NonSerialized] public ScrollRectFaster overScrollViewRect;

    private GameObject oldView;
    private GameObject nextView;

    public enum NavigationMode
    {
        DisplayOpen,
        StackOpen,
        StackClose,
        OverOpen,
        OverClose
    }

    [NonSerialized] public NavigationMode currentNavigationMode;

    private Dictionary<string, GameObject> viewsDict = new Dictionary<string, GameObject>();

    private List<NavigationConfig> viewsStack = new List<NavigationConfig>();

#pragma warning restore 0649

    private void Awake()
    {
        instance = this;

        isNavigationInitializationInProgress = true;

        // start the app loader
        OnGlobalAppLoaderOpen();
    }

    private void Start()
    {
        CacheController.OnCacheLoaded += OnCacheLoadedCallback;
        singlePageContentLoadedDelegate += OnSinglePageContentLoadedContinuePushToStackCallback;

        StartCoroutine(InitNavigation());
    }

    private void OnDestroy()
    {
        CacheController.OnCacheLoaded -= OnCacheLoadedCallback;
        singlePageContentLoadedDelegate -= OnSinglePageContentLoadedContinuePushToStackCallback;
    }

    private void Update()
    {
#if UNITY_ANDROID
        // Check if back button on Android OS UI was pressed this frame
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnStackClose();
        }
#endif
    }

    private void OnCacheLoadedCallback(object sender, EventArgs e)
    {

        if (!firstCacheLoad)
            return;

        firstCacheLoad = false;

        // close the app loader (before firing the event)
        OnGlobalAppLoaderClose();

        // A-LL CORE IS READY, we fire an event to tell the app controllers that they can start
        ALLCoreConfig.GetInstance().ALLCoreReady();

    }

    private IEnumerator InitNavigation()
    {
        // get scroll view rect
        scrollViewRect = scrollView.GetComponent<ScrollRectFaster>();

        // get over canvas raw image
        overCanvasRawImage = overCanvas.GetComponent<RawImage>();

        // disable global loader by default
        globalSceneLoaderContainer.SetActive(false);

        // disable crea tech by default
        creaTechContainer.SetActive(false);

        // disable crea tech by default
        popUpContainer.SetActive(false);

        // init overlay canvas
        InitOverlayCanvas();

        // Init normal views layout
        yield return StartCoroutine(InitNormalViews());

        // Init over views layout
        yield return StartCoroutine(InitOverViews());

        // canvas rect transform
        overCanvasRectTransform = overCanvas.GetComponent<RectTransform>();

        viewsCanvasRectTransform = viewsCanvas.GetComponent<RectTransform>();

        // disable notification panel by default
        notificationContainer.SetActive(false);
        notificationContainerRectTransform = notificationContainer.GetComponent<RectTransform>();
        notificationContainerInitialYPosition = notificationContainer.transform.localPosition.y;

        // setup back button
        headerStackAmount = overCanvasRectTransform.rect.width / 10.9f;
        headerBackButton.SetActive(false);
        headerBackButton.GetComponent<Button>().onClick.AddListener(OnStackClose);

        // header title
        navigationBarTitle = headerTitle.GetComponent<TextMeshProUGUI>();

        // trigger the cache load (but it will wait anyway for isNavigationInitializationInProgress
        CacheController.GetInitializationInstance().LoadAllCaches();

        isNavigationInitializationInProgress = false;
    }

    private void InitOverlayCanvas()
    {
        overlayCanvas.SetActive(true);

        // make sure all popups are deactivated
        foreach (Transform currentPopUpTransform in popUpContainer.transform)
        {
            currentPopUpTransform.gameObject.SetActive(false);
        }
    }

    private void InitViews(GameObject viewsContainer)
    {
        foreach (Transform currentView in viewsContainer.transform) // normal views
        {
            // store the views in a dictionary
            viewsDict.Add(currentView.name, currentView.gameObject);

            // and deactivate the views
            currentView.gameObject.SetActive(false);
        }
    }

    private IEnumerator InitNormalViews()
    {
        //while (!UIController.GetInstance().hasFinishedAdjustingSafeArea)
        //    yield return null;

        headerCanvas.SetActive(true);
        viewsCanvas.SetActive(true);
        footerCanvas.SetActive(true);

        foreach (Transform currentNormalView in scrollViewContainer.transform) // normal views
        {
            // activate the views (to force layout calculation)
            currentNormalView.gameObject.SetActive(true);
            currentNormalView.GetComponent<LayoutElement>().ignoreLayout = false;
        }

        // wait for the next frame
        yield return null;

        InitViews(scrollViewContainer);

        headerCanvas.SetActive(false);
        viewsCanvas.SetActive(false);
        footerCanvas.SetActive(false);
    }

    private IEnumerator InitOverViews()
    {

        if (overCanvas.transform.childCount == 0) // make sure we have at least one child
            yield break;

        overCanvas.SetActive(true);

        // make sure the over canvas raw image is deactivated by default
        overCanvasRawImage.enabled = false;

        overViewsInitialYPosition = overCanvas.transform.GetChild(0).localPosition.y;

        foreach (Transform currentOverView in overCanvas.transform) // over views
        {
            // activate the views (to force layout calculation)
            currentOverView.gameObject.SetActive(true);
        }

        // wait for the next frame
        yield return null;

        InitViews(overCanvas);

        overCanvas.SetActive(false);

    }

    public void Navigate(string routingStr)
    {

        //Log page in Analytics
        AnalyticsController.GetInstance()?.Log(AnalyticsController.LOGS.VISIT_PAGE, routingStr);

        // parse routing string
        string nextViewStr = ParseRoute(routingStr);

        // extract next view gameobject
        ExtractNextView(nextViewStr);

        UpdateCanvas();

        switch (currentNavigationMode)
        {
            case NavigationMode.DisplayOpen:


                // set new view activation
                DisplayNextView();

                break;

            case NavigationMode.StackOpen:

                // push to the stack
                PushToStack();

                break;

            case NavigationMode.StackClose:

                // pop from stack
                PopFromStack();

                break;

            case NavigationMode.OverOpen:

                // display the view over the others
                DisplayOverViews();

                break;

            case NavigationMode.OverClose:

                // remove the view displayed over the others
                CloseFromOver();

                break;

            default:
                break;
        }

        // DO NOT PUT ANY CODE HERE AS THERE ARE ANIMATIONS ASYNC IN THE SWITCH FUNCTIONS BUT IN THE CALLBACK BELOW "NavigationCallback()"
    }

    // parsing of the routing str
    // global pages (plural) => ex: news
    // single page (with id) => ex: news/12
    // other pages (singular) => ex: welcome
    private string ParseRoute(string routingStr)
    {
        // detects if '/' in string
        string[] routeElements = routingStr.Split('/');

        switch (routeElements.Length)
        {
            // normal page
            case 1:
                return routeElements[0];

            // single page with ID
            case 2:
                currentSinglePageID = routeElements[1];

                // add suffix for single page
                string nextViewStr = routeElements[0] + singlePageSuffix;
                return nextViewStr;
        }

        GlobalController.DialogMe("Route parsing failed for string: " + routingStr);
        return "";

    }


    // used to make sure a view is not being displayed or navigation controller has successfully been integrated
    public IEnumerator QueueToNavigate(Action navigate)
    {

        while (isNavigationInProgress || isNavigationInitializationInProgress)
            yield return null;

        eventSystem.enabled = false; // deactivate to prevent multiple touch while navigating

        isNavigationInProgress = true;

        navigate?.Invoke();
    }

    private void UpdateCanvas()
    {

        // toggle canvas depending over/normal mode
        if (currentViewBackupOver == null)
        {
            headerCanvas.SetActive(true);
            viewsCanvas.SetActive(true);
            footerCanvas.SetActive(true);
            overCanvas.SetActive(false);
        }
        else
        {
            headerCanvas.SetActive(false);
            viewsCanvas.SetActive(false);
            footerCanvas.SetActive(false);
            overCanvas.SetActive(true);
        }
    }

    public void OnDisplayOpen(string routingStr)
    {
        currentNavigationMode = NavigationMode.DisplayOpen;

        // wait for other navigation to finish
        StartCoroutine(QueueToNavigate(() => Navigate(routingStr)));

    }


    public void OnStackOpen(string routingStr)
    {
        currentNavigationMode = NavigationMode.StackOpen;

        // wait for other navigation to finish
        StartCoroutine(QueueToNavigate(() => Navigate(routingStr)));

    }

    public void OnStackClose()
    {
        currentNavigationMode = NavigationMode.StackClose;

        // wait for other navigation to finish
        StartCoroutine(QueueToNavigate(() => Navigate("-1")));
    }

    public void OnOverOpen(string routingStr)
    {
        currentNavigationMode = NavigationMode.OverOpen;

        // wait for other navigation to finish
        StartCoroutine(QueueToNavigate(() => Navigate(routingStr)));
    }

    // called to get out of the "over" mode
    public void OnOverClose()
    {
        currentNavigationMode = NavigationMode.OverClose;

        // wait for other navigation to finish
        StartCoroutine(QueueToNavigate(() => Navigate("-1")));
    }

    // to display the popup
    public void OnPopUpOpen(GameObject popUp)
    {

        currentActivePopUp = popUp;

        popUpContainer.SetActive(true);

        currentActivePopUp.SetActive(true);
        currentActivePopUp.transform.localScale = Vector3.zero;
        LeanTween.scale(currentActivePopUp, Vector3.one, ANIMATION_POP_UP_DURATION).setEaseOutBack();

    }

    // to close the popup
    public void OnPopUpClose()
    {
        if (!currentActivePopUp)
            return;

        LeanTween.scale(currentActivePopUp, Vector3.zero, ANIMATION_POP_UP_DURATION).setEaseInBack().setOnComplete(() => {

            popUpContainer.SetActive(false);

            currentActivePopUp = null;

        });
    }

    private IEnumerator QueueNotification(bool isSuccess, float secondsBeforeAutoHide, string title, string body, string localizationTable, string cta, NotificationActionLink ctaLink)
    {
        while (isNotificationInProgress)
            yield return null;

        OnNotificationOpen(isSuccess, secondsBeforeAutoHide, title, body, localizationTable, cta, ctaLink);
    }

    // for title and body, use the prefix "string:" at the beginning of the variable to use the string as is without using it as a Localization key
    public void OnNotificationOpen(bool isSuccess, float secondsBeforeAutoHide, string title = "", string body = "", string localizationTable = "Main", string cta = "", NotificationActionLink ctaLink = NotificationActionLink.None)
    {
        if (isNotificationInProgress)
        {
            // if it is a success notification, we close the current notification and make the success notification in the queue
            if (isSuccess)
            {
                OnNotificationClose();
                StartCoroutine(QueueNotification(isSuccess, secondsBeforeAutoHide, title, body, localizationTable, cta, ctaLink));
            }

            return;
        }

        isNotificationInProgress = true;

        // toggle notification
        notificationContainer.SetActive(true);

        if (isSuccess)
        {
            notificationBackground.color = new Color(111 / 255f, 195 / 255f, 92 / 255f);
        }
        else
        {
            notificationBackground.color = new Color(254 / 255f, 104 / 255f, 78 / 255f);
        }

        if (secondsBeforeAutoHide > 0)
        {
            CancelInvoke();
            Invoke("OnNotificationClose", secondsBeforeAutoHide);
        }

        // toggle title
        if (!String.IsNullOrEmpty(title))
        {
            notificationTitle.gameObject.SetActive(true);

            string titleStringValue = title.Substring(0, notificationStringPrefix.Length);
            if (titleStringValue == notificationStringPrefix)
            {
                notificationTitle.enabled = false;

                notificationTitle.gameObject.GetComponent<TextMeshProUGUI>().text = title.Substring(notificationStringPrefix.Length);
            }
            else
            {
                notificationTitle.enabled = true;

                notificationBody.StringReference.TableReference = localizationTable;
                notificationTitle.StringReference.TableEntryReference = title;
            }
        }
        else
        {
            notificationTitle.gameObject.SetActive(false);
        }

        // toggle body
        if (!String.IsNullOrEmpty(body))
        {
            notificationBody.gameObject.SetActive(true);

            string bodyStringValue = body.Substring(0, notificationStringPrefix.Length);
            if (bodyStringValue == notificationStringPrefix)
            {
                notificationBody.enabled = false;

                notificationBody.gameObject.GetComponent<TextMeshProUGUI>().text = body.Substring(notificationStringPrefix.Length);
            }
            else
            {
                notificationBody.enabled = true;

                notificationBody.StringReference.TableReference = localizationTable;
                notificationBody.StringReference.TableEntryReference = body;
            }
        }
        else
        {
            notificationBody.gameObject.SetActive(false);
        }

        // toggle cta
        notificationCTA.gameObject.SetActive(false);


        // animate the notification
        notificationContainer.transform.localPosition = new Vector3(notificationContainer.transform.localPosition.x, notificationContainerInitialYPosition + notificationContainerRectTransform.rect.height, notificationContainer.transform.localPosition.z);

        LeanTween.moveLocalY(notificationContainer, notificationContainerInitialYPosition, ANIMATION_NOTIFICATION_DURATION).setEaseInOutExpo();
    }

    public void OnNotificationClose()
    {

        LeanTween.moveLocalY(notificationContainer, notificationContainerInitialYPosition + notificationContainerRectTransform.rect.height, ANIMATION_NOTIFICATION_DURATION).setEaseInOutExpo().setOnComplete(() =>
        {
            notificationContainer.SetActive(false);

            isNotificationInProgress = false;

        });
    }

    public void OnGlobalAppLoaderOpen()
    {
        // toggle global loader
        globalAppLoaderContainer.SetActive(true);
    }

    public void OnGlobalAppLoaderClose()
    {
        // toggle global loader
        globalAppLoaderContainer.SetActive(false);
    }


    public void OnGlobalSceneLoaderOpen()
    {
        // toggle global loader
        globalSceneLoaderContainer.SetActive(true);
    }

    public void OnGlobalSceneLoaderClose()
    {
        // toggle global loader
        globalSceneLoaderContainer.SetActive(false);
    }

    public void OnCreaTechOpen()
    {
        // toggle global loader
        creaTechContainer.SetActive(true);
    }

    public void OnCreaTechClose()
    {
        // toggle global loader
        creaTechContainer.SetActive(false);
    }

    public GameObject BuildPopUpGeneral(string title, string description)
    {
        popUpGeneral.transform.Find("Text 2").GetComponent<TextMeshProUGUI>().text = title;
        popUpGeneral.transform.Find("Text 3").GetComponent<TextMeshProUGUI>().text = description;

        return popUpGeneral;
    }

    private void ExtractNextView(string nextViewStr)
    {
        // in the case it is called by FromOver or PopFromStack
        if (nextViewStr == "-1")
        {
            nextView = null;
            return;
        }


        if (viewsDict.TryGetValue(nextViewStr, out GameObject nextViewGameObject))
        {
            nextView = nextViewGameObject;
        }
        else
        {
            GlobalController.DialogMe("The UI view '" + nextViewStr + "' cannot be found.");
        }
    }

    private void PushToStack()
    {
        isPushToStackInProgress = true;

        // if stack is currently empty, we fill it with parent and child
        if (viewsStack.Count == 0)
        {
            viewsStack.Add(new NavigationConfig(currentView, scrollViewContainer.transform.localPosition.y));
            viewsStack.Add(new NavigationConfig(nextView, 0));
            isNewStack = true;

            // activate gesture (swipe right for pop from stack)
            GestureController.GetInstance().isGestureActive = true;
        }
        else
        {
            viewsStack[viewsStack.Count - 1].scrollBarValue = scrollViewContainer.transform.localPosition.y; // update parent scroll bar value
            viewsStack.Add(new NavigationConfig(nextView, 0));
        }


        oldView = currentView;
        currentView = nextView;

        scrollViewRect.StopMovement();

        // check whether we need to make the header transparent
        if (currentView.GetComponent<SinglePageController>().hasTransparentHeader)
        {
            scrollViewViewportAnchorMax = scrollViewViewportRectTransform.anchorMax;
            scrollViewViewportRectTransform.anchorMax = Vector2.one;

            headerContainerBackgroundCanvasGroupAlpha = headerContainerBackgroundCanvasGroup.alpha;
            headerContainerBackgroundCanvasGroup.alpha = 0;

            opaqueHeaderContainer.SetActive(false);
            transparentHeaderContainer.SetActive(true);
        }
        else
        {
            opaqueHeaderContainer.SetActive(true);
            transparentHeaderContainer.SetActive(false);
        }

        // we prepare the child page for the animation
        currentView.GetComponent<LayoutElement>().ignoreLayout = false;


        currentView.GetComponent<CanvasGroup>().alpha = 0; // to avoid seeing the view before its animation

        currentView.SetActive(true); // this line triggers the "OnEnable" on the Single Page Script

        scrollViewRect.enabled = false;

    }

    // this function is triggered by the callback when the content is loaded on the Single Page Script. This makes sure the layout is computed BEFORE showing the page
    private void OnSinglePageContentLoadedContinuePushToStackCallback(object sender, EventArgs e)
    {
        //we make sure this function is called only after PushToStack() function.
        if (!isPushToStackInProgress)
            return;

        // we make sure the event on loaded single page is called here only after push to stack and nowhere else (like for instance after a OnOverClose)
        if (currentNavigationMode != NavigationMode.StackOpen)
            return;

        currentView.GetComponent<LayoutElement>().ignoreLayout = true;

        // we change child position with respect to the amount of scroll we made on the parent
        currentView.transform.localPosition = new Vector3(currentView.transform.localPosition.x, -viewsStack[viewsStack.Count - 2].scrollBarValue, currentView.transform.localPosition.z);

        // position child view to the right of the the screen
        float screenWidth = viewsCanvasRectTransform.rect.width; // !!!! sometimes Screen.width does not give same value as full screen views canvas width. Weird!! So don't trust Screen.width from Unity

        currentView.transform.localPosition = new Vector3(screenWidth, currentView.transform.localPosition.y, currentView.transform.localPosition.z);

        currentView.GetComponent<CanvasGroup>().alpha = 1;


        // we do the animation
        LeanTween.moveLocalX(currentView, 0, ANIMATION_STACK_DURATION).setEase(LeanTweenType.easeInOutExpo).setOnComplete(() =>
        {

            // reset the layout for the scroll on the children
            currentView.GetComponent<LayoutElement>().ignoreLayout = false;

            // reset scroll to the top
            Transform scrollViewContent = currentView.transform.parent;
            scrollViewContent.localPosition = Vector3.zero;

            // hide the parent view
            oldView.SetActive(false);

            scrollViewRect.enabled = true;

            oldView = null;
            nextView = null;

            isPushToStackInProgress = false;

            // end of the animation
            NavigationCallback();
        });
    }



    private void PopFromStack()
    {
        // we verify we have enough views
        if (viewsStack.Count <= 1)
        {
            NavigationCallback();
            return;
        }

        // display the parent view from the stack
        oldView = currentView;
        currentView = viewsStack[viewsStack.Count - 2].view;

        scrollViewRect.StopMovement();

        scrollViewRect.enabled = false;

        currentView.SetActive(true);

        // we save the scroll amount for the child
        float childScrollBarAmount = scrollViewContainer.transform.localPosition.y;

        // deactivate layout on the child
        oldView.GetComponent<LayoutElement>().ignoreLayout = true;

        // we correct the child position for the above scroll on the parent
        oldView.transform.localPosition = new Vector3(oldView.transform.localPosition.x, childScrollBarAmount - viewsStack[viewsStack.Count - 2].scrollBarValue, oldView.transform.localPosition.z);

        // we change the scroll
        scrollViewContainer.transform.localPosition = new Vector3(scrollViewContainer.transform.localPosition.x, viewsStack[viewsStack.Count - 2].scrollBarValue, scrollViewContainer.transform.localPosition.z);

        // reset header (because of possible transparency)
        if (oldView.GetComponent<SinglePageController>().hasTransparentHeader)
        {
            LeanTween.value(1, scrollViewViewportAnchorMax.y, ANIMATION_STACK_DURATION).setEase(LeanTweenType.easeInOutExpo).setOnUpdate((float value) =>
            {
                scrollViewViewportRectTransform.anchorMax = new Vector2(1, value);
            }).setOnComplete(() =>
            {

                ContinuePopFromStack();
            });
        }
        else
        {
            ContinuePopFromStack();
        }



    }

    private void ContinuePopFromStack()
    {
        scrollViewViewportRectTransform.anchorMax = scrollViewViewportAnchorMax;
        headerContainerBackgroundCanvasGroup.alpha = headerContainerBackgroundCanvasGroupAlpha;
        opaqueHeaderContainer.SetActive(true);
        transparentHeaderContainer.SetActive(false);

        // we do the animation for the pop
        float screenWidth = viewsCanvasRectTransform.rect.width; // !!!! sometimes Screen.width does not give same value as full screen views canvas width. Weird!! So don't trust Screen.width from Unity
        LeanTween.moveLocalX(oldView, screenWidth, ANIMATION_STACK_DURATION).setEase(LeanTweenType.easeInOutExpo).setOnComplete(() =>
        {
            // hide the child view
            oldView.SetActive(false);

            // remove the child from the stack
            viewsStack.RemoveAt(viewsStack.Count - 1);

            if (viewsStack.Count == 1)
            {
                isClosingStack = true;
                viewsStack.Clear();

                // deactivate gesture
                GestureController.GetInstance().isGestureActive = false;
            }

            scrollViewRect.enabled = true;

            oldView = null;
            nextView = null;

            // end of the animation
            NavigationCallback();
        });
    }

    // we backup the current state of views that we will reset after the over is done
    private void DisplayOverViews()
    {
        // prevent calling over mode twice
        if (currentViewBackupOver != null)
        {
            NavigationCallback();
            return;
        }

        oldView = currentView;
        currentView = nextView;

        currentViewBackupOver = oldView;
        viewsStackBackupOver = new List<NavigationConfig>(viewsStack);

        viewsStack.Clear();

        // display all canvas for the animation
        headerCanvas.SetActive(true);
        viewsCanvas.SetActive(true);
        footerCanvas.SetActive(true);
        overCanvas.SetActive(true);

        overCanvasRawImage.enabled = false;

        // prepare the over view (needs to be first so that automatic layout can compute its space)
        currentView.SetActive(true);

        // set view to starting position
        float screenHeight = overCanvasRectTransform.rect.height; // !!!! sometimes Screen.height does not give same value as full screen views canvas height. Weird!! So don't trust Screen.height from Unity
        currentView.transform.localPosition = new Vector3(currentView.transform.localPosition.x, overViewsInitialYPosition + screenHeight, currentView.transform.localPosition.z);

        //we do the animation
        LeanTween.moveLocalY(currentView, overViewsInitialYPosition, ANIMATION_OVER_DURATION).setEase(LeanTweenType.easeInOutExpo).setOnComplete(() =>
        {

            // hide views below
            headerCanvas.SetActive(false);
            viewsCanvas.SetActive(false);
            footerCanvas.SetActive(false);

            oldView.SetActive(false);

            // make sure the end position is precise (not precise with animation)
            currentView.transform.localPosition = new Vector3(currentView.transform.localPosition.x, overViewsInitialYPosition, currentView.transform.localPosition.z);

            // reactivate the background image (necessary if input fields moves up the over view)
            overCanvasRawImage.enabled = true;

            oldView = null;
            nextView = null;

            // end of the animation
            NavigationCallback();

        });

    }

    private void CloseFromOver()
    {
        // prevent from calling this function when over mode was not previously activated
        if (currentViewBackupOver == null)
        {
            NavigationCallback();
            return;
        }

        oldView = currentView;
        currentView = currentViewBackupOver;

        viewsStack = new List<NavigationConfig>(viewsStackBackupOver);

        currentView.SetActive(true);

        // display all canvas for the animation
        headerCanvas.SetActive(true);
        viewsCanvas.SetActive(true);
        footerCanvas.SetActive(true);
        overCanvas.SetActive(true);

        overCanvasRawImage.enabled = false;

        // we do the animation
        float screenHeight = overCanvasRectTransform.rect.height; // !!!! sometimes Screen.height does not give same value as full screen views canvas height. Weird!! So don't trust Screen.height from Unity
        LeanTween.moveLocalY(oldView, overViewsInitialYPosition + screenHeight, ANIMATION_OVER_DURATION).setEase(LeanTweenType.easeInOutExpo).setOnComplete(() =>
        {

            // reset view to initial position
            oldView.transform.localPosition = new Vector3(oldView.transform.localPosition.x, overViewsInitialYPosition, oldView.transform.localPosition.z);

            // hide views below
            oldView.SetActive(false);
            overCanvas.SetActive(false);

            oldView = null;
            nextView = null;

            currentViewBackupOver = null;
            viewsStackBackupOver.Clear();
            viewsStackBackupOver = null;

            // end of the animation
            NavigationCallback();

            // HACK! These 2 lines force Unity Auto Layout to recompute the page as sometimes the page was moving upward weirdly
            currentView.SetActive(false);
            currentView.SetActive(true);

        });
    }

    // reset scroll to top
    public void ResetScrollBar(bool isAnimated = false)
    {

        scrollViewRect.StopMovement();
        if (isAnimated)
            LeanTween.moveLocalY(scrollViewContainer, 0, ANIMATION_RESET_SCROLL_TOP_DURATION).setEase(LeanTweenType.easeInOutQuint);
        else
            scrollViewRect.verticalNormalizedPosition = 1;
    }

    private void ResetViewsStack()
    {
        // set active false for the views currently in the stack
        foreach (NavigationConfig currentViewStack in viewsStack)
        {
            currentViewStack.view.SetActive(false);
        }

        if (viewsStack.Count > 0)
        {
            isClosingStack = true;
        }

        viewsStack.Clear();
    }

    private void DisplayNextView()
    {
        // reset views in stack
        ResetViewsStack();

        // reset scroll to top
        ResetScrollBar();

        oldView = currentView;
        currentView = nextView;

        oldView.SetActive(false);
        currentView.SetActive(true);

        oldView = null;
        nextView = null;

        NavigationCallback();
    }


    private void NavigationCallback()
    {
        // make UI adjustment
        UpdateUI();

        // reactivate touch
        eventSystem.enabled = true; // deactivate to prevent multiple touch while navigating
    }

    private void UpdateUI()
    {
        // check whether the nextView is a view associated with a menu button
        if (UIMenuController.GetInstance().menuButtons.TryGetValue(currentView.name, out GameObject menuButton))
        {
            UIMenuController.GetInstance().ChangeMenuButtonState(menuButton);

            navigationBarTitle.gameObject.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = "CODE_" + currentView.name;
        }

        // back button
        if (isNewStack)
            ShowStackNavigation();
        else if (isClosingStack)
            HideStackNavigation();
        else
            ResetNavigationBooleans();

    }

    private void ResetNavigationBooleans()
    {
        isNavigationInProgress = false;
        isNewStack = false;
        isClosingStack = false;
    }



    private void ShowStackNavigation()
    {

        // hide the navigation
        scrollViewViewportAnchorMin = scrollViewViewportRectTransform.anchorMin;
        footerContainerBackgroundHeight = footerContainerBackgroundRectTransform.rect.height;
        footerContainerInitialPositionY = footerContainerRectTransform.localPosition.y;
        footerContainerBackgroundInitialPositionY = footerContainerBackgroundRectTransform.localPosition.y;

        scrollViewViewportRectTransform.anchorMin = Vector2.zero;
        LeanTween.moveLocalY(footerContainerRectTransform.gameObject, footerContainerRectTransform.localPosition.y - footerContainerBackgroundHeight, ANIMATION_STACK_DURATION).setEase(LeanTweenType.easeInOutExpo);
        LeanTween.moveLocalY(footerContainerBackgroundRectTransform.gameObject, footerContainerBackgroundRectTransform.localPosition.y - footerContainerBackgroundHeight, ANIMATION_STACK_DURATION).setEase(LeanTweenType.easeInOutExpo);

        // we do the animation for the back button
        headerBackButton.SetActive(true);

        LeanTween.moveX(headerBackButton, headerBackButton.transform.position.x + headerStackAmount, 0.4f).setEase(LeanTweenType.easeInOutExpo);
        LeanTween.moveX(headerTitle, headerTitle.transform.position.x + headerStackAmount, 0.4f).setEase(LeanTweenType.easeInOutExpo).setOnComplete(() =>
        {
            ResetNavigationBooleans();
        });
    }

    private void HideStackNavigation()
    {
        // reset the bottom navigation
        scrollViewViewportRectTransform.anchorMin = scrollViewViewportAnchorMin;

        LeanTween.moveLocalY(footerContainerRectTransform.gameObject, footerContainerRectTransform.localPosition.y + footerContainerBackgroundHeight, ANIMATION_STACK_DURATION).setEase(LeanTweenType.easeInOutExpo).setOnComplete(() => {
            footerContainerRectTransform.localPosition = new Vector3(footerContainerRectTransform.localPosition.x, footerContainerInitialPositionY, footerContainerRectTransform.localPosition.z);
        });
        LeanTween.moveLocalY(footerContainerBackgroundRectTransform.gameObject, footerContainerBackgroundRectTransform.localPosition.y + footerContainerBackgroundHeight, ANIMATION_STACK_DURATION).setEase(LeanTweenType.easeInOutExpo).setOnComplete(() => {
            footerContainerBackgroundRectTransform.localPosition = new Vector3(footerContainerBackgroundRectTransform.localPosition.x, footerContainerBackgroundInitialPositionY, footerContainerBackgroundRectTransform.localPosition.z);
        });


        // we do the animation for the back button
        LeanTween.moveX(headerBackButton, headerBackButton.transform.position.x - headerStackAmount, 0.4f).setEase(LeanTweenType.easeInOutExpo).setOnComplete(() =>
        {
            headerBackButton.SetActive(false);
        });

        LeanTween.moveX(headerTitle, headerTitle.transform.position.x - headerStackAmount, 0.4f).setEase(LeanTweenType.easeInOutExpo).setOnComplete(() =>
        {
            ResetNavigationBooleans();
        });
    }

    public bool DoesSceneExists(string sceneName)
    {

        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            string currentSceneName = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));

            if (currentSceneName == sceneName)
                return true;
        }

        return false;
    }

    public void OnLoadNextSceneAsync(string sceneName) // necessary to make it accessible to buttons
    {
        LoadNextSceneAsync(sceneName);
    }

    public bool LoadNextSceneAsync(string sceneName)
    {
        if (DoesSceneExists(sceneName))
        {
            OnGlobalAppLoaderOpen(); // no need to cancel it as GO is destroyed with new scene

            StartCoroutine(LoadSceneAsync(sceneName));

            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // TO DO: manage error when scene cannot be found/loaded

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        // Wait until the asynchronous scene fully loads
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

    }

    public class NavigationConfig
    {
        public GameObject view;
        public float scrollBarValue;

        public NavigationConfig(GameObject view, float scrollValue)
        {
            this.view = view;
            this.scrollBarValue = scrollValue;
        }
    }



}
