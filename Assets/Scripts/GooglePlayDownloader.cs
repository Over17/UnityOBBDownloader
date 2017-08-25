using UnityEngine;
using System.IO;
using System;

public interface IGooglePlayObbDownloader
{
    string PublicKey { get; set; }

    string GetExpansionFilePath();
    string GetMainOBBPath(string expansionFilePath);
    string GetPatchOBBPath(string expansionFilePath);
    void FetchOBB();
}

public class GooglePlayObbDownloadManager
{
    private static AndroidJavaClass m_AndroidOSBuildClass = new AndroidJavaClass("android.os.Build");
    private static IGooglePlayObbDownloader m_Instance;

    public static IGooglePlayObbDownloader GetGooglePlayObbDownloader()
    {
        if (m_Instance != null)
            return m_Instance;

        if (!IsRunningOnAndroid())
            return null;

        m_Instance = new GooglePlayObbDownloader();
        return m_Instance;
    }

    public static bool IsRunningOnAndroid()
    {
        return m_AndroidOSBuildClass.GetRawClass() != IntPtr.Zero;
    }


}

internal class GooglePlayObbDownloader : IGooglePlayObbDownloader
{
    private static AndroidJavaClass EnvironmentClass = new AndroidJavaClass("android.os.Environment");
    private const string Environment_MediaMounted = "mounted";

    public string PublicKey { get; set; }

    internal GooglePlayObbDownloader()
    {
        using (var downloaderServiceClass = new AndroidJavaClass("com.unity3d.plugin.downloader.UnityDownloaderService"))
        {
            downloaderServiceClass.SetStatic("BASE64_PUBLIC_KEY", PublicKey);
            // Used by the preference obfuscator
            downloaderServiceClass.SetStatic("SALT", new byte[] { 1, 43, 256 - 12, 256 - 1, 54, 98, 256 - 100, 256 - 12, 43, 2, 256 - 8, 256 - 4, 9, 5, 256 - 106, 256 - 108, 256 - 33, 45, 256 - 1, 84 });
        }
    }

    public void FetchOBB()
    {
        using (var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            var intent = new AndroidJavaObject("android.content.Intent",
                                                currentActivity,
                                                new AndroidJavaClass("com.unity3d.plugin.downloader.UnityDownloaderActivity"));

            const int Intent_FLAG_ACTIVITY_NO_ANIMATION = 0x10000;
            intent.Call<AndroidJavaObject>("addFlags", Intent_FLAG_ACTIVITY_NO_ANIMATION);
            intent.Call<AndroidJavaObject>("putExtra", "unityplayer.Activity",
                                                        currentActivity.Call<AndroidJavaObject>("getClass").Call<string>("getName"));
            try
            {
                currentActivity.Call("startActivity", intent);
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception occurred while attempting to start DownloaderActivity - is the AndroidManifest.xml incorrect?\n" + ex.Message);
            }
        }
    }

    public string GetExpansionFilePath()
    {
        if (EnvironmentClass.CallStatic<string>("getExternalStorageState") != Environment_MediaMounted)
            return null;

        const string obbPath = "Android/obb";
        using (var externalStorageDirectory = EnvironmentClass.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
        {
            var externalRoot = externalStorageDirectory.Call<string>("getPath");
            return string.Format("{0}/{1}/{2}", externalRoot, obbPath, ObbPackage);
        }
    }

    public string GetMainOBBPath(string expansionFilePath)
    {
        return GetOBBPackagePath(expansionFilePath, "main");
    }

    public string GetPatchOBBPath(string expansionFilePath)
    {
        return GetOBBPackagePath(expansionFilePath, "patch");
    }

    private static string GetOBBPackagePath(string expansionFilePath, string prefix)
    {
        if (expansionFilePath == null)
            return null;

        string filePath = string.Format("{0}/{1}.{2}.{3}.obb", expansionFilePath, prefix, ObbVersion, ObbPackage);
        return File.Exists(filePath) ? filePath : null;
    }

    private static string m_ObbPackage;
    private static string ObbPackage
    {
        get
        {
            if (m_ObbPackage == null)
            {
                PopulateOBBProperties();
            }
            return m_ObbPackage;
        }
    }

    private static int m_ObbVersion;
    private static int ObbVersion
    {
        get
        {
            if (m_ObbVersion == 0)
            {
                PopulateOBBProperties();
            }
            return m_ObbVersion;
        }
    }

    // This code will reuse the package version from the .apk when looking for the .obb
    // Modify as appropriate
    private static void PopulateOBBProperties()
    {
        using (var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            m_ObbPackage = currentActivity.Call<string>("getPackageName");
            var packageInfo = currentActivity.Call<AndroidJavaObject>("getPackageManager").Call<AndroidJavaObject>("getPackageInfo", ObbPackage, 0);
            m_ObbVersion = packageInfo.Get<int>("versionCode");
        }
    }
}
