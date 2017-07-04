UnityOBBDownloader
==================

This package is an adaption of the Google Play market_downloader library, for use with Unity Android (as a plugin).

This plugin does NOT solve splitting up a >100MB .apk into .obb (through asset bundles or similar techiniques).
It merely handles the downloading of .obb files attached to a published .apk, on devices that don't support automatic downloading.

This software is free and published as is, with no warranty and responsibilities - use it at your own risk.

To try it out
-------------
This plugin is published as an Android Library project, and is compatible with Unity 4.5 and Unity 5.

1.	Open Unity, create a new project.
2.	Add this package
3.	Attach the DownloadObbExample.cs to the Main Camera
4.	Open GooglePlayDownloader.cs and replace the PUBLIC_KEY with your value
5.	Change the Bundle Identifier / Version Code so it matches an application already available on Google Play (that has .obb files attached).
6.	Build and Run on your android device.

To rebuild the code
-------------------
1.	Make sure you have the JDK and Ant installed
2.	Open a Terminal.app / cmd.exe window
3.	Change directory to the plugin root

	$> cd {your new project}/Assets/Plugins/Android/UnityOBBDownloader
	
4.	Update the ant project with the local SDK path

	$> android update project -p .

5.	Build the plugin (use 'help' to see a complete list of commands)

	$> ant build


License
-------
Copyright (C) 2016 Yury Habets

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

See also
-------- 
For more information on using .obb files in Unity, please refer to http://docs.unity3d.com/Manual/android-OBBsupport.html and http://developer.android.com/guide/market/expansion-files.html

For more information on Unity Android plugins, please refer to http://docs.unity3d.com/Manual/PluginsForAndroid.html

For more information on Android Library projects, please refer to http://developer.android.com/tools/projects/index.html#LibraryProjects
