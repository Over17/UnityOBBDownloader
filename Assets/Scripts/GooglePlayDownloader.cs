using UnityEngine;
using System;

public interface IGooglePlayObbDownloader
{
    string PublicKey { get; set; }

    string GetExpansionFilePath();
    string GetMainOBBPath();
    string GetPatchOBBPath();
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

        if (!IsDownloaderAvailable())
            return null;

        m_Instance = new GooglePlayObbDownloader();
        return m_Instance;
    }

    public static bool IsDownloaderAvailable()
    {
        return m_AndroidOSBuildClass.GetRawClass() != IntPtr.Zero;
    }
}
