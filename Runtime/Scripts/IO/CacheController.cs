using System.IO;
using UnityEngine;
using System;
using static CacheModel;
using System.Collections;

public class CacheController : MonoBehaviour
{
    private static CacheController instance;
    private static CacheController initializationInstance; // used only for initialization purpose

    public static CacheController GetInstance()
    {
        return instance;
    }

    public static CacheController GetInitializationInstance()
    {
        return initializationInstance;
    }
#pragma warning disable 0649
    readonly private float WAIT_TIME_FOR_LATE_SUBSCRIBERS = 1f; // in seconds

    public static event EventHandler OnCacheLoaded;
    public static event EventHandler OnInitDirectories;
    public static event EventHandler OnAppCacheToBeLoaded;
    public static event EventHandler OnPermanentCacheToBeLoaded;
    public static event EventHandler OnNormalCacheToBeLoaded;

    // TO ADD A NEW CACHE ELEMENT
    // 1. Add field static variable in CacheController
    // 2. Add the loading in LoadAllCaches()
    // 3. Add the type as a child of Cache
    [HideInInspector]
    public AppConfig appConfig = new AppConfig();

    [HideInInspector]
    public APIConfig apiConfig = new APIConfig();

    [HideInInspector]
    public static string appCacheLocation;

    private static string standardSceneCacheLocation;
    public static string permanentSceneCacheLocation;

    public static string standardConfigCacheLocation;
    public static string standardImagesCacheLocation;


    public static bool isCacheLoaded = false;

    private int nbSubscribersCacheLoaded;

#pragma warning restore 0649

    private void Awake()
    {
        // WE DO NOT INITIALIZE THE CACHE PUBLIC INSTANCE HERE AS WE WANT TO MAKE SURE THE CACHE IS PROPERLY LOADED BEFORE USED
        initializationInstance = this;

    }

    public static void InitDirectories()
    {
        string rootCacheLocation = Path.Combine(Application.persistentDataPath, "Cache");

        appCacheLocation = Path.Combine(rootCacheLocation, "App");

        string scenesCacheLocation = Path.Combine(rootCacheLocation, "Scenes");
        string sceneCacheLocation = Path.Combine(scenesCacheLocation, GetInitializationInstance().gameObject.scene.name);

        standardSceneCacheLocation = Path.Combine(sceneCacheLocation, "Standard");
        permanentSceneCacheLocation = Path.Combine(sceneCacheLocation, "Permanent");

        standardConfigCacheLocation = Path.Combine(standardSceneCacheLocation, "Config");
        standardImagesCacheLocation = Path.Combine(standardSceneCacheLocation, "Images");


        CreateDirectory(rootCacheLocation);
        CreateDirectory(appCacheLocation);
        CreateDirectory(scenesCacheLocation);

        CreateDirectory(sceneCacheLocation);
        CreateDirectory(standardSceneCacheLocation);
        CreateDirectory(permanentSceneCacheLocation);

        CreateDirectory(standardConfigCacheLocation);

        CreateDirectory(standardImagesCacheLocation);

        if (OnInitDirectories != null)
        {
            OnInitDirectories(GetInstance(), EventArgs.Empty);
        }

    }

    public static void ClearAllCache()
    {
        RemoveDirectory(standardSceneCacheLocation);

        // recreate the empty Cache Directory
        InitDirectories();

        // retrigger loading of elements
        LoadNormalCaches();

        GetInitializationInstance().StartCoroutine(GetInitializationInstance().CacheIsLoaded());

        GlobalController.LogMe("Normal caches successfully loaded from location: " + Application.persistentDataPath);
    }

    // triggered from NavigationController
    public void LoadAllCaches()
    {
        LoadAppCaches();
        LoadPermanentCaches();
        LoadNormalCaches();

        StartCoroutine(CacheIsLoaded());

        GlobalController.LogMe("All caches successfully loaded from location: " + Application.persistentDataPath);
    }

    private IEnumerator CacheIsLoaded()
    {
        while (NavigationController.GetInstance().isNavigationInitializationInProgress)
            yield return null;

        instance = this;


        // fires the event to notify that cache has been loaded
        FiresCacheLoaded();

        if (OnCacheLoaded != null)
        {
            nbSubscribersCacheLoaded = OnCacheLoaded.GetInvocationList().Length;
            StartCoroutine(WatchForLateCacheLoadedSubscribers());
        }
        else
        {
            NavigationController.GetInstance().CacheLoadedSuccessfully();
        }

        isCacheLoaded = true;

    }

