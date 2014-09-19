UnityOBBDownloader
==================

This package is an adaption of the Google Play market_downloader library, for use with Unity Android (as a plugin).

This plugin does NOT solve splitting up a >50MB .apk into .obb (through asset bundles or similar techiniques).
It merely handles the downloading of .obb files attached to a published .apk, on devices that don't support automatic downloading.

This software is free and published as is, with no warranty and responsibilities - use it at your own risk.

To try it out
-------------
This plugin is published as an Android Library project, and is compatible with Unity 4.5 and Unity 5.

1.	Open Unity, create a new project.
2.	Add this package
3.	Attach the DownloadObbExample.cs to the Main Camera
4.	Open GooglePlayDownloader.cs and replace the BASE64_PUBLIC_KEY.
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

	
See also
-------- 
For more information on using .obb files in Unity, please refer to http://docs.unity3d.com/Manual/android-OBBsupport.html and http://developer.android.com/guide/market/expansion-files.html

For more information on Unity Android plugins, please refer to http://docs.unity3d.com/Manual/PluginsForAndroid.html

For more information on Android Library projects, please refer to http://developer.android.com/tools/projects/index.html#LibraryProjects
