﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.IO;
using B83.Image.BMP;

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

    Coroutine delayPlay;
    bool isStreamFirstTime = true;

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

            var ezrSongUI = songUI.GetComponent<EZR.SongUI>();
            ezrSongUI.SongName = info.name;
            ezrSongUI.DisplayName = info.displayName;
            ezrSongUI.BPM = info.bpm;
            ezrSongUI.DifficultyLevel = info.difficulty[currentMode][currentDifficulty];

            var songName = songUI.transform.Find("SongName").GetComponent<Text>();
            songName.text = ezrSongUI.DisplayName;

            var difficulty = songUI.transform.Find("Difficulty").GetComponent<Text>();
            difficulty.text = ezrSongUI.DifficultyLevel.ToString();

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
                else if (currentMode == EZR.GameMode.Mode.ClubMix8)
                    text.text = "8K";
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
        EZR.MemorySound.PlaySound("e_click");
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
        EZR.MemorySound.PlaySound("e_click");
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

        EZR.MemorySound.PlaySound("e_page");
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

        EZR.MemorySound.PlaySound("e_level");
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
        var levelText = transform.Find("Level/Text").GetComponent<Text>();
        switch (btn.gameObject.name)
        {
            case "BtnEZ":
                text.color = EzColor;
                levelText.color = EzColor;
                break;
            case "BtnNM":
                text.color = NmColor;
                levelText.color = NmColor;
                break;
            case "BtnHD":
                text.color = HdColor;
                levelText.color = HdColor;
                break;
            case "BtnSHD":
                text.color = ShdColor;
                levelText.color = ShdColor;
                break;
        }
        btn.SetSelected(true);
    }

    public void BtnSetCurrentSong()
    {
        setCurrentName(EventSystem.current.currentSelectedGameObject.transform.parent);
        EZR.MemorySound.PlaySound("e_music");
    }

    void setCurrentName(Transform songUI)
    {
        var btn = songUI.GetComponent<EZR.SongUI>();

        var isSameSong = false;
        if (isStreamFirstTime)
            isStreamFirstTime = false;
        else if (currentSongName == btn.SongName)
            isSameSong = true;

        currentSongName = btn.SongName;
        var btn2 = songUI.Find("Over").GetComponent<EZR.ButtonExtension>();
        btn2.SetSelected(true);

        // 读取碟片图
        string fileName = "";
        switch (currentType)
        {
            case EZR.GameType.EZ2ON:
                fileName = "big_" + currentSongName + EZR.GameDifficulty.GetString(currentDifficulty) + ".png";
                break;
            case EZR.GameType.EZ2DJ:
                fileName = currentSongName + EZR.GameDifficulty.GetString(currentDifficulty) + ".bmp";
                break;
            case EZR.GameType.DJMAX:
                fileName = "song_pic_f_" + currentSongName + "_" + ((int)currentDifficulty - 3).ToString().PadLeft(2, '0') + ".png";
                break;
        }
        string fullPath = Path.Combine(EZR.Master.GameResourcesFolder, currentType.ToString(), "Disc", fileName);
        var picDisc = transform.Find("Disc/PicDisc").GetComponent<RawImage>();
        if (File.Exists(fullPath))
        {
            Texture2D tex = new Texture2D(2, 2);
            if (currentType == EZR.GameType.EZ2DJ)
            {
                // bmp
                var bmpLoader = new BMPLoader();
                picDisc.texture = bmpLoader.LoadBMP(File.ReadAllBytes(fullPath)).ToTexture2D();
            }
            else
            {
                // png
                tex.LoadImage(File.ReadAllBytes(fullPath));
                picDisc.texture = tex;
            }
        }

        transform.Find("SongName").GetComponent<Text>().text = btn.DisplayName.ToUpper();
        transform.Find("Bpm/Text").GetComponent<Text>().text = btn.BPM.ToString().PadLeft(3, '0');
        transform.Find("Level/Text").GetComponent<Text>().text = btn.DifficultyLevel.ToString();

        // 播放预览音乐
        if (isSameSong) return;
        if (delayPlay != null) StopCoroutine(delayPlay);
        delayPlay = StartCoroutine(DelayPlayStream());
    }

    IEnumerator DelayPlayStream()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        string fullPath = "";
        if (currentType == EZR.GameType.DJMAX)
            fullPath = Path.Combine(EZR.Master.GameResourcesFolder, currentType.ToString(), "Preview", currentSongName);
        else
            fullPath = Path.Combine(EZR.Master.GameResourcesFolder, currentType.ToString(), "Preview", "preview_" + currentSongName);

        for (int i = 0; i < 3; i++)
        {
            var extName = ".wav";
            switch (i)
            {
                case 1:
                    extName = ".mp3";
                    break;
                case 2:
                    extName = ".ogg";
                    break;
            }
            if (File.Exists(fullPath + extName))
            {
                EZR.MemorySound.PlayStream(fullPath + extName);
                break;
            }
        }
    }

    public void BtnStart()
    {
        EZR.PlayManager.GameType = currentType;
        EZR.PlayManager.SongName = currentSongName;
        EZR.PlayManager.GameMode = currentMode;
        EZR.PlayManager.GameDifficult = currentDifficulty;
        SceneManager.LoadScene("SinglePlay");
        if (EZR.MemorySound.StreamChannel != null)
            ((FMOD.Channel)EZR.MemorySound.StreamChannel).stop();
        EZR.MemorySound.PlaySound("e_start");
    }
}