#if UNITY_IOS
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class iOSAppStoreLocalization
{
    // set your languages here
    static string[] localizations = new string[] { "en" }; 

    [PostProcessBuild(int.MaxValue)]
    static void OnBuildDone(BuildTarget target, string pathToBuiltProject)
    {
        ModifyInfoPList(pathToBuiltProject);
    }

    static void ModifyInfoPList(string pathToBuiltProject)
    {
        string path = pathToBuiltProject + "/info.plist";
        var plist = new PlistDocument();
        plist.ReadFromFile(path);
        var root = plist.root;
        CFBundleLocalizations(root);
        plist.WriteToFile(path);
    }

    static void CFBundleLocalizations(PlistElementDict root)
    {
        var rootDic = root.values;
        
        var array = root.CreateArray("CFBundleLocalizations");
        foreach (var localization in localizations) array.AddString(localization);
    }
}
#endif