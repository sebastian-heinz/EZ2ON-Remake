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
        EZR.Master.GameResourcesFolder = EZR.Master.DEBUG ? Settings.devGameResourcesFolder : "EZRData";
        Debug.Log(string.Format("Version: {0}", EZR.Master.Version));
        Debug.Log(string.Format("devGameResourcesFolder: {0}", EZR.Master.GameResourcesFolder));
        SceneManager.LoadScene(Settings.LaunchScene);
        Debug.Log("Launch!");
    }
}
