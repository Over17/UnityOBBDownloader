using UnityEngine;
using System.IO;
using System;

internal class GooglePlayObbDownloader : IGooglePlayObbDownloader
{
    private static AndroidJavaClass EnvironmentClass = new AndroidJavaClass("android.os.Environment");
    private const string Environment_MediaMounted = "mounted";

    public string PublicKey { get; set; }

    private void ApplyPublicKey()
    {
        if (string.IsNullOrEmpty(PublicKey))
        {
            Debug.LogError("GooglePlayObbDownloader: The public key is not set - did you forget to set it in the script?\n");
        }
        using (var downloaderServiceClass = new AndroidJavaClass("com.unity3d.plugin.downloader.UnityDownloaderService"))
        {
            downloaderServiceClass.SetStatic("BASE64_PUBLIC_KEY", PublicKey);
            // Used by the preference obfuscator
            downloaderServiceClass.SetStatic("SALT", new byte[] { 1, 43, 256 - 12, 256 - 1, 54, 98, 256 - 100, 256 - 12, 43, 2, 256 - 8, 256 - 4, 9, 5, 256 - 106, 256 - 108, 256 - 33, 45, 256 - 1, 84 });
        }
    }

    public void FetchOBB()
    {
        ApplyPublicKey();
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
                Debug.LogError("GooglePlayObbDownloader: Exception occurred while attempting to start DownloaderActivity - is the AndroidManifest.xml incorrect?\n" + ex.Message);
            }
        }
    }

    private string m_ExpansionFilePath;
    public string GetExpansionFilePath()
    {
        if (EnvironmentClass.CallStatic<string>("getExternalStorageState") != Environment_MediaMounted)
        {
            m_ExpansionFilePath = null;
            return m_ExpansionFilePath;
        }

        if (string.IsNullOrEmpty(m_ExpansionFilePath))
        {
            const string obbPath = "Android/obb";
            using (var externalStorageDirectory = EnvironmentClass.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
            {
                var externalRoot = externalStorageDirectory.Call<string>("getPath");
                m_ExpansionFilePath = string.Format("{0}/{1}/{2}", externalRoot, obbPath, ObbPackage);
            }
        }
        return m_ExpansionFilePath;
    }

    public string GetMainOBBPath()
    {
        return GetOBBPackagePath(GetExpansionFilePath(), "main");
    }

    public string GetPatchOBBPath()
    {
        return GetOBBPackagePath(GetExpansionFilePath(), "patch");
    }

    private static string GetOBBPackagePath(string expansionFilePath, string prefix)
    {
        if (string.IsNullOrEmpty(expansionFilePath))
            return null;

        var filePath = string.Format("{0}/{1}.{2}.{3}.obb", expansionFilePath, prefix, ObbVersion, ObbPackage);
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
            var packageInfo = currentActivity.Call<AndroidJavaObject>("getPackageManager").Call<AndroidJavaObject>("getPackageInfo", m_ObbPackage, 0);
            m_ObbVersion = packageInfo.Get<int>("versionCode");
        }
    }
}
