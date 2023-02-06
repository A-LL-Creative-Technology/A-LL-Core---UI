# Core package of A-LL Creative Technology mobile Unity Framework

## Installation

To use this package in a unity project :

1. Install the [Unity-UI-Extensions](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Home) package either in the UI (i) or by editing a file (ii):
    1. In Unity, open the menu Edit > Project Settings... and select Package Manager in the window. In Scoped Registries, add an entry with Name: `npmjs`, URL: `https://registry.npmjs.org`, Scope(s): `com.unity.uiextensions` and click Save. Then open the menu Window > Project Settings, select Packages: My Registries in the dropdown menu, and install Unity UI Extensions.
    2. Alternatively, edit the `manifest.json` file located at \<your unity project\>/Packages and add the following lines after the dependencies :
      ```json
        "scopedRegistries": [
          {
            "name": "npmjs",
            "url": "https://registry.npmjs.org/",
            "scopes": [
              "com.unity.uiextensions"
            ]
          }
        ],
      ```
      and add this dependency in the dependencies list: 
      ```json
        "dependencies": {
          "com.unity.uiextensions": "2.2.4",
      ```

2. In Unity, open Window > Package Manager and "Add Package from git url ..." and insert this URL `https://github.com/A-LL-Creative-Technology/A-LL-Core---UI.git`
4. Add the following third-party packages from the Package Manager:
    1. Add the free packages to your assets from the [Unity asset store](https://assetstore.unity.com/) and ask Laurent or Leo to give you access to the paid packages in Unity.
    2. Select "My Assets" in the Package Manager to display free and paid Packages from the Asset Store.
    3. Select and import all these packages:
        - [Native Gallery for Android & iOS](https://assetstore.unity.com/packages/tools/integration/native-gallery-for-android-ios-112630) (Free)
        - [Procedural UI Image](https://assetstore.unity.com/packages/tools/gui/procedural-ui-image-52200) (Paid)
        - [Lean Touch](https://assetstore.unity.com/packages/tools/input-management/lean-touch-30111) (Free)
            1. Open the file "Plugins/CW/LeanTouch/Extras/Scripts/LeanScreenDepth.cs" and comment line 222 `Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", gameObject);` or change it to a `Debug.LogWarning`.
        - [Online Maps](https://assetstore.unity.com/packages/tools/integration/online-maps-v3-138509) (Paid)
        - [Advanced Input Field 2](https://assetstore.unity.com/packages/tools/gui/advanced-input-field-2-185464) (Free)
        - Nice Vibrations by Lofelt | HD Haptic Feedback for Mobile and Gamepads (Paid and deprecated)
5. Rename scene to **Main** and remove **Camera** and **Directional Ligh** in scene.
6. Create **A-LL** folder at the root of **Assets** folder and **A-LL/Scenes**. 
7. Move the scene **Main** into **A-LL/Scenes**.
8. Open Unity > Preferences > External Tools and tick the boxes for the items below.
    1. Registry packages
    2. Git packages
    3. Built-in packages
    4. Local tarball
    5. Packages from unknown sources
    6. Player projects
    Then click on "Regenerate project files" button right below. 
9. Update Project Settings for iOS + Android based on another recent working project or the below screenshots:
    1. iOS
    https://drive.google.com/drive/folders/14ODdUNbfT1asG4tnpNG5yAevRvAZKrGQ?usp=sharing
    2. Android
    https://drive.google.com/drive/folders/1tF_sY-Bdap5m67DyTJhtdS3boWUpBB0E?usp=sharing
10. Add Localization package
    Ref: To add the [Localization Unity Package](https://docs.unity3d.com/Packages/com.unity.localization@0.9/manual/Installation.html) :
    1. Go to **Edit > Project Settings > Localization** and click **Create**.
    2. Create **Main** asset file in **A-LL/Config/Localization**.
    3. In **Edit > Project Settings > Localization**, click **Locale Generator**.
    4. Select your application languages and save them in **A-LL/Config/Localization**
    5. Add the default locale in **Locale Selectors > Specific Locale Selector > Locale Id**.
    6. Open **Window > Asset Management > Localization Tables**.
    7. In the Asset Tables window, select the **New Table Collection** tab. Select which Locales you want to generate tables for, give the table the name "Main Table" and create the corresponding Asset Table Collection. Save it to **A-LL/Config/Localization**.
    8. In Unity, go to "Window -> Asset Management -> Adressables -> Groups" and then select "Build -> New Build -> Default Build Script" to build the Localization file
11. Switch platform to iOS/Android in build setttings
12. Create AppNavigationController.cs based on another recent projet (or follow the link below) and its corresponding currentView GameObject to start an initial page.
    1. AppNavigationController.cs
    https://drive.google.com/drive/folders/1sG32pHv0QT01bMdR1TDCsSritsAa7waY?usp=sharing
13. Got to "File -> Build & Run" to to test the installation.
