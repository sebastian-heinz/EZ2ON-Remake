using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System.IO;
using System.Text.RegularExpressions;

public class SingleSelectSongsUI : MonoBehaviour
{
    public GameObject SongUI;
    public GameObject Eyecatch;
    public GameObject Option;
    public GameObject Inventory;

    ScrollRect songsListScrollRect;

    RectTransform songsListContent;
    RectTransform gameModeContent;
    List<EZR.SongsList.SongInfo> currentList = new List<EZR.SongsList.SongInfo>();

    EZR.GameType currentType = EZR.PlayManager.GameType;
    EZR.GameMode.Mode currentMode = EZR.PlayManager.GameMode;
    EZR.GameDifficult.Difficult currentDifficult = EZR.PlayManager.GameDifficult;
    int currentSongIndex = EZR.SongsList.CurrentIndex;
    string currentSongName = EZR.PlayManager.SongName;

    float speed = EZR.PlayManager.FallSpeed;

    Coroutine delayPlay;
    bool isStreamFirstTime = true;

    float songListHeight;
    float headSplitHeight;
    float songUIHeight;
    int songUICount;

    InputField searchSongName;
    string searchText;

    void Start()
    {
        if (EZR.SongsList.List.Count == 0 || EZR.SongsList.Version < EZR.Utils.Version2Decmal(EZR.SongsList.MinVer))
        {
            var messageBox = Instantiate(EZR.Master.MessageBox);
            messageBox.transform.SetParent(transform.parent, false);
            messageBox.GetComponent<EZR.MessageBox>().Text = "加载歌曲列表失败！";
            Destroy(this);
            return;
        }

        songsListScrollRect = transform.Find("PanelSongsList/List").GetComponent<ScrollRect>();
        songsListContent = (RectTransform)songsListScrollRect.transform.Find("Viewport/Content");
        gameModeContent = (RectTransform)transform.Find("PanelSongsList/ListGameMode/Viewport/Content");
        songListHeight = ((RectTransform)songsListScrollRect.transform).sizeDelta.y;
        headSplitHeight = ((RectTransform)songsListContent.GetChild(0)).sizeDelta.y;
        songUIHeight = ((RectTransform)SongUI.transform).sizeDelta.y;
        songUICount = (int)(songListHeight / songUIHeight + 1);
        searchSongName = transform.Find("PanelSongsList/SearchSongName").GetComponent<InputField>();

        //设置滚动条最小值
        songsListScrollRect.verticalNormalizedPosition = 0;
        songsListScrollRect.onValueChanged.AddListener(value =>
        {
            if (songsListScrollRect.verticalScrollbar.size > 0)
                songsListScrollRect.verticalScrollbar.size = Mathf.Max(0.1f, songsListScrollRect.verticalScrollbar.size);
        });

        // 重新映射模式
        switch (currentMode)
        {
            case EZR.GameMode.Mode.FourButton:
                currentMode = EZR.GameMode.Mode.FourKey;
                break;
            case EZR.GameMode.Mode.FiveButton:
                currentMode = EZR.GameMode.Mode.FiveKey;
                break;
            case EZR.GameMode.Mode.SixButton:
                currentMode = EZR.GameMode.Mode.SixKey;
                break;
            case EZR.GameMode.Mode.EightButton:
                currentMode = EZR.GameMode.Mode.EightKey;
                break;
        }

        // 歌曲列表按钮初始状态
        if (EZR.SongsList.CurrentSortMode == EZR.SongsList.SortMode.ByName && !EZR.SongsList.IsAscending)
        {
            transform.Find("PanelSongsList/BtnSongsName").GetComponent<EZR.ButtonExtension>().SetSelected(true);
        }
        else if (EZR.SongsList.CurrentSortMode == EZR.SongsList.SortMode.ByDifficult && !EZR.SongsList.IsAscending)
        {
            transform.Find("PanelSongsList/BtnDifficult").GetComponent<EZR.ButtonExtension>().SetSelected(true);
        }

        filterSongs();
        updateDifficultState();
        focusOnBtnDifficult();
        updateGameModeButton();
        updateGameType();
        updateList();
        updateBtnSpeed();
    }

    // 过滤歌曲
    void filterSongs()
    {
        currentList.Clear();
        foreach (var info in EZR.SongsList.List)
        {
            if (info.isHidden) continue;
            if (!string.IsNullOrEmpty(searchText) &&
            !Regex.IsMatch(info.displayName, "^" + Regex.Escape(searchText), RegexOptions.IgnoreCase))
                continue;

            bool isHit = false;
            if (info.difficult.ContainsKey(currentMode))
                isHit = true;
            else if (info.difficult.ContainsKey(EZR.SongsList.DJMaxModeMapping(currentMode)))
                isHit = true;

            if (isHit) currentList.Add(info);
        }
    }

