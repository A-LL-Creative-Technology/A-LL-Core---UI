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
Your manifest.json should look like this :
```json
{
  "dependencies": {
    	...
  },
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
}
```
3. In Unity, open Window > Package Manager and "Add Package from git url ..." and insert this URL https://laurent-all@bitbucket.org/a-lltech/a-ll-core.git.
4. Add the following third-party packages from the Package Manager
    1. Connect using the lab@a-ll.tech account in Unity
    2. Select "My Assets" in the Package Manager to display paid Packages from the Asset Store
    3. Select and import all these packages
        - Nice Vibrations | Haptic Feedback for Mobile & Gamepads
            1. Rename file "Newtonsoft.Json.dll" to "Dupplicate - Newtonsoft.Json.dll" in "ThirdParty/Newtonsoft"
        - Lean Touch
            1. Open the file "Assets/Lean/Touch/Extras/LeanScreenDepth.cs" and comment line 221 "Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", gameObject);"
        - Rest Client for Unity 
            1. Additionnaly, create an Assembly Definition Reference named "Main Proyecto26" in "RestClient/Packages"
            2. Link it to Assembly A-LL.Core.Runtime.asmdef
        - Online Maps
5. Copy/Paste files in "A-LL Core/Assets To Copy/A-LL/Config/Firebase/" and paste them to "Assets/A-LL/Config/Firebase/" and rename by removing the prefix "Sandbox - "
6. In Unity, go to "Window -> Asset Management -> Adressables -> Groups" and then select "Build -> New Build -> Default Build Script" to build the Localization file
7. Open Unity > Preferences > External Tools and tick the boxes for the items below.
    1. Registry packages
    2. Git packages
    3. Built-in packages
    4. Local tarball
    5. Packages from unknown sources
    6. Player projects
    Then click on "Regenerate project files" button right below. 

## A-LL Project Setup

To set up the project structure for A-LL in a new project :

1. Rename scene to **Main**.
2. Remove **Camera** in scene.
3. Create **A-LL** folder at the root of **Assets** folder.
4. Create folder **A-LL/Scenes** and move **Main** into this folder.
5. Update Project Settings for iOS + Android based on another recent working project.
6. Add Localization package (see "Add Localization").
    Ref: To add the [Localization Unity Package](https://docs.unity3d.com/Packages/com.unity.localization@0.9/manual/Installation.html) :
    1. Open the **Window > Package Manager** and **Add package from git URL ...**
    2. Type in **com.unity.localization**.
    3. Go to **Edit > Project Settings > Localization** and click **Create**.
    4. Create **Main Localization** asset file in **A-LL/Config/Localization**.
    5. In **Localization Settings** click **Locale Generator**.
    6. Select your application languages and save them in **A-LL/Config/Localization**
    7. Add the default locale in **Locale Selectors > Element 2 > Locale Id**.
    8. Open **Window > Asset Management > Localization Tables**.
    9. In the Asset Tables window, select the **New Table** tab. Select which Locales you want to generate tables for, give the table a name and select the **Asset Table** table type.
7. Switch platform to iOS/Android in build setttings
