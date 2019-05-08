using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using PatternUtils;
using System.IO;

public class DisplayLoop : MonoBehaviour
{
    public string PanelResource = "";
    public string NoteResource = "";

    [HideInInspector]
    public double Position = 0;
    double positionDelta;
    float time = 0;
    bool bgaPlayed = false;

    bool isStarted = false;
    public bool IsStarted { get => isStarted; }

    int readyFrame;

    bool grooveLight = false;
    Animation grooveLightAnim;

    bool[] linesUpdate;
    Animation[] linesAnim;

    Transform header;
    RectTransform noteArea;

    FlareAnimCTL[] flarePlayList;
    FlareAnimCTL[] LongflarePlayList;
    JudgmentAnimCTL judgmentAnim;

    ComboCounter comboCounter;
    int combo = 0;

    Text scoreText;
    Text maxComboText;

    EZR.NoteType.Note[] Notes;

    float noteScale;

    [HideInInspector]
    public int[] CurrentIndex;
    [HideInInspector]
    public Queue<NoteInLine>[] NoteInLines;

    public VideoPlayer VideoPlayer;

    void Start()
    {
        // 初始化面板
        var panel = Instantiate(Resources.Load<GameObject>("Skin/Panel/" + PanelResource));
        panel.transform.SetParent(GameObject.Find("Canvas").transform, false);

        // 初始化音符
        var noteType = Resources.Load<EZR.NoteType>("Skin/Note/" + NoteResource);
        Notes = noteType.Notes;
        noteScale = noteType.NoteScale[EZR.PlayManager.NumLines - 4];
        var target = Instantiate(noteType.Target[EZR.PlayManager.NumLines - 4]);
        target.transform.SetParent(panel.transform.Find("Target"), false);

        // 找节奏灯
        grooveLightAnim = panel.transform.Find("Groove").GetComponent<Animation>();

        // 找按键动画
        linesUpdate = new bool[EZR.PlayManager.NumLines];
        linesAnim = new Animation[EZR.PlayManager.NumLines];
        for (int i = 0; i < EZR.PlayManager.NumLines; i++)
        {
            linesAnim[i] = panel.transform.Find("Lines/" + EZR.PlayManager.NumLines + "/Line" + (i + 1)).GetComponent<Animation>();
        }

        // 选择正确的按键ui
        for (int i = 4; i <= EZR.PlayManager.MaxLines; i++)
        {
            var lines = panel.transform.Find("Lines/" + i).gameObject;
            if (i == EZR.PlayManager.NumLines)
                lines.SetActive(true);
            else
                lines.SetActive(false);
        }

        // 找播放头
        header = panel.transform.Find("NoteArea/Header");
        noteArea = header.parent.GetComponent<RectTransform>();

        // 初始化按键火花特效
        flarePlayList = new FlareAnimCTL[EZR.PlayManager.NumLines];
        LongflarePlayList = new FlareAnimCTL[EZR.PlayManager.NumLines];
        for (int i = 0; i < EZR.PlayManager.NumLines; i++)
        {
            var flare = Instantiate(noteType.Flare);
            var longFlare = Instantiate(panel.GetComponent<Panel>().LongFlare);
            flarePlayList[i] = flare.GetComponent<FlareAnimCTL>();
            LongflarePlayList[i] = longFlare.GetComponent<FlareAnimCTL>();
            flare.transform.SetParent(panel.transform, false);
            longFlare.transform.SetParent(panel.transform, false);
            var panelTarget = panel.transform.Find("Target");
            flare.transform.position = new Vector3(linesAnim[i].transform.position.x, panelTarget.GetChild(0).position.y, 0);
            longFlare.transform.position = new Vector3(linesAnim[i].transform.position.x, panelTarget.GetChild(0).position.y, 0);
        }

        // 初始化判定字动画
        var judgment = Instantiate(panel.GetComponent<Panel>().Judgment);
        judgmentAnim = judgment.GetComponent<JudgmentAnimCTL>();
        judgment.transform.SetParent(panel.transform.Find("Judgment"), false);

        // 找连击计数器
        comboCounter = panel.transform.Find("Combo/ComboCounter").GetComponent<ComboCounter>();
        scoreText = panel.GetComponent<Panel>().ScoreText;
        maxComboText = panel.GetComponent<Panel>().MaxComboText;

        // 生成Lines
        CurrentIndex = new int[EZR.PlayManager.NumLines];
        NoteInLines = new Queue<NoteInLine>[EZR.PlayManager.NumLines];
        for (int i = 0; i < EZR.PlayManager.NumLines; i++)
        {
            NoteInLines[i] = new Queue<NoteInLine>();
        }

        var bgaUrl = Path.Combine(
            EZR.Master.IsDebug ? EZR.Master.GameResourcesFolder : "..\\" + EZR.Master.GameResourcesFolder,
            EZR.PlayManager.GameType.ToString(),
            "Ingame",
            EZR.PlayManager.SongName + ".mp4"
        );
        if (File.Exists(bgaUrl))
            // 初始化BGA
            VideoPlayer.url = bgaUrl;
    }