    // 歌曲难度状态
    bool[] songDifficultState(EZR.SongsList.SongInfo info)
    {
        bool ez = false;
        bool nm = false;
        bool hd = false;
        bool shd = false;
        bool ex = false;

        if (currentType == EZR.GameType.DJMAX)
        {
            if (info.GetCurrentMode(currentMode, EZR.GameDifficult.Difficult.DJMAX_EZ) != EZR.GameMode.Mode.None)
                ez = true;
            if (info.GetCurrentMode(currentMode, EZR.GameDifficult.Difficult.DJMAX_NM) != EZR.GameMode.Mode.None)
                nm = true;
            if (info.GetCurrentMode(currentMode, EZR.GameDifficult.Difficult.DJMAX_HD) != EZR.GameMode.Mode.None)
                hd = true;
            if (info.GetCurrentMode(currentMode, EZR.GameDifficult.Difficult.DJMAX_MX) != EZR.GameMode.Mode.None)
                shd = true;
            if (info.GetCurrentMode(currentMode, EZR.GameDifficult.Difficult.DJMAX_SC) != EZR.GameMode.Mode.None)
                ex = true;
        }
        else
        {
            if (info.GetCurrentMode(currentMode, EZR.GameDifficult.Difficult.EZ) != EZR.GameMode.Mode.None)
                ez = true;
            if (info.GetCurrentMode(currentMode, EZR.GameDifficult.Difficult.NM) != EZR.GameMode.Mode.None)
                nm = true;
            if (info.GetCurrentMode(currentMode, EZR.GameDifficult.Difficult.HD) != EZR.GameMode.Mode.None)
                hd = true;
            if (info.GetCurrentMode(currentMode, EZR.GameDifficult.Difficult.SHD) != EZR.GameMode.Mode.None)
                shd = true;
            if (info.GetCurrentMode(currentMode, EZR.GameDifficult.Difficult.EX) != EZR.GameMode.Mode.None)
                ex = true;
        }

        return new bool[] { ez, nm, hd, shd, ex };
    }

    // 找首个可用的歌曲和难度
    void updateDifficultState()
    {
        // 当前歌曲和难度直接可用，直接排一次序就完事
        if (EZR.SongsList.List[currentSongIndex].GetCurrentMode(currentMode, currentDifficult) != EZR.GameMode.Mode.None)
        {
            switch (EZR.SongsList.CurrentSortMode)
            {
                case EZR.SongsList.SortMode.ByName:
                    sortListByName(EZR.SongsList.IsAscending);
                    break;
                case EZR.SongsList.SortMode.ByDifficult:
                    sortListByDifficult(EZR.SongsList.IsAscending);
                    break;
            }
            return;
        }

        bool[] currentDifficultState = new bool[] { false, false, false, false, false };

        foreach (var info in EZR.SongsList.List)
        {
            var state = songDifficultState(info);
            currentDifficultState[0] |= state[0];
            currentDifficultState[1] |= state[1];
            currentDifficultState[2] |= state[2];
            currentDifficultState[3] |= state[3];
            currentDifficultState[4] |= state[4];
        }

        if (currentType == EZR.GameType.DJMAX)
        {
            if (!currentDifficultState[(int)currentDifficult - (int)EZR.GameDifficult.Difficult.DJMAX_EZ])
            {
                for (int i = 0; i < currentDifficultState.Length; i++)
                {
                    if (currentDifficultState[i])
                    {
                        currentDifficult = (EZR.GameDifficult.Difficult)(i + (int)EZR.GameDifficult.Difficult.DJMAX_EZ);
                        break;
                    }
                }
            }
        }
        else
        {
            if (!currentDifficultState[(int)currentDifficult])
            {
                for (int i = 0; i < currentDifficultState.Length; i++)
                {
                    if (currentDifficultState[i])
                    {
                        currentDifficult = (EZR.GameDifficult.Difficult)i;
                        break;
                    }
                }
            }
        }

        // 第一次排序
        switch (EZR.SongsList.CurrentSortMode)
        {
            case EZR.SongsList.SortMode.ByName:
                sortListByName(EZR.SongsList.IsAscending);
                break;
            case EZR.SongsList.SortMode.ByDifficult:
                sortListByDifficult(EZR.SongsList.IsAscending);
                break;
        }

        // 找当前歌曲可用难度
        var state2 = songDifficultState(EZR.SongsList.List[currentSongIndex]);
        bool isHit = false;
        for (int i = 0; i < state2.Length; i++)
        {
            if (state2[i])
            {
                isHit = true;
                if (currentType == EZR.GameType.DJMAX)
                    currentDifficult = (EZR.GameDifficult.Difficult)(i + (int)EZR.GameDifficult.Difficult.DJMAX_EZ);
                else
                    currentDifficult = (EZR.GameDifficult.Difficult)i;
                break;
            }
        }
        // 找首个可用的歌曲
        if (!isHit)
        {
            foreach (var info in currentList)
            {
                state2 = songDifficultState(info);
                for (int i = 0; i < state2.Length; i++)
                {
                    if (state2[i])
                    {
                        isHit = true;
                        currentSongIndex = EZR.SongsList.List.IndexOf(info);
                        if (currentType == EZR.GameType.DJMAX)
                            currentDifficult = (EZR.GameDifficult.Difficult)(i + (int)EZR.GameDifficult.Difficult.DJMAX_EZ);
                        else
                            currentDifficult = (EZR.GameDifficult.Difficult)i;
                        break;
                    }
                }
                if (isHit) break;
            }
        }

        // 第二次排序
        switch (EZR.SongsList.CurrentSortMode)
        {
            case EZR.SongsList.SortMode.ByName:
                sortListByName(EZR.SongsList.IsAscending);
                break;
            case EZR.SongsList.SortMode.ByDifficult:
                sortListByDifficult(EZR.SongsList.IsAscending);
                break;
        }
    }

