using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;

public class Launcher : MonoBehaviour
{
    public EZR.SettingsAsset Settings;
    void Start()
    {
        EZR.Master.Version = Settings.Version;
        EZR.Master.GameResourcesFolder = EZR.Master.IsDebug ? Settings.devGameResourcesFolder : "EZRData";
        EZR.Master.TimePrecision = Settings.TimePrecision;
        EZR.Master.MessageBox = Settings.MessageBox;
        var songListData = File.ReadAllText(Path.Combine(EZR.Master.GameResourcesFolder, "SongsList.json"), Encoding.UTF8);
        EZR.SongsList.Parse(songListData);
        Debug.Log(string.Format("Version: {0}", EZR.Master.Version));
        Debug.Log(string.Format("devGameResourcesFolder: {0}", EZR.Master.GameResourcesFolder));
        Debug.Log(string.Format("TimePrecision: {0} ms", EZR.Master.TimePrecision));
        SceneManager.LoadScene(Settings.LaunchScene);
        Debug.Log("Launch!");
    }
}
