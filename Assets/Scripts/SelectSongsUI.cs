using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectSongsUI : MonoBehaviour
{
    public GameObject SongUI;

    Transform songslistContent;
    List<EZR.SongsList.SongInfo> currentList = new List<EZR.SongsList.SongInfo>();

    EZR.GameType currentType = EZR.PlayManager.GameType;
    EZR.GameMode.Mode currentMode = EZR.PlayManager.GameMode;
    EZR.GameDifficulty.Difficulty currentDifficulty = EZR.PlayManager.GameDifficult;
    string currentName = "";

    bool[] currentDifficultyState = new bool[] { false, false, false, false };

    void Start()
    {
        songslistContent = transform.Find("PanelSongsList/List/Viewport/Content");

        updateDifficultyState();
        focusOnBtnDifficulty();
    }

    // 过滤歌曲
    void filterSongs()
    {
        currentList.Clear();
        foreach (var info in EZR.SongsList.List)
        {
            if (info.difficulty.ContainsKey(currentMode) &&
                info.difficulty[currentMode].ContainsKey(currentDifficulty))
            {
                currentList.Add(info);
            }
        }
    }

    // 统计可用难度
    void updateDifficultyState()
    {
        bool ez = false;
        bool nm = false;
        bool hd = false;
        bool shd = false;
        foreach (var info in EZR.SongsList.List)
        {
            if (info.difficulty.ContainsKey(currentMode))
            {
                foreach (var difficulty in info.difficulty[currentMode])
                {
                    if (currentType == EZR.GameType.DJMAX)
                    {
                        switch (difficulty.Key)
                        {
                            case EZR.GameDifficulty.Difficulty.DJMAX_NM:
                                nm = true;
                                break;
                            case EZR.GameDifficulty.Difficulty.DJMAX_HD:
                                hd = true;
                                break;
                            case EZR.GameDifficulty.Difficulty.DJMAX_MX:
                                shd = true;
                                break;
                        }
                    }
                    else
                    {
                        switch (difficulty.Key)
                        {
                            case EZR.GameDifficulty.Difficulty.EZ:
                                ez = true;
                                break;
                            case EZR.GameDifficulty.Difficulty.NM:
                                nm = true;
                                break;
                            case EZR.GameDifficulty.Difficulty.HD:
                                hd = true;
                                break;
                            case EZR.GameDifficulty.Difficulty.SHD:
                                shd = true;
                                break;
                        }
                    }
                }
            }
        }

        currentDifficultyState = new bool[] { ez, nm, hd, shd };

        if (currentType == EZR.GameType.DJMAX)
        {
            if (!currentDifficultyState[(int)currentDifficulty - 3])
            {
                for (int i = 0; i < currentDifficultyState.Length; i++)
                {
                    if (currentDifficultyState[i])
                    {
                        currentDifficulty = (EZR.GameDifficulty.Difficulty)(i + 3);
                        break;
                    }
                }
            }
        }
        else
        {
            if (!currentDifficultyState[(int)currentDifficulty])
            {
                for (int i = 0; i < currentDifficultyState.Length; i++)
                {
                    if (currentDifficultyState[i])
                    {
                        currentDifficulty = (EZR.GameDifficulty.Difficulty)i;
                        break;
                    }
                }
            }
        }
    }

    // 刷新歌曲列表
    void updateList()
    {
        for (int i = 1; i < songslistContent.childCount; i++)
        {
            Destroy(songslistContent.GetChild(i).gameObject);
        }

        for (int i = 0; i < currentList.Count; i++)
        {
            var info = currentList[i];

            var songUI = Instantiate(SongUI);
            songUI.transform.SetParent(songslistContent, false);

            songUI.GetComponent<EZR.SongUI>().SongName = info.name;

            var songName = songUI.transform.Find("SongName").GetComponent<Text>();
            songName.text = info.displayName;

            var difficulty = songUI.transform.Find("Difficulty").GetComponent<Text>();
            difficulty.text = info.difficulty[currentMode][currentDifficulty].ToString();

            if (i == 0)
                setCurrentName(songUI.transform);
        }

        var panelDifficulty = transform.Find("PanelDifficulty");
        for (int i = 0; i < currentDifficultyState.Length; i++)
        {
            var btn = panelDifficulty.GetChild(i).GetComponent<Button>();
            btn.interactable = currentDifficultyState[i];
            var text = btn.transform.Find("Text").GetComponent<Text>();
            if (currentDifficultyState[i])
                text.color = UnselectedColor;
            else
                text.color = DisabledColor;
            if (i == 3)
            {
                if (currentType == EZR.GameType.DJMAX)
                    text.text = "MX";
                else
                    text.text = "SHD";
            }
        }
    }

    void sortListByName(bool isAsc)
    {
        currentList.Sort((a, b) => a.displayName.CompareTo(b.displayName));
        if (!isAsc)
            currentList.Reverse();
    }

    void sortListByDifficulty(bool isAsc)
    {
        currentList.Sort((a, b) => a.difficulty[currentMode][currentDifficulty].CompareTo(b.difficulty[currentMode][currentDifficulty]));
        if (!isAsc)
            currentList.Reverse();
    }

    public void BtnSongsName()
    {
        var btn = EventSystem.current.currentSelectedGameObject.GetComponent<EZR.ButtonExtension>();
        if (btn.IsSelected)
        {
            sortListByName(true);
            updateList();
            btn.SetSelected(false);
        }
        else
        {
            sortListByName(false);
            updateList();
            btn.SetSelected(true);
        }
    }

    public void BtnSongsDifficulty()
    {
        var btn = EventSystem.current.currentSelectedGameObject.GetComponent<EZR.ButtonExtension>();
        if (btn.IsSelected)
        {
            sortListByDifficulty(true);
            updateList();
            btn.SetSelected(false);
        }
        else
        {
            sortListByDifficulty(false);
            updateList();
            btn.SetSelected(true);
        }
    }

    public void BtnGameType(string value)
    {
        var valArr = value.Split(',');

        currentType = EZR.Utils.ParseEnum<EZR.GameType>(valArr[0]);
        currentMode = EZR.Utils.ParseEnum<EZR.GameMode.Mode>(valArr[1]);
        currentDifficulty = EZR.GameDifficulty.Difficulty.EZ;

        if (currentType == EZR.GameType.EZ2ON && (
        currentDifficulty != EZR.GameDifficulty.Difficulty.EZ &&
        currentDifficulty != EZR.GameDifficulty.Difficulty.NM &&
        currentDifficulty != EZR.GameDifficulty.Difficulty.HD &&
        currentDifficulty != EZR.GameDifficulty.Difficulty.SHD
        ))
        {
            currentDifficulty = EZR.GameDifficulty.Difficulty.EZ;
        }
        else if (currentType == EZR.GameType.EZ2ON &&
       currentMode == EZR.GameMode.Mode.ClubMix8 &&
       currentDifficulty != EZR.GameDifficulty.Difficulty.SHD)
        {
            currentDifficulty = EZR.GameDifficulty.Difficulty.SHD;
        }
        else if (currentType == EZR.GameType.EZ2DJ && (
           currentDifficulty != EZR.GameDifficulty.Difficulty.EZ &&
           currentDifficulty != EZR.GameDifficulty.Difficulty.NM &&
           currentDifficulty != EZR.GameDifficulty.Difficulty.HD &&
           currentDifficulty != EZR.GameDifficulty.Difficulty.SHD
        ))
        {
            currentDifficulty = EZR.GameDifficulty.Difficulty.EZ;
        }
        else if (currentType == EZR.GameType.DJMAX && (
           currentDifficulty != EZR.GameDifficulty.Difficulty.DJMAX_NM &&
           currentDifficulty != EZR.GameDifficulty.Difficulty.DJMAX_HD &&
           currentDifficulty != EZR.GameDifficulty.Difficulty.DJMAX_MX
        ))
        {
            currentDifficulty = EZR.GameDifficulty.Difficulty.DJMAX_NM;
        }

        updateDifficultyState();
        focusOnBtnDifficulty();

        foreach (var btn in EZR.ButtonExtension.GroupMaster["SongsList"])
        {
            btn.SetSelected(false);
        }

        {
            var btn = EventSystem.current.currentSelectedGameObject.GetComponent<EZR.ButtonExtension>();
            btn.SetSelected(true);
        }
    }

    void focusOnBtnDifficulty()
    {
        var panelDifficulty = transform.Find("PanelDifficulty");
        for (int i = 0; i < currentDifficultyState.Length; i++)
        {
            if (currentDifficultyState[i])
            {
                var btn = panelDifficulty.GetChild(i).GetComponent<EZR.ButtonExtension>();
                updateBtnDifficulty(btn);
                break;
            }
        }
    }

    public void BtnSetDifficulty()
    {
        var btn = EventSystem.current.currentSelectedGameObject.GetComponent<EZR.ButtonExtension>();
        switch (btn.gameObject.name)
        {
            case "BtnEZ":
                if (currentType == EZR.GameType.DJMAX)
                    return;
                else
                    currentDifficulty = EZR.GameDifficulty.Difficulty.EZ;
                break;
            case "BtnNM":
                if (currentType == EZR.GameType.DJMAX)
                    currentDifficulty = EZR.GameDifficulty.Difficulty.DJMAX_NM;
                else
                    currentDifficulty = EZR.GameDifficulty.Difficulty.NM;
                break;
            case "BtnHD":
                if (currentType == EZR.GameType.DJMAX)
                    currentDifficulty = EZR.GameDifficulty.Difficulty.DJMAX_HD;
                else
                    currentDifficulty = EZR.GameDifficulty.Difficulty.HD;
                break;
            case "BtnSHD":
                if (currentType == EZR.GameType.DJMAX)
                    currentDifficulty = EZR.GameDifficulty.Difficulty.DJMAX_MX;
                else
                    currentDifficulty = EZR.GameDifficulty.Difficulty.SHD;
                break;
        }
        updateDifficultyState();
        updateBtnDifficulty(btn);
    }

    public Color EzColor = Color.white;
    public Color NmColor = Color.white;
    public Color HdColor = Color.white;
    public Color ShdColor = Color.white;
    public Color UnselectedColor = Color.white;
    public Color DisabledColor = Color.white;
    void updateBtnDifficulty(EZR.ButtonExtension btn)
    {
        filterSongs();
        sortListByName(true);
        updateList();

        var text = btn.transform.Find("Text").GetComponent<Text>();
        switch (btn.gameObject.name)
        {
            case "BtnEZ":
                text.color = EzColor;
                break;
            case "BtnNM":
                text.color = NmColor;
                break;
            case "BtnHD":
                text.color = HdColor;
                break;
            case "BtnSHD":
                text.color = ShdColor;
                break;
        }
        btn.SetSelected(true);
    }

    public void BtnSetCurrentSong()
    {
        setCurrentName(EventSystem.current.currentSelectedGameObject.transform.parent);
    }

    void setCurrentName(Transform songUI)
    {
        var btn = songUI.GetComponent<EZR.SongUI>();
        currentName = btn.SongName;
        var btn2 = songUI.Find("Over").GetComponent<EZR.ButtonExtension>();
        btn2.SetSelected(true);
    }

    public void StartPlay()
    {
        EZR.PlayManager.GameType = currentType;
        EZR.PlayManager.GameMode = currentMode;
        EZR.PlayManager.GameDifficult = currentDifficulty;
    }
}
