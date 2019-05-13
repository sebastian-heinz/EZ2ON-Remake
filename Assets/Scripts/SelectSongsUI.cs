using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.IO;
using System.Threading.Tasks;

public class SelectSongsUI : MonoBehaviour
{
    public GameObject SongUI;
    public GameObject Eyecatch;

    Transform songslistContent;
    Transform gameTypeContent;
    List<EZR.SongsList.SongInfo> currentList = new List<EZR.SongsList.SongInfo>();

    EZR.GameType currentType = EZR.PlayManager.GameType;
    EZR.GameMode.Mode currentMode = EZR.PlayManager.GameMode;
    EZR.GameDifficult.Difficult currentDifficult = EZR.PlayManager.GameDifficult;
    int currentSongIndex = EZR.SongsList.currentIndex;
    string currentSongName = EZR.PlayManager.SongName;

    float speed = EZR.PlayManager.FallSpeed;

    bool[] currentDifficultState = new bool[] { false, false, false, false };

    Coroutine delayPlay;
    bool isStreamFirstTime = true;

    void Start()
    {
        songslistContent = transform.Find("PanelSongsList/List/Viewport/Content");
        gameTypeContent = transform.Find("PanelSongsList/ListGameType/Viewport/Content");

        updateDifficultState();
        focusOnBtnDifficult();
        updateGameType();
        updateBtnSpeed();
    }

    // 过滤歌曲
    void filterSongs()
    {
        currentList.Clear();
        foreach (var info in EZR.SongsList.List)
        {
            if (info.difficult.ContainsKey(currentMode) &&
                info.difficult[currentMode].ContainsKey(currentDifficult))
            {
                currentList.Add(info);
            }
        }
    }

