# UnityOBBDownloader
This package is an adaption of the Google Play market_downloader library, for use with Unity Android (as a plugin).

This plugin does NOT solve splitting up a >100MB .apk into .obb (through asset bundles or similar techniques).
It merely handles the downloading of .obb files attached to a published .apk, on devices that don't support automatic downloading.

This software is free and published as is, with no warranty and responsibilities - use it at your own risk.

## Usage
This plugin is published as an AAR, and is compatible with Unity 5+. Minimum Android version supported is Honeycomb (API level 11). For a version compatible with Unity 4 or with older Android devices, please checkout changeset tagged `1.0`.
The C# API of the plugin is available in `IGooglePlayObbDownloader` interface. Use `GooglePlayObbDownloadManager.GetGooglePlayObbDownloader()` to obtain an instance.

0.	Add the plugin to your project. You need the AAR and the C# scripts (`Assets/Plugins/Android/unityOBBDownloader.aar`, `Assets/Scripts/GooglePlayDownloader.cs` and `Assets/Scripts/GooglePlayDownloaderImpl.cs`)
1.	In your script, don't forget to set `PublicKey` to your own value. You'll see a message logged and probably a crash if you skip this step
2.	Change the Bundle Identifier / Version Code so it matches the application already available on Google Play (that has .obb files attached)
3.	Build and Run on your Android device

For a script sample, please refer to `Assets/Scripts/DownloadObbExample.cs`.

## How to Build
Run `gradlew assemble` from src/UnityAndroidPermissions/

## License
Copyright (C) 2016-2017 Yury Habets

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

## See also
For more information on using .obb files in Unity, please refer to http://docs.unity3d.com/Manual/android-OBBsupport.html and http://developer.android.com/guide/market/expansion-files.html

For more information on Unity Android plugins, please refer to http://docs.unity3d.com/Manual/PluginsForAndroid.html