    // to avoid having a late subscriber while the OnCacheLoaded event has already fired during initialization
    private IEnumerator WatchForLateCacheLoadedSubscribers()
    {

        float startTime = Time.time;

        bool hasLateSubscriberArrived = false;

        while (startTime + WAIT_TIME_FOR_LATE_SUBSCRIBERS > Time.time)
        {
            if (OnCacheLoaded.GetInvocationList().Length > nbSubscribersCacheLoaded)
            {
                nbSubscribersCacheLoaded = OnCacheLoaded.GetInvocationList().Length;

                hasLateSubscriberArrived = true;
            }

            yield return null;
        }

        // verify if late subscriber has arrived
        if (hasLateSubscriberArrived)
        {
            FiresCacheLoaded();
        }

        NavigationController.GetInstance().CacheLoadedSuccessfully();

    }

    private void FiresCacheLoaded()
    {
        // fires the event to notify that cache has been loaded
        if (OnCacheLoaded != null)
        {
            OnCacheLoaded(this, EventArgs.Empty);
        }
    }

    private static void LoadAppCaches()
    {
        LoadConfigFromDisk(GetInitializationInstance().appConfig);

        LoadConfigFromDisk(GetInitializationInstance().apiConfig, () => {
            GetInitializationInstance().apiConfig.Save(BaseConfig.SaveType.NoEvent); // if no cache found, we create a default config, so that state of the app always starts as if guest mode
        });

        // fires the event to notify that cache can be loaded
        if (OnAppCacheToBeLoaded != null)
        {
            OnAppCacheToBeLoaded(GetInitializationInstance(), EventArgs.Empty);
        }

    }

    private static void LoadPermanentCaches()
    {
        // fires the event to notify that cache can be loaded
        if (OnPermanentCacheToBeLoaded != null)
        {
            OnPermanentCacheToBeLoaded(GetInitializationInstance(), EventArgs.Empty);
        }

    }

    private static void LoadNormalCaches()
    {
        // fires the event to notify that cache can be loaded
        if (OnNormalCacheToBeLoaded != null)
        {
            OnNormalCacheToBeLoaded(GetInitializationInstance(), EventArgs.Empty);
        }

    }

    // load the cache image from disk
    public static void LoadImageFromDisk(string imageID, string imageURL, string imageCacheLocation, bool isSmall, Action<Texture2D> callback)
    {
        string prefix = isSmall ? "_small_" : "";
        string imagePath = Path.Combine(imageCacheLocation, imageID + prefix + ".png");

        // first verify if available in the cache
        if (File.Exists(imagePath))
        {
            Texture2D image = new Texture2D(1, 1);

            image.LoadImage(File.ReadAllBytes(imagePath));

            callback?.Invoke(image);
        }
        else
        {
            // no image found on disk.
            callback?.Invoke(null);
        }

    }

    // load the cache file from disk
    public static CacheConfigGeneric LoadConfigFromDisk<CacheConfigGeneric>(CacheConfigGeneric baseConfig, Action noCacheCallback = null)
    {
        BaseConfig baseConfigCast = baseConfig as BaseConfig;

        string filePath = baseConfigCast.GetFullFilePath();

        if (File.Exists(filePath))
        {
            GlobalController.LogMe("Cache is beeing loaded from: " + filePath);

            // read the json from the file into a string
            string cacheJson = File.ReadAllText(filePath);

            // overwrite to object instead of creating a new one
            JsonUtility.FromJsonOverwrite(cacheJson, baseConfig);
        }
        else
        {
            // clear last update value to make sure the cache loaded in memory is considered empty (in case of a logout for instance)
            baseConfigCast.Clear();

            noCacheCallback?.Invoke();

            GlobalController.LogMe("No cache file at location: " + filePath);
        }

        return baseConfig;

    }

    // save the cache to file
    public static void SaveConfigToDisk<CacheConfigGeneric>(CacheConfigGeneric baseConfig)
    {
        BaseConfig baseConfigCast = baseConfig as BaseConfig;

        string cacheJSON = JsonUtility.ToJson(baseConfig);

        File.WriteAllText(baseConfigCast.GetFullFilePath(), cacheJSON);

    }

    public static void CreateDirectory(string directoryPath)
    {

        string parentDirectory = Path.GetDirectoryName(directoryPath);

        if (Directory.Exists(parentDirectory))
        {
            Directory.CreateDirectory(directoryPath); // creates only if it does not already exist
        }
    }

    public static void RemoveDirectory(string directoryPath)
    {

        if (Directory.Exists(Path.GetDirectoryName(directoryPath)))
        {
            Directory.Delete(directoryPath, true);

        }
    }
}
