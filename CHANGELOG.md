# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## [2.0.2] - 2023-03-02

### Added

- N/A

### Changed

- Update localization dependencies
- Update install instructions in README

### Fixed

- Revert NiceVibration from 3.9 to 4.1

## [1.0.1] - 19.07.2021 - [Unreleased]

### Added

- post process script for changing the localization of the app on the App Store (cf. iOSAppStoreLocalization.cs)
- added callback function to OnPopUpClose()
- added optional field option in Input Field
- added features for async API calls in APIController

## [1.0.0] - 03.05.2021 - 12.07.2021

### Added

- Config script on the A-LL Core prefab to activate/deactivate computation on the safe area
- Splash screen prefabs
- Added Editor scripts to manage scene views
- Safe Area supports in over views (needs to use dedicated prefab) and updated in UIController for normal views
- Added function to convert phone number to international format

### Changed

- Support for UI Rounded Corners using masks. It is replaced by Procedural UI Image.
- Support for SVG and menu controller updated accordingly.
- Removed static on APIController + server url is editable.
- Support for defining number of seconds before autoHide (default was 5 seconds)
- Refactor the cache in Core Cache and App Cache. See the integration example for CCIF in A-LL Core Integration.
- Changed loader spritesheet color (needs to be tinted)
- Cleaned prefab popups
- Added support for URLs with accents. The Core removes the accents as it crashed on iOS/Mac OS.
- Changed the eyeIcon logic (no backward compatibility!)
- Added a safeguard to send API requests only when the Core is ready
- Added an option for custom API Token and Expiration Date in APIController and UserController
- Added feature to remove automatic error notification in APIController
- Hide bottom navigation in single pages
- Made the option for the transparent transparent 
- Updated URL format function to make sure urls start with "http"
- Moved elements from UserController to APIController and refactored the cache (UserConfig => APIConfig)
