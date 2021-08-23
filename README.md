# Core package of A-LL Creative Technology mobile Unity Framework

## Installation

To use this package in a unity project :

1. Clone the repository in local directory.
2. Edit the manifest.json file located at <your unity project>/Packages and add the following lines after the dependencies :
```json
"scopedRegistries": [
  {
    "name": "Game Package Registry by Google",
    "url": "https://unityregistry-pa.googleapis.com",
    "scopes": [
      "com.google"
    ]
  },
  {
    "name": "npmjs",
    "url": "https://registry.npmjs.org/",
    "scopes": [
      "com.unity.uiextensions"
    ]
  }
]
```
3. In Unity, open Window > Package Manager and "Add Package from git url ..." and insert this URL https://laurent-all@bitbucket.org/a-lltech/a-ll-core.git.
4. In Unity, open Window > Package Manager and "Add Package from git url ..." and insert this URL com.unity.localization.
5. Add the following third-party packages from the Package Manager
    1. Connect using the lab@a-ll.tech account in Unity
    2. Select "My Assets" in the Package Manager to display paid Packages from the Asset Store
    3. Select and import all these packages
        - Native Gallery for Android & iOS
        - Procedural UI Image
        - Nice Vibrations | Haptic Feedback for Mobile & Gamepads
            1. Remove platform "Editor" from inspector in the Include Platforms of the assembly file "Newtonsoft.Json.dll" in "ThirdParty/Newtonsoft"
        - Lean Touch
            1. Open the file "Lean/Touch/Extras/LeanScreenDepth.cs" and comment line 221 "Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", gameObject);"
        - Rest Client for Unity 
            1. Additionnaly, create an Assembly Definition Reference named "Main Proyecto26" in "RestClient/Packages"
            2. Link it to Assembly A-LL.Core.Runtime.asmdef
        - Online Maps
        - Advanced Input Field 2
6. Copy/Paste files in "A-LL Core/Assets To Copy/A-LL/Config/Firebase/" and paste them to "Assets/A-LL/Config/Firebase/" and rename by removing the prefix "Sandbox - "
7. Open Unity > Preferences > External Tools and tick the boxes for the items below.
    1. Registry packages
    2. Git packages
    3. Built-in packages
    4. Local tarball
    5. Packages from unknown sources
    6. Player projects
    Then click on "Regenerate project files" button right below. 
8. Rename scene to **Main** and remove **Camera** and **Directional Ligh** in scene.
9. Create **A-LL** folder at the root of **Assets** folder and **A-LL/Scenes**. 
10. Move the scene **Main** into **A-LL/Scenes**.
11. Update Project Settings for iOS + Android based on another recent working project.
12. Add Localization package
    Ref: To add the [Localization Unity Package](https://docs.unity3d.com/Packages/com.unity.localization@0.9/manual/Installation.html) :
    1. Go to **Edit > Project Settings > Localization** and click **Create**.
    2. Create **Main** asset file in **A-LL/Config/Localization**.
    3. In **Edit > Project Settings > Localization**, click **Locale Generator**.
    4. Select your application languages and save them in **A-LL/Config/Localization**
    5. Add the default locale in **Locale Selectors > Specific Locale Selector > Locale Id**.
    6. Open **Window > Asset Management > Localization Tables**.
    7. In the Asset Tables window, select the **New Table Collection** tab. Select which Locales you want to generate tables for, give the table the name "Main Table" and create the corresponding Asset Table Collection. Save it to **A-LL/Config/Localization**.
    8. In Unity, go to "Window -> Asset Management -> Adressables -> Groups" and then select "Build -> New Build -> Default Build Script" to build the Localization file
13. Switch platform to iOS/Android in build setttings
14. Create AppNavigationController.cs based on another recent projet and its corresponding currentView GameObject to start an initial page.
15. Got to "File -> Build & Run" to to test the installation.
