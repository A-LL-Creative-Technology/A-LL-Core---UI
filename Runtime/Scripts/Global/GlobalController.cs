using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
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
        if (str.Length <= THRESHOLD_TRUNCATE_TITLE)
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
        string monthTranslated = new CultureInfo(CacheController.GetInstance().appConfig.lang).DateTimeFormat.GetMonthName(dateDT.Month);

        return monthTranslated;

    }

    public static string FormatTime(string dateTimeStr)
    {
        // set the date
        DateTime dateDT = DateTime.Parse(dateTimeStr);

        return dateDT.Hour.ToString("00") + ":" + dateDT.Minute.ToString("00");

    }

    public static string RemoveSpaceInString(string str)
    {
        return Regex.Replace(str, @"\s+", "");
    }

    public static string ReplaceMultipleSpaceInString(string str)
    {
        return Regex.Replace(str, @"\s+", " ");
    }

    public static string RemoveTabInString(string str)
    {
        return Regex.Replace(str, @"\t+", "");
    }

    public static string RemoveCarriageReturnInString(string str)
    {
        return Regex.Replace(str, @"\r+", "");
    }

    public static string RemoveNewLineInString(string str)
    {
        return Regex.Replace(str, @"\n+", "");
    }

    public static string FormatGraphQLQuery(string str)
    {
        str = ReplaceMultipleSpaceInString(str);
        str = RemoveTabInString(str);
        str = RemoveNewLineInString(str);
        str = RemoveCarriageReturnInString(str);
        return str;
    }


    public static string ConvertToStandardFormat(string text)
    {
        //Check mailto and tel
        if (text.StartsWith("tel:") || text.StartsWith("mailto:"))
            return text;
        // Add http://
        if (!text.StartsWith("http"))
            text = "http://" + text;
        // Remove Accent
        StringBuilder sbReturn = new StringBuilder();
        var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
        foreach (char letter in arrayText)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                sbReturn.Append(letter);
        }
        return sbReturn.ToString();
    }

    // check phone number format:
    // 1) If it starts with +, we leave it as is
    // 2) If it starts with 00, we convert the 00 to +
    // 3) If it start with 0, we convert the 0 to +41 or the selected country code
    public static string ConvertPhoneNumberToInternationalFormat(string phone, string countryCode = "CH")
    {

        if (phone.Length < 2)
            return phone;


        phone = phone.Replace(" ", String.Empty); //Remove spaces

        int indexOfPlus = phone.IndexOf("+");
        if(indexOfPlus != -1)
            phone = phone.Substring(indexOfPlus); // Remove everything in front of "+"

        // extract first characters
        string firstChar = phone.Substring(0, 1);
        string secondChar = phone.Substring(1, 1);

        // 1)
        if (firstChar == "+")
            return phone;

        // 2)
        if (firstChar == "0" && secondChar == "0")
            return "+" + phone.Substring(2);

        // 3)
        if (firstChar == "0" && secondChar != "0")
        {

            string countryCodeNumber = "+41";

            switch (countryCode)
            {
                case "FR":

                    countryCodeNumber = "+33";
                    break;

                case "DE":

                    countryCodeNumber = "+49";
                    break;

                case "IT":

                    countryCodeNumber = "+39";
                    break;

                case "US":

                    countryCodeNumber = "+1";
                    break;

            }

            return countryCodeNumber + phone.Substring(1);
        }


        return phone;
    }

    public static IEnumerator RebuildLayout(RectTransform rootRectTransform, Action callback)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rootRectTransform);

        yield return null;

        callback?.Invoke();
    }
}