    // 统计可用难度
    void updateDifficultState()
    {
        bool ez = false;
        bool nm = false;
        bool hd = false;
        bool shd = false;
        foreach (var info in EZR.SongsList.List)
        {
            if (info.difficult.ContainsKey(currentMode))
            {
                foreach (var difficult in info.difficult[currentMode])
                {
                    if (currentType == EZR.GameType.DJMAX)
                    {
                        switch (difficult.Key)
                        {
                            case EZR.GameDifficult.Difficult.DJMAX_NM:
                                nm = true;
                                break;
                            case EZR.GameDifficult.Difficult.DJMAX_HD:
                                hd = true;
                                break;
                            case EZR.GameDifficult.Difficult.DJMAX_MX:
                                shd = true;
                                break;
                        }
                    }
                    else
                    {
                        switch (difficult.Key)
                        {
                            case EZR.GameDifficult.Difficult.EZ:
                                ez = true;
                                break;
                            case EZR.GameDifficult.Difficult.NM:
                                nm = true;
                                break;
                            case EZR.GameDifficult.Difficult.HD:
                                hd = true;
                                break;
                            case EZR.GameDifficult.Difficult.SHD:
                                shd = true;
                                break;
                        }
                    }
                }
            }
        }

        currentDifficultState = new bool[] { ez, nm, hd, shd };

        if (currentType == EZR.GameType.DJMAX)
        {
            if (!currentDifficultState[(int)currentDifficult - 3])
            {
                for (int i = 0; i < currentDifficultState.Length; i++)
                {
                    if (currentDifficultState[i])
                    {
                        currentDifficult = (EZR.GameDifficult.Difficult)(i + 3);
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

            songUI.GetComponent<EZR.SongUI>().Index = EZR.SongsList.List.IndexOf(info);

            var songName = songUI.transform.Find("SongName").GetComponent<Text>();
            songName.text = info.displayName;

            var difficult = songUI.transform.Find("Difficult").GetComponent<Text>();
            difficult.text = info.difficult[currentMode][currentDifficult].ToString();

            var btn = songUI.transform.Find("Over").GetComponent<Button>();
            btn.onClick.AddListener(BtnSetCurrentSong);

            if (info.name == currentSongName)
            {
                setCurrentName(songUI.transform, "level");
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
            if (currentSongName == "")
                setCurrentName(firstSong, "");
            else
                setCurrentName(firstSong, "change");
            scroll.verticalNormalizedPosition = 1;
        }

        var panelDifficult = transform.Find("PanelDifficult");
        for (int i = 0; i < currentDifficultState.Length; i++)
        {
            var btn = panelDifficult.GetChild(i).GetComponent<Button>();
            btn.interactable = currentDifficultState[i];
            var text = btn.transform.Find("Text").GetComponent<Text>();
            if (currentDifficultState[i])
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

    void sortListByDifficult(bool isAsc)
    {
        currentList.Sort((a, b) => a.difficult[currentMode][currentDifficult].CompareTo(b.difficult[currentMode][currentDifficult]));
        if (!isAsc)
            currentList.Reverse();
    }

    public void BtnSongsName()
    {
        var btn = EventSystem.current.currentSelectedGameObject.GetComponent<EZR.ButtonExtension>();
        if (btn.IsSelected)
        {
            focusOnBtnDifficult("name", true);
            btn.SetSelected(false);
        }
        else
        {
            focusOnBtnDifficult("name", false);
            btn.SetSelected(true);
        }
        EZR.MemorySound.PlaySound("e_click");
    }

    public void BtnSongsDifficult()
    {
        var btn = EventSystem.current.currentSelectedGameObject.GetComponent<EZR.ButtonExtension>();
        if (btn.IsSelected)
        {
            focusOnBtnDifficult("difficult", true);
            btn.SetSelected(false);
        }
        else
        {
            focusOnBtnDifficult("difficult", false);
            btn.SetSelected(true);
        }
        EZR.MemorySound.PlaySound("e_click");
    }

    public void BtnSetGameType()
    {
        var btn = EventSystem.current.currentSelectedGameObject;
        if (btn.GetComponent<EZR.ButtonExtension>().IsSelected) return;
        var diffUI = btn.GetComponent<EZR.GameTypeUI>();

        currentType = diffUI.GameType;
        currentMode = diffUI.GameMode;
        currentDifficult = EZR.GameDifficult.Difficult.EZ;

        if (currentType == EZR.GameType.EZ2ON && (
        currentDifficult != EZR.GameDifficult.Difficult.EZ &&
        currentDifficult != EZR.GameDifficult.Difficult.NM &&
        currentDifficult != EZR.GameDifficult.Difficult.HD &&
        currentDifficult != EZR.GameDifficult.Difficult.SHD
        ))
        {
            currentDifficult = EZR.GameDifficult.Difficult.EZ;
        }
        else if (currentType == EZR.GameType.EZ2ON &&
       currentMode == EZR.GameMode.Mode.ClubMix8 &&
       currentDifficult != EZR.GameDifficult.Difficult.SHD)
        {
            currentDifficult = EZR.GameDifficult.Difficult.SHD;
        }
        else if (currentType == EZR.GameType.EZ2DJ && (
           currentDifficult != EZR.GameDifficult.Difficult.EZ &&
           currentDifficult != EZR.GameDifficult.Difficult.NM &&
           currentDifficult != EZR.GameDifficult.Difficult.HD &&
           currentDifficult != EZR.GameDifficult.Difficult.SHD
        ))
        {
            currentDifficult = EZR.GameDifficult.Difficult.EZ;
        }
        else if (currentType == EZR.GameType.DJMAX && (
           currentDifficult != EZR.GameDifficult.Difficult.DJMAX_NM &&
           currentDifficult != EZR.GameDifficult.Difficult.DJMAX_HD &&
           currentDifficult != EZR.GameDifficult.Difficult.DJMAX_MX
        ))
        {
            currentDifficult = EZR.GameDifficult.Difficult.DJMAX_NM;
        }

        updateDifficultState();
        focusOnBtnDifficult();
        updateGameType();

        foreach (var btn2 in EZR.ButtonExtension.GroupMaster["SongsList"])
        {
            btn2.SetSelected(false);
        }

        EZR.MemorySound.PlaySound("e_page");
    }

    void updateGameType()
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

    void focusOnBtnDifficult()
    {
        int currentDifficultNormal;
        if (currentType == EZR.GameType.DJMAX)
            currentDifficultNormal = (int)currentDifficult - 3;
        else
            currentDifficultNormal = (int)currentDifficult;
        var panelDifficult = transform.Find("PanelDifficult");
        for (int i = currentDifficultNormal; i < currentDifficultState.Length; i++)
        {
            if (currentDifficultState[i])
            {
                var btn = panelDifficult.GetChild(i).GetComponent<EZR.ButtonExtension>();
                updateBtnDifficult(btn, "name", true);
                break;
            }
        }
    }
    void focusOnBtnDifficult(string mode, bool isAsc)
    {
        int currentDifficultNormal;
        if (currentType == EZR.GameType.DJMAX)
            currentDifficultNormal = (int)currentDifficult - 3;
        else
            currentDifficultNormal = (int)currentDifficult;
        var panelDifficult = transform.Find("PanelDifficult");
        for (int i = currentDifficultNormal; i < currentDifficultState.Length; i++)
        {
            if (currentDifficultState[i])
            {
                var btn = panelDifficult.GetChild(i).GetComponent<EZR.ButtonExtension>();
                updateBtnDifficult(btn, mode, isAsc);
                break;
            }
        }
    }

    public void BtnSetDifficult()
    {
        var btn = EventSystem.current.currentSelectedGameObject.GetComponent<EZR.ButtonExtension>();
        if (btn.IsSelected) return;
        switch (btn.gameObject.name)
        {
            case "BtnEZ":
                if (currentType == EZR.GameType.DJMAX)
                    return;
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
        }
        updateDifficultState();
        updateBtnDifficult(btn, "name", true);

        foreach (var btn2 in EZR.ButtonExtension.GroupMaster["SongsList"])
        {
            btn2.SetSelected(false);
        }

        EZR.MemorySound.PlaySound("e_level");
    }

    public Color EzColor = Color.white;
    public Color NmColor = Color.white;
    public Color HdColor = Color.white;
    public Color ShdColor = Color.white;
    public Color UnselectedColor = Color.white;
    public Color DisabledColor = Color.white;
    void updateBtnDifficult(EZR.ButtonExtension btn, string mode, bool isAsc)
    {
        filterSongs();
        switch (mode)
        {
            case "name":
                sortListByName(isAsc);
                break;
            case "difficult":
                sortListByDifficult(isAsc);
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
        var btn = EventSystem.current.currentSelectedGameObject.GetComponent<EZR.ButtonExtension>();
        if (btn.IsSelected) return;
        setCurrentName(btn.transform.parent, "change");
        EZR.MemorySound.PlaySound("e_music");
    }

    async void setCurrentName(Transform songUI, string state)
    {
        currentSongIndex = songUI.GetComponent<EZR.SongUI>().Index;
        var info = EZR.SongsList.List[currentSongIndex];

        var isSameSong = false;
        if (isStreamFirstTime)
            isStreamFirstTime = false;
        else if (currentSongName == info.name)
            isSameSong = true;

        currentSongName = info.name;
        var btn2 = songUI.Find("Over").GetComponent<EZR.ButtonExtension>();
        btn2.SetSelected(true);

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
                fileName = "big_" + currentSongName + EZR.GameDifficult.GetString(currentDifficult) + ".png";
                break;
            case EZR.GameType.EZ2DJ:
                fileName = currentSongName + EZR.GameDifficult.GetString(currentDifficult) + ".bmp";
                break;
            case EZR.GameType.DJMAX:
                fileName = "song_pic_f_" + currentSongName + "_" + ((int)currentDifficult - 3).ToString().PadLeft(2, '0') + ".png";
                break;
        }

        var buffer = await EZR.ZipLoader.LoadFile(Path.Combine(EZR.Master.GameResourcesFolder, currentType.ToString(), "Songs", currentSongName + ".zip"), fileName);
        if (buffer != null)
        {
            disc.Find("Image").GetComponent<RawImage>().texture = EZR.ImageLoader.Load(buffer, fileName);
        }

        transform.Find("SongName").GetComponent<Text>().text = info.displayName.ToUpper();
        transform.Find("Bpm/Text").GetComponent<Text>().text = info.bpm.ToString().PadLeft(3, '0');
        transform.Find("Level/Text").GetComponent<Text>().text = info.difficult[currentMode][currentDifficult].ToString();

        // 播放预览音乐
        if (!isSameSong)
        {
            if (delayPlay != null) StopCoroutine(delayPlay);
            EZR.MemorySound.StopSound();
            delayPlay = StartCoroutine(DelayPlayStream());
        }

        transform.Find("MyBestScore/Text").GetComponent<Text>().text = EZR.UserSaveData.GetScore(currentSongName, currentType, currentMode, currentDifficult).ToString();
    }

    IEnumerator DelayPlayStream()
    {
        yield return new WaitForSecondsRealtime(0.4f);

        string fileName = "";
        switch (currentType)
        {
            case EZR.GameType.EZ2ON:
                fileName = "preview_" + currentSongName + ".ogg";
                break;
            case EZR.GameType.EZ2DJ:
                fileName = "preview_" + currentSongName + ".wav";
                break;
            case EZR.GameType.DJMAX:
                fileName = currentSongName + ".wav";
                break;
        }
        playPreview(fileName);
    }

    async void playPreview(string fileName)
    {
        var buffer = await EZR.ZipLoader.LoadFile(Path.Combine(EZR.Master.GameResourcesFolder, currentType.ToString(), "Songs", currentSongName + ".zip"), fileName);
        if (buffer != null)
        {
            EZR.MemorySound.PlaySound(buffer, true);
        }
    }

    public async void BtnStart()
    {
        var btn = transform.Find("BtnStart").GetComponent<Button>();
        if (!btn.interactable) return;
        else btn.interactable = false;

        var zipPath = Path.Combine(EZR.Master.GameResourcesFolder, currentType.ToString(), "Songs", currentSongName + ".zip");
        // 检查sha1
        if (File.Exists(zipPath))
        {
            var zipBuffer = new byte[0];
            await Task.Run(async () =>
            {
                using (var stream = File.OpenRead(zipPath))
                {
                    zipBuffer = new byte[stream.Length];
                    await stream.ReadAsync(zipBuffer, 0, (int)stream.Length);
                }
            });
            var sha1 = await EZR.Hash.Sha1(zipBuffer);

            if (sha1.ToLower() != EZR.SongsList.List[currentSongIndex].sha1.ToLower())
            {
                var messageBox = Instantiate(EZR.Master.MessageBox);
                messageBox.transform.SetParent(transform.parent, false);
                messageBox.GetComponent<EZR.MessageBox>().Text = "歌曲包完整性校验失败，歌曲包可能被修改或损坏！";
                btn.interactable = true;
                return;
            }
        }
        else
        {
            var messageBox = Instantiate(EZR.Master.MessageBox);
            messageBox.transform.SetParent(transform.parent, false);
            messageBox.GetComponent<EZR.MessageBox>().Text = "缺少歌曲包！";
            btn.interactable = true;
            return;
        }
        // 检查json文件是否存在
        var jsonFileName = PatternUtils.Pattern.GetFileName(currentSongName, currentType, currentMode, currentDifficult);
        if (!EZR.ZipLoader.Exists(zipPath, jsonFileName))
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
        var buffer = await EZR.ZipLoader.LoadFile(zipPath, fileName);

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
        EZR.SongsList.currentIndex = currentSongIndex;
        EZR.PlayManager.SongName = currentSongName;
        EZR.PlayManager.GameMode = currentMode;
        EZR.PlayManager.GameDifficult = currentDifficult;
        EZR.PlayManager.FallSpeed = speed;

        EZR.MemorySound.StopSound();
        if (delayPlay != null) StopCoroutine(delayPlay);
        EZR.MemorySound.PlaySound("e_start");
    }

    IEnumerator startPlay(Animation anim)
    {
        while (anim.isPlaying)
        {
            yield return null;
        }
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
        yield return new WaitForSecondsRealtime(0.2f);
        for (; ; )
        {
            yield return new WaitForSecondsRealtime(0.075f);
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
}