    // 刷新歌曲列表
    void updateList()
    {
        // 生成songUI实例
        if (songsListContent.childCount < songUICount)
        {
            for (int i = 0; i < songUICount; i++)
            {
                var songUI = Instantiate(SongUI);
                songUI.transform.SetParent(songsListContent, false);
                songUI.GetComponent<EZR.SongUI>().Index = i;
                var btn = songUI.transform.Find("Over").GetComponent<Button>();
                btn.onClick.AddListener(BtnSetCurrentSong);
            }
        }

        updateSongsListContentHeight();

        // 滚动定位
        songsListScrollRect.verticalNormalizedPosition =
            1 - Mathf.Clamp01(
                    ((currentList.IndexOf(EZR.SongsList.List[currentSongIndex]) - songUICount / 2 + 1) * songUIHeight) / (songsListContent.sizeDelta.y - songListHeight));

        var selectedSongUI = updateSongUI(true);
        if (EZR.ButtonExtension.GroupMaster["SongUI"].CurrentSelected != null)
            selectedSongUI = EZR.ButtonExtension.GroupMaster["SongUI"].CurrentSelected.transform.parent;
        if (selectedSongUI != null)
            setCurrentSong(selectedSongUI, "level");
    }

    void updateSongsListContentHeight()
    {
        songsListContent.sizeDelta = new Vector2(
            songsListContent.sizeDelta.x,
            4 + headSplitHeight + currentList.Count * songUIHeight + 100
        );
    }

    Transform updateSongUI(bool isRefresh)
    {
        float currentPosition = songsListContent.localPosition.y - (4 + headSplitHeight);
        int startIndex = (int)(currentPosition / songUIHeight);

        Transform outSongUI = null;

        for (int i = 1; i < songsListContent.childCount; i++)
        {
            var songUI = (RectTransform)songsListContent.GetChild(i);
            var songUIComp = songUI.GetComponent<EZR.SongUI>();

            bool isLoop = false;
            while (songUIComp.Index < startIndex)
            {
                isLoop = true;
                songUIComp.Index += songUICount;
            }
            while (songUIComp.Index > startIndex + (songUICount - 1))
            {
                isLoop = true;
                songUIComp.Index -= songUICount;
            }

            if (isRefresh || isLoop)
            {
                if (songUIComp.Index < 0 || songUIComp.Index >= currentList.Count)
                {
                    songUI.gameObject.SetActive(false);
                    continue;
                }
                else
                {
                    songUI.gameObject.SetActive(true);
                    if (songUIComp.Index == startIndex)
                        outSongUI = songUI;
                }

                songUI.localPosition = new Vector3(
                    songUI.localPosition.x,
                    -(4 + headSplitHeight) - songUIComp.Index * songUIHeight,
                    0
                );
                var songInfo = currentList[songUIComp.Index];
                songUIComp.SongIndex = EZR.SongsList.List.IndexOf(songInfo);
                if (songUIComp.SongIndex == currentSongIndex)
                {
                    songUI.Find("Over").GetComponent<EZR.ButtonExtension>().SetSelected(true);
                }
                else
                {
                    songUI.Find("Over").GetComponent<EZR.ButtonExtension>().SetSelected(false);
                }
                var songName = songUI.transform.Find("SongName").GetComponent<Text>();
                songName.text = songInfo.displayName;
                var difficult = songUI.transform.Find("Difficult").GetComponent<Text>();
                var btn = songUI.transform.Find("Over").GetComponent<Button>();

                if (songInfo.GetCurrentMode(currentMode, currentDifficult) != EZR.GameMode.Mode.None)
                {
                    difficult.text = songInfo.difficult[songInfo.GetCurrentMode(currentMode, currentDifficult)][currentDifficult].ToString();
                    songName.color = songUIComp.NormalColor;
                    difficult.color = Color.white;
                    btn.interactable = true;
                }
                else
                {
                    songName.color = Color.gray;
                    difficult.text = "--";
                    difficult.color = Color.gray;
                    btn.interactable = false;
                }
            }
        }

        return outSongUI;
    }

    void sortListByName(bool isAsc)
    {
        currentList.Sort((a, b) => a.displayName.CompareTo(b.displayName));
        if (!isAsc)
            currentList.Reverse();
    }

    void sortListByDifficult(bool isAsc)
    {
        currentList.Sort((a, b) =>
        {
            int numA, numB;
            if (a.GetCurrentMode(currentMode, currentDifficult) != EZR.GameMode.Mode.None) numA = a.difficult[a.GetCurrentMode(currentMode, currentDifficult)][currentDifficult];
            else numA = 0;
            if (b.GetCurrentMode(currentMode, currentDifficult) != EZR.GameMode.Mode.None) numB = b.difficult[b.GetCurrentMode(currentMode, currentDifficult)][currentDifficult];
            else numB = 0;
            return numA.CompareTo(numB);
        });
        if (!isAsc)
            currentList.Reverse();
    }

    public void BtnSongsName()
    {
        EZR.SongsList.CurrentSortMode = EZR.SongsList.SortMode.ByName;
        var btn = EventSystem.current.currentSelectedGameObject.GetComponent<EZR.ButtonExtension>();
        if (btn.IsSelected)
        {
            EZR.SongsList.IsAscending = true;
            btn.SetSelected(false);
        }
        else
        {
            EZR.SongsList.IsAscending = false;
            btn.SetSelected(true);
        }
        sortListByName(EZR.SongsList.IsAscending);
        updateList();
        EZR.MemorySound.PlaySound("e_click");
    }

