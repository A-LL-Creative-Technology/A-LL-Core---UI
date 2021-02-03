# Core package of A-LL Creative Technology mobile Unity Framework
## A-LL Setup

To set up the project structure for A-LL in a new project :

1. Rename scene to **Main**.
2. Remove **Camera** in scene.
3. Create **A-LL** folder at the root of **Assets** folder.
4. Move **Scenes** folder into **A-LL**.
5. Adapt Project Settings.
6. Add Localization package (see "Add Localization").
7. 


n. switch plateform to IOS/Android

## Add Localization

To add the [Localization Unity Package](https://docs.unity3d.com/Packages/com.unity.localization@0.9/manual/Installation.html) :

1. Open the **Window > Package Manager** and **Add package from git URL ...**
2. Type in **com.unity.localization**.
3. Go to **Edit > Project Settings > Localization** and click **Create**.
4. Create **Main Localization** asset file in **A-LL/Config/Localization**.
5. In **Localization Settings** click **Locale Generator**.
6. Select your application languages and save them in **A-LL/Config/Localization**
7. Add the default locale in **Locale Selectors > Element 2 > Locale Id**.
8. Open **Window > Asset Management > Localization Tables**.
9. In the Asset Tables window, select the **New Table** tab. Select which Locales you want to generate tables for, give the table a name and select the **Asset Table** table type.




## Installation

To use this package in a unity project :

1. clone repository in local drive.
2. Edit the manifest.json file located at <your unity project>/Packages and add the following lines after the dependecies :
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
3. In Unity, open Window > Package Manager and "Add Package from disk ...".
4. Open the directory in which you cloned the repository, and select package.json file.