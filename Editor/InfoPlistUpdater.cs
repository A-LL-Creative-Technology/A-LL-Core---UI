#if UNITY_IOS
using UnityEditor;
using UnityEditor.Build;
using System.IO;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;


// adds an ATS exception domain to the Info.plist
public class InfoPlistUpdater : IPostprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPostprocessBuild(BuildReport report)
    {
        BuildTarget buildTarget = report.summary.platform;
        string pathToBuiltProject = report.summary.outputPath;

        if (buildTarget == BuildTarget.iOS)
        {
            // Get plist
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            PlistElementDict allowsDict = plist.root.CreateDict("NSAppTransportSecurity");

            allowsDict.SetBoolean("NSAllowsArbitraryLoads", true);

            PlistElementDict exceptionsDict = allowsDict.CreateDict("NSExceptionDomains");

            PlistElementDict domainDictStaging = exceptionsDict.CreateDict("staging.ccif.ch");
            domainDictStaging.SetBoolean("NSExceptionAllowsInsecureHTTPLoads", true);
            domainDictStaging.SetBoolean("NSIncludesSubdomains", true);

            PlistElementDict domainDictProduction = exceptionsDict.CreateDict("ccif.ch");
            domainDictProduction.SetBoolean("NSExceptionAllowsInsecureHTTPLoads", true);
            domainDictProduction.SetBoolean("NSIncludesSubdomains", true);

            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}
#endif