using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectSongsUI : MonoBehaviour
{
    public GameObject SongUI;

    Transform songslistContent;
    Transform gameTypeContent;
    List<EZR.SongsList.SongInfo> currentList = new List<EZR.SongsList.SongInfo>();

    EZR.GameType currentType = EZR.PlayManager.GameType;
    EZR.GameMode.Mode currentMode = EZR.PlayManager.GameMode;
    EZR.GameDifficulty.Difficulty currentDifficulty = EZR.PlayManager.GameDifficult;
    string currentSongName = EZR.PlayManager.SongName;

    bool[] currentDifficultyState = new bool[] { false, false, false, false };

    void Start()
    {
        songslistContent = transform.Find("PanelSongsList/List/Viewport/Content");
        gameTypeContent = transform.Find("PanelSongsList/ListGameType/Viewport/Content");

        updateDifficultyState();
        focusOnBtnDifficulty();
        UpdateGameType();
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
        for (int i = songslistContent.childCount - 1; i >= 1; i--)
        {
            var song = songslistContent.GetChild(i);
            song.SetParent(null);
            Destroy(song.gameObject);
        }

        bool isHitName = false;
        Transform firstSong = null;
        Transform curSong = null;
        for (int i = 0; i < currentList.Count; i++)
        {
            var info = currentList[i];

            var songUI = Instantiate(SongUI);
            songUI.transform.SetParent(songslistContent, false);

            if (i == 0)
                firstSong = songUI.transform;

            songUI.GetComponent<EZR.SongUI>().SongName = info.name;

            var songName = songUI.transform.Find("SongName").GetComponent<Text>();
            songName.text = info.displayName;

            var difficulty = songUI.transform.Find("Difficulty").GetComponent<Text>();
            difficulty.text = info.difficulty[currentMode][currentDifficulty].ToString();

            var btn = songUI.transform.Find("Over").GetComponent<Button>();
            btn.onClick.AddListener(BtnSetCurrentSong);

            if (info.name == currentSongName)
            {
                setCurrentName(songUI.transform);
                isHitName = true;
                curSong = songUI.transform;
            }
        }

        // 滚动定位
        var scroll = songslistContent.parent.parent.GetComponent<ScrollRect>();
        if (isHitName)
        {
            scroll.verticalNormalizedPosition = 1 - curSong.GetSiblingIndex() / (float)songslistContent.childCount;
        }
        else
        {
            setCurrentName(firstSong);
            scroll.verticalNormalizedPosition = 1;
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
            focusOnBtnDifficulty("name", true);
            btn.SetSelected(false);
        }
        else
        {
            focusOnBtnDifficulty("name", false);
            btn.SetSelected(true);
        }
    }

    public void BtnSongsDifficulty()
    {
        var btn = EventSystem.current.currentSelectedGameObject.GetComponent<EZR.ButtonExtension>();
        if (btn.IsSelected)
        {
            focusOnBtnDifficulty("difficulty", true);
            btn.SetSelected(false);
        }
        else
        {
            focusOnBtnDifficulty("difficulty", false);
            btn.SetSelected(true);
        }
    }

    public void BtnSetGameType()
    {
        var btn = EventSystem.current.currentSelectedGameObject;
        var diffUI = btn.GetComponent<EZR.GameTypeUI>();

        currentType = diffUI.GameType;
        currentMode = diffUI.GameMode;
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
        UpdateGameType();

        foreach (var btn2 in EZR.ButtonExtension.GroupMaster["SongsList"])
        {
            btn2.SetSelected(false);
        }
    }

    void UpdateGameType()
    {
        foreach (Transform btn in gameTypeContent)
        {
            var diffUI = btn.GetComponent<EZR.GameTypeUI>();
            if (diffUI.GameType == currentType && diffUI.GameMode == currentMode)
            {
                btn.GetComponent<EZR.ButtonExtension>().SetSelected(true);
                break;
            }
        }
    }

    void focusOnBtnDifficulty()
    {
        int currentDifficultyNormal;
        if (currentType == EZR.GameType.DJMAX)
            currentDifficultyNormal = (int)currentDifficulty - 3;
        else
            currentDifficultyNormal = (int)currentDifficulty;
        var panelDifficulty = transform.Find("PanelDifficulty");
        for (int i = currentDifficultyNormal; i < currentDifficultyState.Length; i++)
        {
            if (currentDifficultyState[i])
            {
                var btn = panelDifficulty.GetChild(i).GetComponent<EZR.ButtonExtension>();
                updateBtnDifficulty(btn, "name", true);
                break;
            }
        }
    }
    void focusOnBtnDifficulty(string mode, bool isAsc)
    {
        int currentDifficultyNormal;
        if (currentType == EZR.GameType.DJMAX)
            currentDifficultyNormal = (int)currentDifficulty - 3;
        else
            currentDifficultyNormal = (int)currentDifficulty;
        var panelDifficulty = transform.Find("PanelDifficulty");
        for (int i = currentDifficultyNormal; i < currentDifficultyState.Length; i++)
        {
            if (currentDifficultyState[i])
            {
                var btn = panelDifficulty.GetChild(i).GetComponent<EZR.ButtonExtension>();
                updateBtnDifficulty(btn, mode, isAsc);
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
        updateBtnDifficulty(btn, "name", true);
    }

    public Color EzColor = Color.white;
    public Color NmColor = Color.white;
    public Color HdColor = Color.white;
    public Color ShdColor = Color.white;
    public Color UnselectedColor = Color.white;
    public Color DisabledColor = Color.white;
    void updateBtnDifficulty(EZR.ButtonExtension btn, string mode, bool isAsc)
    {
        filterSongs();
        switch (mode)
        {
            case "name":
                sortListByName(isAsc);
                break;
            case "difficulty":
                sortListByDifficulty(isAsc);
                break;
        }
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
        currentSongName = btn.SongName;
        var btn2 = songUI.Find("Over").GetComponent<EZR.ButtonExtension>();
        btn2.SetSelected(true);
    }

    public void StartPlay()
    {
        EZR.PlayManager.GameType = currentType;
        EZR.PlayManager.SongName = currentSongName;
        EZR.PlayManager.GameMode = currentMode;
        EZR.PlayManager.GameDifficult = currentDifficulty;
    }
}