    public void BtnSongsDifficult()
    {
        EZR.SongsList.CurrentSortMode = EZR.SongsList.SortMode.ByDifficult;
        var btn = EventSystem.current.currentSelectedGameObject.GetComponent<EZR.ButtonExtension>();
        if (btn.IsSelected)
        {
            EZR.SongsList.IsAscending = true;
            btn.SetSelected(false);
        }
        else
        {
            EZR.SongsList.IsAscending = false;
            btn.SetSelected(true);
        }
        sortListByDifficult(EZR.SongsList.IsAscending);
        updateList();
        EZR.MemorySound.PlaySound("e_click");
    }

    public void BtnSetGameType()
    {
        var btn = EventSystem.current.currentSelectedGameObject.GetComponent<EZR.ButtonExtension>();
        if (btn.IsSelected) return;

        switch (btn.gameObject.name)
        {
            case "BtnEZ2ON":
                currentType = EZR.GameType.EZ2ON;
                currentMode = EZR.GameMode.Mode.RubyMixON;
                break;
            case "BtnEZ2DJ":
                currentType = EZR.GameType.EZ2DJ;
                currentMode = EZR.GameMode.Mode.RubyMixDJ;
                break;
            case "BtnDJMAX":
                currentType = EZR.GameType.DJMAX;
                currentMode = EZR.GameMode.Mode.FourKey;
                break;
        }
        currentDifficult = EZR.GameDifficult.Difficult.EZ;
        updateGameModeButton();

        updateGameDefaultDifficut();

        filterSongs();
        updateDifficultState();
        focusOnBtnDifficult();
        updateGameType();
        updateList();

        EZR.MemorySound.PlaySound("e_page");
    }

    public void BtnSetGameMode()
    {
        var btn = EventSystem.current.currentSelectedGameObject.GetComponent<EZR.ButtonExtension>();
        if (btn.IsSelected) return;
        var diffUI = btn.GetComponent<EZR.GameTypeUI>();

        currentType = diffUI.GameType;
        currentMode = diffUI.GameMode;
        currentDifficult = EZR.GameDifficult.Difficult.EZ;

        updateGameDefaultDifficut();

        filterSongs();
        updateDifficultState();
        focusOnBtnDifficult();
        btn.SetSelected(true);
        updateList();

        EZR.MemorySound.PlaySound("e_page");
    }

    void updateGameModeButton()
    {
        foreach (Transform btn2 in gameModeContent)
        {
            btn2.gameObject.SetActive(false);
        }
        switch (currentType)
        {
            case EZR.GameType.EZ2ON:
                gameModeContent.Find("BtnOnRuby").gameObject.SetActive(true);
                gameModeContent.Find("BtnOnStreet").gameObject.SetActive(true);
                gameModeContent.Find("BtnOnClub").gameObject.SetActive(true);
                gameModeContent.Find("BtnOn8K").gameObject.SetActive(true);
                break;
            case EZR.GameType.EZ2DJ:
                gameModeContent.Find("BtnDjRuby").gameObject.SetActive(true);
                gameModeContent.Find("BtnDjStreet").gameObject.SetActive(true);
                gameModeContent.Find("BtnDj5K").gameObject.SetActive(true);
                break;
            case EZR.GameType.DJMAX:
                gameModeContent.Find("BtnMax4K").gameObject.SetActive(true);
                gameModeContent.Find("BtnMax5K").gameObject.SetActive(true);
                gameModeContent.Find("BtnMax6K").gameObject.SetActive(true);
                gameModeContent.Find("BtnMax7K").gameObject.SetActive(true);
                gameModeContent.Find("BtnMax8K").gameObject.SetActive(true);
                break;
        }
    }

    void updateGameDefaultDifficut()
    {
        if (currentType == EZR.GameType.EZ2ON && (
        currentDifficult != EZR.GameDifficult.Difficult.EZ &&
        currentDifficult != EZR.GameDifficult.Difficult.NM &&
        currentDifficult != EZR.GameDifficult.Difficult.HD &&
        currentDifficult != EZR.GameDifficult.Difficult.SHD &&
        currentDifficult != EZR.GameDifficult.Difficult.EX
        ))
        {
            currentDifficult = EZR.GameDifficult.Difficult.EZ;
        }
        else if (currentType == EZR.GameType.EZ2DJ && (
            currentDifficult != EZR.GameDifficult.Difficult.EZ &&
            currentDifficult != EZR.GameDifficult.Difficult.NM &&
            currentDifficult != EZR.GameDifficult.Difficult.HD &&
            currentDifficult != EZR.GameDifficult.Difficult.SHD &&
            currentDifficult != EZR.GameDifficult.Difficult.EX
        ))
        {
            currentDifficult = EZR.GameDifficult.Difficult.EZ;
        }
        else if (currentType == EZR.GameType.DJMAX && (
            currentDifficult != EZR.GameDifficult.Difficult.DJMAX_EZ &&
            currentDifficult != EZR.GameDifficult.Difficult.DJMAX_NM &&
            currentDifficult != EZR.GameDifficult.Difficult.DJMAX_HD &&
            currentDifficult != EZR.GameDifficult.Difficult.DJMAX_MX &&
            currentDifficult != EZR.GameDifficult.Difficult.DJMAX_SC
        ))
        {
            currentDifficult = EZR.GameDifficult.Difficult.DJMAX_EZ;
        }
    }

