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
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows)
        {
            if (System.Environment.OSVersion.Version.Major <= 6 && System.Environment.OSVersion.Version.Minor <= 1)
            {
                EZR.Master.IsOldWin = true;
            }
        }
        EZR.Master.GameResourcesFolder =
         EZR.Master.IsDebug ?
         Settings.devGameResourcesFolder :
         Path.Combine(
             Application.dataPath,
             "..",
             "EZRData"
        );
        EZR.Master.MessageBox = Settings.MessageBox;
        EZR.Master.Tooltips = Instantiate(Settings.Tooltips);
        DontDestroyOnLoad(EZR.Master.Tooltips);
        DontDestroyOnLoad(GameObject.Find("PersistentCanvas"));
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

        // Direct3D11 API下解决开启垂直同步后画面延迟的问题
        QualitySettings.maxQueuedFrames = 0;

        // 读取设置
        var option = EZR.UserSaveData.GetOption();
        EZR.Option.ApplyOption(option);

        // 读取库存
        var inventory = EZR.UserSaveData.GetInventory();
        EZR.DisplayLoop.PanelResource = inventory["panelResource"];
        EZR.DisplayLoop.NoteResource = inventory["noteResource"];

        SceneManager.LoadScene(Settings.LaunchScene);
        Debug.Log("Launch!");
    }
}
