# Core package of A-LL Creative Technology mobile Unity Framework

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
3. In Unity, open Window->Package Manager and "Add Package from disk ...".
4. Open the directory in which you cloned the repository, and select A-LL Core/package.json file.