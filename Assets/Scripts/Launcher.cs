using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviour
{
    public EZR.SettingsAsset Settings;
    void Start()
    {
        EZR.Master.Version = Settings.Version;
        EZR.Master.GameResourcesFolder = EZR.Master.IsDebug ? Settings.devGameResourcesFolder : "EZRData";
        EZR.Master.TimePrecision = Settings.TimePrecision;
        Debug.Log(string.Format("Version: {0}", EZR.Master.Version));
        Debug.Log(string.Format("devGameResourcesFolder: {0}", EZR.Master.GameResourcesFolder));
        Debug.Log(string.Format("TimePrecision: {0} ms", EZR.Master.TimePrecision));
        SceneManager.LoadScene(Settings.LaunchScene);
        Debug.Log("Launch!");
    }
}