    void updateGameType()
    {
        switch (currentType)
        {
            case EZR.GameType.EZ2ON:
                transform.Find("PanelSongsList/ListGameType/BtnEZ2ON")
                .GetComponent<EZR.ButtonExtension>().SetSelected(true);
                break;
            case EZR.GameType.EZ2DJ:
                transform.Find("PanelSongsList/ListGameType/BtnEZ2DJ")
                .GetComponent<EZR.ButtonExtension>().SetSelected(true);
                break;
            case EZR.GameType.DJMAX:
                transform.Find("PanelSongsList/ListGameType/BtnDJMAX")
                .GetComponent<EZR.ButtonExtension>().SetSelected(true);
                break;
        }
        foreach (Transform btn in gameModeContent)
        {
            var typeUI = btn.GetComponent<EZR.GameTypeUI>();
            if (typeUI.GameType == currentType)
            {
                if (typeUI.GameMode == currentMode ||
                EZR.SongsList.DJMaxModeMapping(typeUI.GameMode) == currentMode)
                {
                    btn.GetComponent<EZR.ButtonExtension>().SetSelected(true);
                    break;
                }
            }
        }
    }

    void focusOnBtnDifficult()
    {
        int currentDifficultNormal;
        if (currentType == EZR.GameType.DJMAX)
            currentDifficultNormal = (int)currentDifficult - (int)EZR.GameDifficult.Difficult.DJMAX_EZ;
        else
            currentDifficultNormal = (int)currentDifficult;
        var panelDifficult = transform.Find("PanelDifficult");

        var btn = panelDifficult.GetChild(currentDifficultNormal).GetComponent<EZR.ButtonExtension>();
        updateBtnDifficult(btn);
    }

    public void BtnSetDifficult()
    {
        var btn = EventSystem.current.currentSelectedGameObject.GetComponent<EZR.ButtonExtension>();
        if (btn.IsSelected) return;
        switch (btn.gameObject.name)
        {
            case "BtnEZ":
                if (currentType == EZR.GameType.DJMAX)
                    currentDifficult = EZR.GameDifficult.Difficult.DJMAX_EZ;
                else
                    currentDifficult = EZR.GameDifficult.Difficult.EZ;
                break;
            case "BtnNM":
                if (currentType == EZR.GameType.DJMAX)
                    currentDifficult = EZR.GameDifficult.Difficult.DJMAX_NM;
                else
                    currentDifficult = EZR.GameDifficult.Difficult.NM;
                break;
            case "BtnHD":
                if (currentType == EZR.GameType.DJMAX)
                    currentDifficult = EZR.GameDifficult.Difficult.DJMAX_HD;
                else
                    currentDifficult = EZR.GameDifficult.Difficult.HD;
                break;
            case "BtnSHD":
                if (currentType == EZR.GameType.DJMAX)
                    currentDifficult = EZR.GameDifficult.Difficult.DJMAX_MX;
                else
                    currentDifficult = EZR.GameDifficult.Difficult.SHD;
                break;
            case "BtnEX":
                if (currentType == EZR.GameType.DJMAX)
                    currentDifficult = EZR.GameDifficult.Difficult.DJMAX_SC;
                else
                    currentDifficult = EZR.GameDifficult.Difficult.EX;
                break;
        }

        updateBtnDifficult(btn);
        switch (EZR.SongsList.CurrentSortMode)
        {
            case EZR.SongsList.SortMode.ByName:
                sortListByName(EZR.SongsList.IsAscending);
                break;
            case EZR.SongsList.SortMode.ByDifficult:
                sortListByDifficult(EZR.SongsList.IsAscending);
                break;
        }
        updateList();

        EZR.MemorySound.PlaySound("e_level");
    }

    [FormerlySerializedAs("EzColor")]
    public Color EzColor = Color.white;
    [FormerlySerializedAs("NmColor")]
    public Color NmColor = Color.white;
    [FormerlySerializedAs("HdColor")]
    public Color HdColor = Color.white;
    [FormerlySerializedAs("ShdColor")]
    public Color ShdColor = Color.white;
    [FormerlySerializedAs("ExColor")]
    public Color ExColor = Color.white;
    [FormerlySerializedAs("UnselectedColor")]
    public Color UnselectedColor = Color.white;
    [FormerlySerializedAs("DisabledColor")]
    public Color DisabledColor = Color.white;
    void updateBtnDifficult(EZR.ButtonExtension btn)
    {
        // 更新难度按钮状态
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
            case "BtnEX":
                text.color = ExColor;
                levelText.color = ExColor;
                break;
        }

