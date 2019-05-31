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
        EZR.Master.MessageBox = Settings.MessageBox;
        string songListData;
        try
        {
            songListData = File.ReadAllText(Path.Combine(EZR.Master.GameResourcesFolder, "SongsList.json"), Encoding.UTF8);
        }
        catch
        {
            songListData = null;
        }
        EZR.SongsList.Parse(songListData);
        Debug.Log(string.Format("Version: {0}", EZR.Master.Version));
        Debug.Log(string.Format("devGameResourcesFolder: {0}", EZR.Master.GameResourcesFolder));
        Debug.Log(string.Format("TimePrecision: {0} ms", EZR.Master.TimePrecision));
        Debug.Log("Load user data...");
        EZR.UserSaveData.LoadSave();

        var option = EZR.UserSaveData.GetOption();
        // 设置画面模式
        if (option.VSync) QualitySettings.vSyncCount = 1;
        else QualitySettings.vSyncCount = 0;
        if (option.LimitFPS)
            Application.targetFrameRate = option.TargetFrameRate;
        else
            Application.targetFrameRate = 0;
        if (option.Resolution.width != Screen.currentResolution.width ||
        option.Resolution.height != Screen.currentResolution.height ||
        option.FullScreenMode != Screen.fullScreenMode)
            Screen.SetResolution(option.Resolution.width, option.Resolution.height, option.FullScreenMode);
        // 设置时间粒度
        EZR.Master.TimePrecision = option.TimePrecision;

        SceneManager.LoadScene(Settings.LaunchScene);
        Debug.Log("Launch!");
    }
}