    void StartPlay()
    {
        EZR.PlayManager.LoopStop += loopStop;
        EZR.PlayManager.Groove += groove;
        EZR.Master.InputEvent += inputEvent;
        EZR.Master.MainLoop += judgmentLoop;
        EZR.PlayManager.Start();

        // 长音符测试
        // EZR.PlayManager.Position = 3100;
        // position = EZR.PlayManager.Position;

        isStarted = true;

        if (EZR.PlayManager.GameType != EZR.GameType.DJMAX &&
        EZR.PlayManager.BGADelay == 0)
        {
            VideoPlayer.Play();
        }
    }

    void loopStop()
    {
        EZR.PlayManager.LoopStop -= loopStop;
        EZR.PlayManager.Groove -= groove;
        EZR.Master.InputEvent -= inputEvent;
        EZR.Master.MainLoop -= judgmentLoop;
    }

    public void Reset()
    {
        loopStop();

        for (int i = 0; i < NoteInLines.Length; i++)
        {
            for (int j = 0; j < NoteInLines[i].Count; j++)
            {
                Destroy(NoteInLines[i].Dequeue().gameObject);
            }
            NoteInLines[i].Clear();
            CurrentIndex[i] = 0;
        }
        EZR.PlayManager.Position = Position = 0;

        readyFrame = 0;
        isStarted = false;

        time = 0;
        bgaPlayed = false;

        VideoPlayer.Stop();
        VideoPlayer.targetTexture.Release();
    }

    public void Stop()
    {
        loopStop();
        EZR.PlayManager.Stop();
    }