        btn.SetSelected(true);
        updateBtnDifficult();
    }

    void updateBtnDifficult()
    {
        var panelDifficult = transform.Find("PanelDifficult");
        var state = songDifficultState(EZR.SongsList.List[currentSongIndex]);
        for (int i = 0; i < state.Length; i++)
        {
            var btn = panelDifficult.GetChild(i).GetComponent<Button>();
            var text = btn.transform.Find("Text").GetComponent<Text>();
            if (i == 3)
            {
                if (currentType == EZR.GameType.DJMAX)
                    text.text = "MX";
                else
                    text.text = "SHD";
            }
            if (i == 4)
            {
                if (currentType == EZR.GameType.DJMAX)
                    text.text = "SC";
                else
                    text.text = "EX";
            }
            btn.interactable = state[i];
            if (btn.GetComponent<EZR.ButtonExtension>().IsSelected) continue;
            if (state[i])
                text.color = UnselectedColor;
            else
                text.color = DisabledColor;
        }
    }

    public void BtnSetCurrentSong()
    {
        var btn = EventSystem.current.currentSelectedGameObject.GetComponent<EZR.ButtonExtension>();
        if (btn.IsSelected) return;
        setCurrentSong(btn.transform.parent, "change");
        EZR.MemorySound.PlaySound("e_music");
    }

    public Texture DefaultDiscImage;

    async void setCurrentSong(Transform songUI, string state)
    {
        currentSongIndex = songUI.GetComponent<EZR.SongUI>().SongIndex;
        var info = EZR.SongsList.List[currentSongIndex];

        updateBtnDifficult();

        var isSameSong = false;
        if (isStreamFirstTime)
            isStreamFirstTime = false;
        else if (currentSongName == info.name)
            isSameSong = true;

        currentSongName = info.name;
        var btn = songUI.Find("Over").GetComponent<EZR.ButtonExtension>();
        btn.SetSelected(true);

        var disc = transform.Find("Disc/PicDisc");

        switch (state)
        {
            case "change":
                {
                    var oldDisc = Instantiate(disc);
                    oldDisc.SetParent(disc.parent, false);
                    var oldDiscAnim = oldDisc.GetComponent<Animation>();
                    oldDiscAnim.Play("DiscOut");
                    StartCoroutine(discOut(oldDiscAnim));
                    var anim = disc.GetComponent<Animation>();
                    anim["DiscIn"].time = 0;
                    anim.Play("DiscIn");
                }
                break;
            case "level":
                {
                    var anim = disc.GetComponent<Animation>();
                    anim["DiscLevel"].time = 0;
                    anim.Play("DiscLevel");
                }
                break;
        }

        // 读取碟片图
        string fileName = "";
        switch (currentType)
        {
            case EZR.GameType.EZ2ON:
                if (currentDifficult == EZR.GameDifficult.Difficult.EX)
                    fileName = "big_" + currentSongName + EZR.GameDifficult.GetString(EZR.GameDifficult.Difficult.SHD) + ".png";
                else
                    fileName = "big_" + currentSongName + EZR.GameDifficult.GetString(currentDifficult) + ".png";
                break;
            case EZR.GameType.EZ2DJ:
                fileName = currentSongName + EZR.GameDifficult.GetString(currentDifficult) + ".bmp";
                break;
            case EZR.GameType.DJMAX:
                if (info.GetCurrentMode(currentMode, currentDifficult) == EZR.GameMode.Mode.FourKey ||
                    info.GetCurrentMode(currentMode, currentDifficult) == EZR.GameMode.Mode.FiveKey ||
                    info.GetCurrentMode(currentMode, currentDifficult) == EZR.GameMode.Mode.SixKey ||
                    info.GetCurrentMode(currentMode, currentDifficult) == EZR.GameMode.Mode.SevenKey ||
                    info.GetCurrentMode(currentMode, currentDifficult) == EZR.GameMode.Mode.EightKey)
                    fileName = currentSongName + "_ORG" + EZR.GameDifficult.GetString(currentDifficult) + ".png";
                else
                    fileName = "song_pic_f_" + currentSongName + "_" + ((int)currentDifficult - (int)EZR.GameDifficult.Difficult.DJMAX_EZ).ToString().PadLeft(2, '0') + ".png";
                break;
        }
        var buffer = await EZR.DataLoader.LoadFileAsync(EZR.DataLoader.GetEZRDataPath(currentType, currentSongName), fileName);
        if (buffer != null)
        {
            var image = disc.Find("Image");
            var dmo = disc.Find("Dmo");
            if (currentType == EZR.GameType.DJMAX
            && (info.GetCurrentMode(currentMode, currentDifficult) == EZR.GameMode.Mode.FourKey ||
            info.GetCurrentMode(currentMode, currentDifficult) == EZR.GameMode.Mode.FiveKey ||
            info.GetCurrentMode(currentMode, currentDifficult) == EZR.GameMode.Mode.SixKey ||
            info.GetCurrentMode(currentMode, currentDifficult) == EZR.GameMode.Mode.SevenKey ||
            info.GetCurrentMode(currentMode, currentDifficult) == EZR.GameMode.Mode.EightKey))
            {
                image.gameObject.SetActive(false);
                dmo.gameObject.SetActive(true);
                dmo.GetComponent<RawImage>().texture = EZR.ImageLoader.Load(buffer, fileName);
            }
            else
            {
                image.gameObject.SetActive(true);
                dmo.gameObject.SetActive(false);
                image.GetComponent<RawImage>().texture = EZR.ImageLoader.Load(buffer, fileName);
            }
        }
        else
        {
            var image = disc.Find("Image");
            var dmo = disc.Find("Dmo");
            image.gameObject.SetActive(true);
            dmo.gameObject.SetActive(false);
            image.GetComponent<RawImage>().texture = DefaultDiscImage;
        }

        var songNameText = transform.Find("SongInfo/SongName").GetComponent<Text>();
        var composerNameText = transform.Find("SongInfo/ComposerName").GetComponent<Text>();
        if (string.IsNullOrEmpty(info.composer))
        {
            songNameText.text = info.displayName.ToUpper();
            composerNameText.gameObject.SetActive(false);
        }
        else
        {
            songNameText.text = info.displayName.ToUpper();
            composerNameText.gameObject.SetActive(true);
            composerNameText.text = info.composer;
        }
        transform.Find("Bpm/Text").GetComponent<Text>().text = info.bpm.ToString().PadLeft(3, '0');
        transform.Find("Level/Text").GetComponent<Text>().text = info.difficult[info.GetCurrentMode(currentMode, currentDifficult)][currentDifficult].ToString();

        // 播放预览音乐
        if (!isSameSong)
        {
            if (delayPlay != null) StopCoroutine(delayPlay);
            EZR.MemorySound.StopSound();
            EZR.MemorySound.StopStream();
            delayPlay = StartCoroutine(DelayPlayPreview(currentSongName));
        }

        transform.Find("MyBestScore/Text").GetComponent<Text>().text = EZR.UserSaveData.GetScore(currentSongName, currentType, info.GetCurrentMode(currentMode, currentDifficult), currentDifficult).ToString();
    }

    IEnumerator DelayPlayPreview(string songName)
    {
        yield return new WaitForSeconds(0.4f);

        string fileName = "";
        if (currentType == EZR.GameType.DJMAX)
            fileName = currentSongName;
        else
            fileName = "preview_" + currentSongName;
        playPreview(fileName, songName);
    }

    async void playPreview(string fileName, string songName)
    {
        var ezrPath = EZR.DataLoader.GetEZRDataPath(currentType, currentSongName);
        var isExists = false;
        for (int i = 0; i < 3; i++)
        {
            var soundFileName = "";
            switch (i)
            {
                case 0:
                    soundFileName = fileName + ".wav";
                    break;
                case 1:
                    soundFileName = fileName + ".ogg";
                    break;
                case 2:
                    soundFileName = fileName + ".mp3";
                    break;
            }
            if (EZR.DataLoader.Exists(ezrPath, soundFileName))
            {
                isExists = true;
                var buffer = await EZR.DataLoader.LoadFileAsync(ezrPath, soundFileName);
                if (songName == currentSongName)
                    EZR.MemorySound.PlaySound(buffer, true);
                break;
            }
        }
        if (!isExists && currentType == EZR.GameType.DJMAX)
        {
            var buffer = await EZR.DataLoader.LoadFileAsync(Path.Combine(EZR.Master.GameResourcesFolder, "BGM.ezr"), "FreeMode.ogg");
            EZR.MemorySound.PlaySound(buffer, true);
        }
    }

    public async void BtnStart()
    {
        var btn = transform.Find("BtnStart").GetComponent<Button>();
        if (!btn.interactable) return;
        else btn.interactable = false;

        var ezrPath = EZR.DataLoader.GetEZRDataPath(currentType, currentSongName);

        if (File.Exists(ezrPath))
        {
            // 检查sha2
            // var ezrDataBuffer = new byte[0];
            // await Task.Run(async () =>
            // {
            //     using (var stream = File.OpenRead(ezrPath))
            //     {
            //         ezrDataBuffer = new byte[stream.Length];
            //         await stream.ReadAsync(ezrDataBuffer, 0, (int)stream.Length);
            //     }
            // });
            // var sha2 = await EZR.Hash.Sha2Async(ezrDataBuffer);

            // if (sha2.ToLower() != EZR.SongsList.List[currentSongIndex].sha1.ToLower())
            // {
            //     var messageBox = Instantiate(EZR.Master.MessageBox);
            //     messageBox.transform.SetParent(transform.parent, false);
            //     messageBox.GetComponent<EZR.MessageBox>().Text = "歌曲包完整性校验失败，歌曲包可能被修改或损坏！";
            //     btn.interactable = true;
            //     return;
            // }
        }
        else
        {
            var messageBox = Instantiate(EZR.Master.MessageBox);
            messageBox.transform.SetParent(transform.parent, false);
            messageBox.GetComponent<EZR.MessageBox>().Text = "缺少歌曲包！";
            btn.interactable = true;
            return;
        }

        var mode = EZR.SongsList.List[currentSongIndex].GetCurrentMode(currentMode, currentDifficult);

        // 检查json文件是否存在
        var jsonFileName = PatternUtils.Pattern.GetFileName(currentSongName, currentType, mode, currentDifficult);
        if (!EZR.DataLoader.Exists(ezrPath, jsonFileName))
        {
            var messageBox = Instantiate(EZR.Master.MessageBox);
            messageBox.transform.SetParent(transform.parent, false);
            messageBox.GetComponent<EZR.MessageBox>().Text = "缺少曲谱文件！";
            btn.interactable = true;
            return;
        }

        // 读取Eyecatch
        string fileName = "";
        switch (currentType)
        {
            case EZR.GameType.EZ2ON:
                fileName = "eye_" + currentSongName + ".png";
                break;
            case EZR.GameType.EZ2DJ:
                fileName = currentSongName + ".png";
                break;
            case EZR.GameType.DJMAX:
                fileName = "eyecatch_" + currentSongName + ".png";
                break;
        }
        var buffer = await EZR.DataLoader.LoadFileAsync(ezrPath, fileName);

        var eyecatch = Instantiate(Eyecatch);
        eyecatch.transform.SetParent(transform.parent, false);
        if (buffer != null)
        {
            eyecatch.GetComponent<RawImage>().texture = EZR.ImageLoader.Load(buffer, fileName);
        }

        var anim = eyecatch.GetComponent<Animation>();
        anim.Play("Eyecatch");
        StartCoroutine(startPlay(anim));

        EZR.PlayManager.GameType = currentType;
        EZR.SongsList.CurrentIndex = currentSongIndex;
        EZR.PlayManager.SongName = currentSongName;
        EZR.PlayManager.GameMode = mode;
        EZR.PlayManager.GameDifficult = currentDifficult;
        EZR.PlayManager.FallSpeed = speed;

        if (delayPlay != null) StopCoroutine(delayPlay);

        if (currentType == EZR.GameType.DJMAX)
            EZR.MemorySound.PlaySound("Decide");
        else
            EZR.MemorySound.PlaySound("e_start");
    }

    IEnumerator startPlay(Animation anim)
    {
        while (anim.isPlaying)
        {
            yield return null;
        }
        EZR.MemorySound.StopSound();
        EZR.MemorySound.StopStream();
        SceneManager.LoadScene("SinglePlay");
    }

    IEnumerator discOut(Animation anim)
    {
        while (anim.isPlaying)
        {
            yield return null;
        }
        Destroy(anim.gameObject);
    }

    public void BtnChangeSpeed(BaseEventData baseEventData)
    {
        var pointerEventData = (PointerEventData)baseEventData;
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            speedAdd(0.25f);
        }
        else if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            speedAdd(-0.25f);
        }
    }

    void updateBtnSpeed()
    {
        transform.Find("PanelEffect/Speed/Text").GetComponent<Text>().text = speed.ToString("0.00");
    }

    Coroutine speedPressedCoroutine;
    void Update()
    {
        updateSongUI(false);

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                speedAddSmall(0.01f);
                if (speedPressedCoroutine != null) StopCoroutine(speedPressedCoroutine);
                speedPressedCoroutine = StartCoroutine(speedPressed(0.01f));
            }
            else if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                if (speedPressedCoroutine != null) StopCoroutine(speedPressedCoroutine);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                speedAddSmall(-0.01f);
                if (speedPressedCoroutine != null) StopCoroutine(speedPressedCoroutine);
                speedPressedCoroutine = StartCoroutine(speedPressed(-0.01f));
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                if (speedPressedCoroutine != null) StopCoroutine(speedPressedCoroutine);
            }
        }
        else
        {
            if (speedPressedCoroutine != null) StopCoroutine(speedPressedCoroutine);
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                speedAdd(0.25f);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                speedAdd(-0.25f);
            }
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            BtnStart();
        }
    }

    IEnumerator speedPressed(float val)
    {
        yield return new WaitForSeconds(0.2f);
        for (; ; )
        {
            yield return new WaitForSeconds(0.075f);
            speedAddSmall(val);
        }
    }

    void speedAddSmall(float val)
    {
        speed = Mathf.Max(speed + val, 0.25f);
        updateBtnSpeed();
        EZR.MemorySound.PlaySound("e_count_1");
    }

    void speedAdd(float val)
    {
        var decimalPart = speed % 1;
        float closest;
        if (val > 0)
            closest = EZR.Utils.FindClosestNumber(decimalPart, EZR.PlayManager.FallSpeedStep, true);
        else
            closest = EZR.Utils.FindClosestNumber(decimalPart, EZR.PlayManager.FallSpeedStep, false);
        if (Mathf.Abs(((int)speed + closest) - speed) > 0.009f)
            speed = Mathf.Max((int)speed + closest, 0.25f);
        else
            speed = Mathf.Max(((int)speed + closest) + val, 0.25f);
        updateBtnSpeed();
        EZR.MemorySound.PlaySound("e_count_1");
    }

    // 暂时退出程序
    public void BtnBack()
    {
        EZR.MemorySound.PlaySound("e_motion");
        Application.Quit();
    }

    // 打开设置面板
    public void BtnOption()
    {
        var option = Instantiate(Option);
        option.transform.SetParent(transform.parent, false);
        EZR.MemorySound.PlaySound("e_click");
    }

    // 打开库存
    public void BtnInventory()
    {
        var inventory = Instantiate(Inventory);
        inventory.transform.SetParent(transform.parent, false);
        EZR.MemorySound.PlaySound("e_click");
    }

    // 过滤歌曲名
    public void InputSearchSongName(string text)
    {
        searchText = text;
        if (!string.IsNullOrEmpty(searchText))
            searchSongName.transform.Find("Clear").gameObject.SetActive(true);
        else
            searchSongName.transform.Find("Clear").gameObject.SetActive(false);

        filterSongs();

        updateSongsListContentHeight();
        songsListScrollRect.verticalNormalizedPosition = 1;

        updateSongUI(true);
    }

    public void BtnClearSearchSongName()
    {
        searchSongName.text = "";
    }
}