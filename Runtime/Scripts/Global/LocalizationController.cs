using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LocalizationController : MonoBehaviour
{

    /// <summary>
    /// BUG WITH LOCALIZATION PACKAGE FROM UNITY
    /// When run on Mobile devices, it crashes. We need to do the following on each machine in Unity:
    /// Build in Addressables Menus (Window -> Asset Management -> Addressables -> Groups. Then Build -> New Build -> Default Build Script from the top bar), and it worked in Run-Time.
    /// Ref: https://forum.unity.com/threads/addressables-and-code-stripping-il2cpp.700883/
    /// </summary>
    private static readonly string MAIN_TABLE = "Main";

    // to define when script is loaded for the first time
    private bool isOnEnableCalledFirstTime = true;

    private void OnEnable()
    {
        if (isOnEnableCalledFirstTime)
        {
            isOnEnableCalledFirstTime = false;
            CacheController.OnCacheLoaded += OnCacheLoadedCallback;
        }
    }

    private void OnDestroy()
    {
        CacheController.OnCacheLoaded -= OnCacheLoadedCallback;
    }

    private void OnCacheLoadedCallback(object sender, EventArgs e)
    {
        if(CacheController.GetInstance().appConfig.lang != "")
        {
            StartCoroutine(UpdateLocale());
        }
        else
        {
            CacheController.GetInstance().appConfig.lang = LocalizationSettings.SelectedLocale.Identifier.Code;
            CacheController.GetInstance().appConfig.Save();
        }
    }

    public static IEnumerator UpdateLocale()
    {
        yield return LocalizationSettings.InitializationOperation;
         
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(CacheController.GetInstance().appConfig.lang);
        
    }

    static public void GetLocalization(string localizationKey, Action<string> callback)
    {
        AsyncOperationHandle operation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(MAIN_TABLE, localizationKey);
        operation.Completed += (operationRes) => callback?.Invoke((string)operationRes.Result);
    }


}