    // 表现层循环
    void Update()
    {
        // 等待帧数稳定后开始游戏
        if (!isStarted)
        {
            readyFrame++;
            if (readyFrame > 10)
                StartPlay();
        }

        // 节奏灯
        if (grooveLight)
        {
            grooveLight = false;
            grooveLightAnim["GrooveLight"].time = 0;
            if (!grooveLightAnim.isPlaying)
                grooveLightAnim.Play();
        }

        // 按键表现
        for (int i = 0; i < EZR.PlayManager.NumLines; i++)
        {
            if (linesUpdate[i])
            {
                if (EZR.Master.KeysState[i])
                {
                    linesAnim[i].Play("KeyDown");
                }
                else
                {
                    linesAnim[i]["KeyUp"].time = 0;
                    linesAnim[i].Play("KeyUp");
                }
                linesUpdate[i] = false;
            }
        }

        // 背景动画trigger
        if (EZR.PlayManager.IsPlayBGA)
        {
            EZR.PlayManager.IsPlayBGA = false;
            VideoPlayer.Play();
        }

        // Unity Delta Time Position 用于消除音符抖动
        if (isStarted)
        {
            positionDelta = Time.unscaledDeltaTime * ((EZR.PlayManager.TimeLines.BPM / 4d / 60d) * PatternUtils.Pattern.MeasureLength);
            Position += positionDelta;
            // 消除误差，同步时间轴
            if (System.Math.Abs(Position - EZR.PlayManager.Position) > 1)
            {
                Position = EZR.PlayManager.Position;
            }

            // 记录时间
            time += Time.unscaledDeltaTime;
            if (!bgaPlayed &&
            EZR.PlayManager.BGADelay > 0 && EZR.PlayManager.BGADelay <= time)
            {
                VideoPlayer.Play();
                bgaPlayed = true;
            }
        }

        // 插值下落速度
        EZR.PlayManager.RealFallSpeed = Mathf.Lerp(EZR.PlayManager.RealFallSpeed, EZR.PlayManager.FallSpeed,
            Mathf.Min(Time.unscaledDeltaTime * 12, 1)
        );

        // 播放头
        header.localPosition = new Vector3(0,
            -(float)(Position * EZR.PlayManager.GetSpeed()),
            0
        );

        // 生成实时音符
        for (int i = 0; i < EZR.PlayManager.NumLines; i++)
        {
            while (CurrentIndex[i] < EZR.PlayManager.TimeLines.Lines[i].Notes.Count &&
            EZR.PlayManager.TimeLines.Lines[i].Notes[CurrentIndex[i]].position - Position <
            noteArea.sizeDelta.y / EZR.PlayManager.GetSpeed())
            {
                // 测试长音符
                // if (EZR.PlayManager.TimeLines.Lines[i].Notes[currentIndex[i]].length <= 6)
                // {
                //     currentIndex[i]++;
                //     continue;
                // }

                GameObject note;
                Pattern.Note patternNote = EZR.PlayManager.TimeLines.Lines[i].Notes[CurrentIndex[i]];

                note = Instantiate(Notes[EZR.PlayManager.NumLines - 4].NotePrefab[i]);
                note.transform.SetParent(header, false);
                // 新产生的音符永远在最下层
                note.transform.SetSiblingIndex(0);

                var noteInLine = note.GetComponent<NoteInLine>();
                noteInLine.Init(CurrentIndex[i], patternNote.position, noteScale, patternNote.length, linesAnim[i].transform.localPosition.x);
                NoteInLines[i].Enqueue(noteInLine);

                CurrentIndex[i]++;
            }
        }

        // 长音符和移除音符
        for (int i = 0; i < EZR.PlayManager.NumLines; i++)
        {
            if (NoteInLines[i].Count > 0)
            {
                if (NoteInLines[i].Peek().Position + NoteInLines[i].Peek().NoteLength - EZR.PlayManager.Position < -(EZR.JudgmentDelta.Miss + 1))
                {
                    NoteInLines[i].Dequeue();
                }
            }
        }

        // 连击动画
        if (combo != EZR.PlayManager.Combo)
        {
            if (EZR.PlayManager.Combo == 0)
            {
                comboCounter.Clear();
            }
            else
            {
                combo = EZR.PlayManager.Combo;
                comboCounter.SetCombo(combo);
            }
        }

        // 分数
        scoreText.text = EZR.PlayManager.Score.GetScore().ToString();
        // 最大连击
        maxComboText.text = EZR.PlayManager.Score.MaxCombo.ToString();

        // 修复BGA重复播放问题
        if (!VideoPlayer.isPaused && VideoPlayer.frameCount > 0 && (ulong)VideoPlayer.frame == VideoPlayer.frameCount)
        {
            VideoPlayer.Pause();
        }
    }

    void groove()
    {
        grooveLight = true;
    }

    void judgmentLoop()
    {
        EZR.Judgment.Loop(NoteInLines, judgmentAnim, flarePlayList, LongflarePlayList);
    }

    void inputEvent(int keyId, bool state)
    {
        linesUpdate[keyId] = true;

        EZR.Judgment.InputEvent(state, keyId, NoteInLines, judgmentAnim, flarePlayList, LongflarePlayList);
    }
}
