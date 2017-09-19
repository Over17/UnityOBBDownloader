using UnityEngine;
using System.Collections;

public class DownloadObbExample : MonoBehaviour
{
    private IGooglePlayObbDownloader m_obbDownloader;
    void Start()
    {
        m_obbDownloader = GooglePlayObbDownloadManager.GetGooglePlayObbDownloader();
        m_obbDownloader.PublicKey = ""; // YOUR PUBLIC KEY HERE
    }	

    void OnGUI()
    {
        if (!GooglePlayObbDownloadManager.IsDownloaderAvailable())
        {
            GUI.Label(new Rect(10, 10, Screen.width-10, 20), "Use GooglePlayDownloader only on Android device!");
            return;
        }
        
        string expPath = m_obbDownloader.GetExpansionFilePath();
        if (expPath == null)
        {
                GUI.Label(new Rect(10, 10, Screen.width-10, 20), "External storage is not available!");
        }
        else
        {
            var mainPath = m_obbDownloader.GetMainOBBPath();
            var patchPath = m_obbDownloader.GetPatchOBBPath();
            
            GUI.Label(new Rect(10, 10, Screen.width-10, 20), "Main = ..."  + ( mainPath == null ? " NOT AVAILABLE" :  mainPath.Substring(expPath.Length)));
            GUI.Label(new Rect(10, 25, Screen.width-10, 20), "Patch = ..." + (patchPath == null ? " NOT AVAILABLE" : patchPath.Substring(expPath.Length)));
            if (mainPath == null || patchPath == null)
                if (GUI.Button(new Rect(10, 100, 100, 100), "Fetch OBBs"))
                    m_obbDownloader.FetchOBB();
        }

    }
}
