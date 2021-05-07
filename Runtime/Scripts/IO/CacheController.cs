using System.IO;
using UnityEngine;
using System;
using static CacheModel;
using System.Collections;
using Proyecto26;
using UnityEngine.SceneManagement;

public class CacheController : MonoBehaviour
{
    private static CacheController instance;

    public static CacheController GetInstance()
    {
        return instance;
    }

    public static event EventHandler OnCacheLoaded;

    // TO ADD A NEW CACHE ELEMENT
    // 1. Add field static variable in CacheController
    // 2. Add the loading in LoadAllCaches()
    // 3. Add the type as a child of Cache
    public AppConfig appConfig = new AppConfig();

    public SceneConfig sceneConfig = new SceneConfig();
    public UserConfig userConfig = new UserConfig();

    public NewsConfig newsConfig = new NewsConfig();
    public InnovationsConfig innovationsConfig = new InnovationsConfig();
    public EventsConfig eventsConfig = new EventsConfig();
    public FilteredEventsConfig filteredEventsConfig = new FilteredEventsConfig(); // no need to load it at startup
    public MyEventsIDsConfig myEventsIDsConfig = new MyEventsIDsConfig();
    public TagsConfig tagsConfig = new TagsConfig();
    public SurveyRelationalsConfig surveyRelationalsConfig = new SurveyRelationalsConfig();
    public ContactsConfig contactsConfig = new ContactsConfig();
    public ProvidersConfig providersConfig = new ProvidersConfig();
    public ServicesConfig servicesConfig = new ServicesConfig();
    public CreaTechsConfig creaTechsConfig = new CreaTechsConfig();

    public static string appCacheLocation;

    private static string standardSceneCacheLocation;
    public static string permanentSceneCacheLocation;

    public static string standardConfigCacheLocation;
    private static string standardImagesCacheLocation;

    public static string imagesNewsCacheLocation;
    public static string imagesInnovationsCacheLocation;
    public static string imagesEventsCacheLocation;

    public static bool isCacheLoaded = false;

    private void Awake()
    {
        instance = this;

        // define root cache location
        InitDirectories();

    }

    // Start is called before the first frame update
    private void Start()
    {
        LoadAllCaches();

    }

    private static void InitDirectories()
    {
        string rootCacheLocation = Path.Combine(Application.persistentDataPath, "Cache");

        appCacheLocation = Path.Combine(rootCacheLocation, "App");

        string scenesCacheLocation = Path.Combine(rootCacheLocation, "Scenes");
        string sceneCacheLocation = Path.Combine(scenesCacheLocation, GetInstance().gameObject.scene.name);

        standardSceneCacheLocation = Path.Combine(sceneCacheLocation, "Standard");
        permanentSceneCacheLocation = Path.Combine(sceneCacheLocation, "Permanent");

        standardConfigCacheLocation = Path.Combine(standardSceneCacheLocation, "Config");
        standardImagesCacheLocation = Path.Combine(standardSceneCacheLocation, "Images");

        imagesNewsCacheLocation = Path.Combine(standardImagesCacheLocation, "News");
        imagesInnovationsCacheLocation = Path.Combine(standardImagesCacheLocation, "Innovations");
        imagesEventsCacheLocation = Path.Combine(standardImagesCacheLocation, "Events");

        CreateDirectory(rootCacheLocation);
        CreateDirectory(appCacheLocation);
        CreateDirectory(scenesCacheLocation);

        CreateDirectory(sceneCacheLocation);
        CreateDirectory(standardSceneCacheLocation);
        CreateDirectory(permanentSceneCacheLocation);

        CreateDirectory(standardConfigCacheLocation);

        CreateDirectory(standardImagesCacheLocation);
        CreateDirectory(imagesNewsCacheLocation);
        CreateDirectory(imagesInnovationsCacheLocation);
        CreateDirectory(imagesEventsCacheLocation);

    }

    public static void ClearAllCache()
    {
        RemoveDirectory(standardSceneCacheLocation);

        // recreate the empty Cache Directory
        InitDirectories();

        // retrigger loading of elements
        LoadNormalCaches();

        GetInstance().StartCoroutine(GetInstance().CacheIsLoaded());

        GlobalController.LogMe("Normal caches successfully loaded from location: " + Application.persistentDataPath);
    }

    private void LoadAllCaches()
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

        // fires the event to notify that cache has been loaded
        if (OnCacheLoaded != null)
            OnCacheLoaded(this, EventArgs.Empty);

        isCacheLoaded = true;

    }

    private static void LoadAppCaches()
    {
        LoadConfigFromDisk(GetInstance().appConfig);
    }

    private static void LoadPermanentCaches()
    {
        LoadConfigFromDisk(GetInstance().sceneConfig);
        LoadConfigFromDisk(GetInstance().myEventsIDsConfig);
        LoadConfigFromDisk(GetInstance().creaTechsConfig);
        LoadConfigFromDisk(GetInstance().userConfig, () => {
            GetInstance().userConfig.Save(BaseConfig.SaveType.NoEvent); // if no cache found, we create a default config, so that state of the app always starts as if guest mode
        });
    }

    private static void LoadNormalCaches()
    {
        // load all caches        
        LoadConfigFromDisk(GetInstance().newsConfig);
        LoadConfigFromDisk(GetInstance().innovationsConfig);
        LoadConfigFromDisk(GetInstance().eventsConfig);
        LoadConfigFromDisk(GetInstance().servicesConfig);
        LoadConfigFromDisk(GetInstance().tagsConfig);
        LoadConfigFromDisk(GetInstance().surveyRelationalsConfig);
        LoadConfigFromDisk(GetInstance().contactsConfig);
        LoadConfigFromDisk(GetInstance().providersConfig);

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
        else if (imageURL != "") // there is an image on the server
        {
            // it is not yet in the cache, we call the API
            APIController.GetInstance().GetImage(imageURL, (Texture2D texture) =>
            {
                File.WriteAllBytes(imagePath, texture.EncodeToPNG());

                callback?.Invoke(texture);

            }, (RequestException requestException) => {

                // we return the placeholder if there is an error
                callback?.Invoke(UIController.GetInstance().imagePlaceholder);
            });

        }
        else
        {
            // we return the placeholder
            callback?.Invoke(UIController.GetInstance().imagePlaceholder);
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

    private static void CreateDirectory(string directoryPath)
    {

        if (Directory.Exists(Path.GetDirectoryName(directoryPath)))
        {
            Directory.CreateDirectory(directoryPath);

        }
    }

    private static void RemoveDirectory(string directoryPath)
    {

        if (Directory.Exists(Path.GetDirectoryName(directoryPath)))
        {
            Directory.Delete(directoryPath, true);

        }
    }
}
