using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class GooglePlayDownloader
{
#if UNITY_ANDROID && !UNITY_EDITOR
	private static string PublicKey = "REPLACE THIS WITH YOUR PUBLIC KEY";
	private static AndroidJavaClass AndroidOSBuildClass = new AndroidJavaClass("android.os.Build");
	private static AndroidJavaClass EnvironmentClass = new AndroidJavaClass("android.os.Environment");
	private const string Environment_MediaMounted = "mounted";

	public static void SetPublicKey(string newKey)
	{
		PublicKey = newKey;
	}

	public static bool RunningOnAndroid()
	{
		return AndroidOSBuildClass.GetRawClass() != IntPtr.Zero;
	}

	static GooglePlayDownloader()
	{
		if (!RunningOnAndroid())
			return;

		using (AndroidJavaClass downloaderServiceClass = new AndroidJavaClass("com.unity3d.plugin.downloader.UnityDownloaderService"))
		{
			downloaderServiceClass.SetStatic("BASE64_PUBLIC_KEY", PublicKey);
			// Used by the preference obfuscater
			downloaderServiceClass.SetStatic("SALT", new byte[]{1, 43, 256-12, 256-1, 54, 98, 256-100, 256-12, 43, 2, 256-8, 256-4, 9, 5, 256-106, 256-108, 256-33, 45, 256-1, 84});
		}
	}
	
	public static string GetExpansionFilePath()
	{
		PopulateOBBData();

		if (EnvironmentClass.CallStatic<string>("getExternalStorageState") != Environment_MediaMounted)
			return null;
			
		const string obbPath = "Android/obb";

		using (AndroidJavaObject externalStorageDirectory = EnvironmentClass.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
		{
			string root = externalStorageDirectory.Call<string>("getPath");
			return String.Format("{0}/{1}/{2}", root, obbPath, ObbPackage);
		}
	}

	public static string GetMainOBBPath(string expansionFilePath)
	{
		return GetOBBPackagePath(expansionFilePath, "main");
	}

	public static string GetPatchOBBPath(string expansionFilePath)
	{
		return GetOBBPackagePath(expansionFilePath, "patch");
	}

	private static string GetOBBPackagePath(string expansionFilePath, string prefix)
	{
		if (expansionFilePath == null)
			return null;

		PopulateOBBData();
		string filePath = String.Format("{0}/{1}.{2}.{3}.obb", expansionFilePath, prefix, ObbVersion, ObbPackage);
		return File.Exists(filePath) ? filePath : null;
	}

	public static void FetchOBB()
	{
		using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent",
															currentActivity,
															new AndroidJavaClass("com.unity3d.plugin.downloader.UnityDownloaderActivity"));
	
			const int Intent_FLAG_ACTIVITY_NO_ANIMATION = 0x10000;
			intent.Call<AndroidJavaObject>("addFlags", Intent_FLAG_ACTIVITY_NO_ANIMATION);
			intent.Call<AndroidJavaObject>("putExtra", "unityplayer.Activity",
														currentActivity.Call<AndroidJavaObject>("getClass").Call<string>("getName"));
			currentActivity.Call("startActivity", intent);
	
			if (AndroidJNI.ExceptionOccurred() != System.IntPtr.Zero)
			{
				Debug.LogError("Exception occurred while attempting to start DownloaderActivity - is the AndroidManifest.xml incorrect?");
				AndroidJNI.ExceptionDescribe();
				AndroidJNI.ExceptionClear();
			}
		}
	}
	
	// This code will reuse the package version from the .apk when looking for the .obb
	// Modify as appropriate
	private static string ObbPackage;
	private static int ObbVersion = 0;
	private static void PopulateOBBData()
	{
		if (ObbVersion != 0)
			return;
		using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
			ObbPackage = currentActivity.Call<string>("getPackageName");
			AndroidJavaObject packageInfo = currentActivity.Call<AndroidJavaObject>("getPackageManager").Call<AndroidJavaObject>("getPackageInfo", ObbPackage, 0);
			ObbVersion = packageInfo.Get<int>("versionCode");
		}
	}
#endif
}
