using UnityEngine;
using System.Collections;

public class DownloadObbExample : MonoBehaviour
{
    private IGooglePlayObbDownloader m_obbDownloader;
    void Start()
    {
        m_obbDownloader = GooglePlayObbDownloadManager.GetGooglePlayObbDownloader();
    }	

	void OnGUI()
	{
		if (!GooglePlayObbDownloadManager.IsRunningOnAndroid())
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
			string mainPath = m_obbDownloader.GetMainOBBPath(expPath);
			string patchPath = m_obbDownloader.GetPatchOBBPath(expPath);
			
			GUI.Label(new Rect(10, 10, Screen.width-10, 20), "Main = ..."  + ( mainPath == null ? " NOT AVAILABLE" :  mainPath.Substring(expPath.Length)));
			GUI.Label(new Rect(10, 25, Screen.width-10, 20), "Patch = ..." + (patchPath == null ? " NOT AVAILABLE" : patchPath.Substring(expPath.Length)));
			if (mainPath == null || patchPath == null)
				if (GUI.Button(new Rect(10, 100, 100, 100), "Fetch OBBs"))
                    m_obbDownloader.FetchOBB();
		}

	}
}
