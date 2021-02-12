using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static CacheModel;

public class GlobalController: MonoBehaviour
{
    private static GlobalController instance;

    public static GlobalController GetInstance()
    {
        return instance;
    }

#pragma warning disable 0649

    private static readonly int THRESHOLD_TRUNCATE_TITLE = 75;

    // container for debug logs
    [SerializeField] private GameObject debugCanvas;
    [SerializeField] private TextMeshProUGUI debugLog;

#pragma warning restore 0649

    private void Awake()
    {
        instance = this;

        Application.targetFrameRate = 90;

        //DontDestroyOnLoad(gameObject);
    }

    static public void LogMe(string message)
    {
        Debug.Log(message);
    }

    static public void DialogMe(string message)
    {
#if UNITY_EDITOR
        EditorUtility.DisplayDialog("Dialog", message, "Ok");
#else
		LogMe(message);
#endif

    }

    public void DebugMe(string message)
    {
        // activate debug log only if used
        if (!debugCanvas.activeSelf)
        {
            debugCanvas.SetActive(true);
            debugLog.text = "";
        }

        debugLog.text += message + "\r\n";
    }

    public void HideDebugMe()
    {
        debugCanvas.SetActive(false);
    }


    public static BaseItem GetLatestItemInList(List<BaseItem> items)
    {
        if (items.Count > 0)
            return items[items.Count - 1];
        else
        {
            GlobalController.LogMe("The oldest item does not exist for an empty list.");
            return null;
        }
    }

    public static string TruncateTitle(string str)
    {
        if (str.Length < THRESHOLD_TRUNCATE_TITLE)
            return str;

        int i = THRESHOLD_TRUNCATE_TITLE;
        while (str[i] != ' ' && i >= 0)
            i--;

        return str.Substring(0, i) + "...";

    }

    public static string ExtractMonthInText(string dateTimeStr)
    {
        // set the date
        DateTime dateDT = DateTime.Parse(dateTimeStr);
        string monthTranslated = new CultureInfo(CacheController.globalConfig.lang).DateTimeFormat.GetMonthName(dateDT.Month);

        return monthTranslated;

    }

    public static string FormatTime(string dateTimeStr)
    {
        // set the date
        DateTime dateDT = DateTime.Parse(dateTimeStr);

        return dateDT.Hour.ToString("00") + ":" + dateDT.Minute.ToString("00");

    }

    public static bool DoesSceneExists(string sceneName)
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

    public static bool LoadNextSceneAsync(string sceneName)
    {
        if (DoesSceneExists(sceneName))
        {
            NavigationController.GetInstance().OnGlobalLoaderOpen(); // no need to cancel it as GO is destroyed with new scene

            GetInstance().StartCoroutine(LoadSceneAsync(sceneName));

            return true;
        }
        else
        {
            return false;
        }
    }

    private static IEnumerator LoadSceneAsync(string sceneName)
    {
        // TO DO: manage error when scene cannot be found/loaded

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        // Wait until the asynchronous scene fully loads
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

    }

    public static string RemoveSpaceInString(string str)
    {
        return Regex.Replace(str, @"\s+", "");
    }

    public static IEnumerator RebuildLayout(RectTransform rootRectTransform, Action callback)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rootRectTransform);

        yield return null;

        callback?.Invoke();
    }
}
