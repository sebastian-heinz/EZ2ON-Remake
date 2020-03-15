using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

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

        var buffer = EZR.DataLoader.LoadFile(Path.Combine(EZR.Master.GameResourcesFolder, "SongList.gpk"), "SongList.json");
        string songListData = null;
        if (buffer != null) songListData = Encoding.UTF8.GetString(buffer);
        EZR.SongList.Parse(songListData);
        Debug.Log(string.Format("Version: {0}", EZR.Master.Version));
        Debug.Log(string.Format("devGameResourcesFolder: {0}", EZR.Master.GameResourcesFolder));
        Debug.Log(string.Format("TimePrecision: {0} ms", EZR.Master.TimePrecision));
        EZR.UserSaveData.LoadData();
        Debug.Log("Load user data finish");

        // Direct3D11 API下解决开启垂直同步后画面延迟的问题
        QualitySettings.maxQueuedFrames = 0;

        // 读取设置
        var option = EZR.UserSaveData.GetOption();
        EZR.Option.ApplyOption(option);

        // 读取库存
        var inventory = EZR.UserSaveData.GetInventory();
        EZR.DisplayLoop.PanelResource = inventory["panelResource"];
        EZR.DisplayLoop.NoteResource = inventory["noteResource"];

        // 读取按键配置
        var KeyConfigPath = Path.Combine(Application.dataPath, "..", "KeyConfig.json");
        JObject jobj = new JObject();
        try
        {
            jobj = JObject.Parse(File.ReadAllText(KeyConfigPath));
        }
        catch { }
        setKeyCodeMapping(jobj);

        SceneManager.LoadScene(Settings.LaunchScene);
        Debug.Log("Launch!");
    }

    void setKeyCodeMapping(JObject jobj)
    {
        if (jobj.ContainsKey("4key") && ((JArray)jobj["4key"]).Count == 4)
        {
            for (int i = 0; i < ((JArray)jobj["4key"]).Count; i++)
            {
                EZR.Master.KeyCodeMapping[0][i] = (char)(jobj["4key"][i]);
            }
        }
        if (jobj.ContainsKey("5key") && ((JArray)jobj["5key"]).Count == 5)
        {
            for (int i = 0; i < ((JArray)jobj["5key"]).Count; i++)
            {
                EZR.Master.KeyCodeMapping[1][i] = (char)(jobj["5key"][i]);
            }
        }
        if (jobj.ContainsKey("6key") && ((JArray)jobj["6key"]).Count == 6)
        {
            for (int i = 0; i < ((JArray)jobj["6key"]).Count; i++)
            {
                EZR.Master.KeyCodeMapping[2][i] = (char)(jobj["6key"][i]);
            }
        }
        if (jobj.ContainsKey("7key") && ((JArray)jobj["7key"]).Count == 7)
        {
            for (int i = 0; i < ((JArray)jobj["7key"]).Count; i++)
            {
                EZR.Master.KeyCodeMapping[3][i] = (char)(jobj["7key"][i]);
            }
        }
        if (jobj.ContainsKey("8key") && ((JArray)jobj["8key"]).Count == 8)
        {
            for (int i = 0; i < ((JArray)jobj["8key"]).Count; i++)
            {
                EZR.Master.KeyCodeMapping[4][i] = (char)(jobj["8key"][i]);
            }
        }
    }
}
