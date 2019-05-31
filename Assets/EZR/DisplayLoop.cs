using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using PatternUtils;
using System.IO;

namespace EZR
{
    public class DisplayLoop : MonoBehaviour
    {
        public string PanelResource = "";
        public string NoteResource = "";

        [HideInInspector]
        public double Position = 0;
        [HideInInspector]
        public double PositionDelta;
        float time = 0;
        bool bgaPlayed = false;

        bool isStarted = false;

        int readyFrame;

        bool grooveLight = false;
        Animation grooveLightAnim;

        RectTransform HP;
        Coroutine hpBeatCoroutine;

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

        NoteType.Note[] Notes;

        GameObject measureLine;
        int measureCount = 0;

        float noteScale;

        int[] currentIndex;
        Queue<NoteInLine>[] noteInLines;

        public VideoPlayer VideoPlayer;

        void Start()
        {
            // 初始化面板
            var panel = Instantiate(Resources.Load<GameObject>("Skin/Panel/" + PanelResource));
            panel.transform.SetParent(GameObject.Find("Canvas").transform, false);

            // 初始化音符
            var noteType = Resources.Load<NoteType>("Skin/Note/" + NoteResource);
            Notes = noteType.Notes;
            noteScale = noteType.NoteScale[PlayManager.NumLines - 4];
            var target = Instantiate(noteType.Target[PlayManager.NumLines - 4]);
            target.transform.SetParent(panel.transform.Find("Target"), false);

            // 节奏线
            measureLine = panel.GetComponent<Panel>().MeasureLine;

            // 找节奏灯
            grooveLightAnim = panel.transform.Find("Groove").GetComponent<Animation>();
            // 找HP
            HP = panel.transform.Find("HP").GetComponent<RectTransform>();

            // 找按键动画
            linesUpdate = new bool[PlayManager.NumLines];
            linesAnim = new Animation[PlayManager.NumLines];
            for (int i = 0; i < PlayManager.NumLines; i++)
            {
                linesAnim[i] = panel.transform.Find("Lines/" + PlayManager.NumLines + "/Line" + (i + 1)).GetComponent<Animation>();
            }

            // 选择正确的按键ui
            for (int i = 4; i <= PlayManager.MaxLines; i++)
            {
                var lines = panel.transform.Find("Lines/" + i).gameObject;
                if (i == PlayManager.NumLines)
                    lines.SetActive(true);
                else
                    lines.SetActive(false);
            }

            // 找播放头
            header = panel.transform.Find("NoteArea/Header");
            noteArea = header.parent.GetComponent<RectTransform>();

            // 初始化按键火花特效
            flarePlayList = new FlareAnimCTL[PlayManager.NumLines];
            LongflarePlayList = new FlareAnimCTL[PlayManager.NumLines];
            for (int i = 0; i < PlayManager.NumLines; i++)
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
            judgmentAnim = panel.transform.Find("Judgment").GetComponent<JudgmentAnimCTL>();

            // 找连击计数器
            comboCounter = panel.transform.Find("Combo/ComboCounter").GetComponent<ComboCounter>();
            scoreText = panel.GetComponent<Panel>().ScoreText;
            maxComboText = panel.GetComponent<Panel>().MaxComboText;

            // 生成Lines
            currentIndex = new int[PlayManager.NumLines];
            noteInLines = new Queue<NoteInLine>[PlayManager.NumLines];
            for (int i = 0; i < PlayManager.NumLines; i++)
            {
                noteInLines[i] = new Queue<NoteInLine>();
            }

            string bgaUrl;
            if (Master.IsDebug)
            {
                bgaUrl = Path.Combine(
                    Master.GameResourcesFolder,
                    PlayManager.GameType.ToString(),
                    "Ingame",
                    PlayManager.SongName + ".mp4"
                );
            }
            else
            {
                bgaUrl = Path.Combine(
                    Application.dataPath,
                    "..",
                    Master.GameResourcesFolder,
                    PlayManager.GameType.ToString(),
                    "Ingame",
                    PlayManager.SongName + ".mp4"
                 );
            }
            if (File.Exists(bgaUrl))
                // 初始化BGA
                VideoPlayer.url = bgaUrl;

            // 找毛玻璃
            var frostedGlass = panel.transform.Find("FrostedGlass").gameObject;
            var option = UserSaveData.GetOption();
            frostedGlass.SetActive(option.FrostedGlassEffect);
        }

        void StartPlay()
        {
            PlayManager.LoopStop += loopStop;
            PlayManager.Groove += groove;
            Master.InputEvent += inputEvent;
            Master.MainLoop += judgmentLoop;
            PlayManager.Start();

            // 长音符测试
            // PlayManager.Position = 3100;
            // position = PlayManager.Position;

            isStarted = true;

            if (!(PlayManager.GameType == GameType.DJMAX &&
            PlayManager.GameMode < EZR.GameMode.Mode.FourKey) &&
            PlayManager.BGADelay == 0)
            {
                VideoPlayer.Play();
            }
        }

        void loopStop()
        {
            PlayManager.LoopStop -= loopStop;
            PlayManager.Groove -= groove;
            Master.InputEvent -= inputEvent;
            Master.MainLoop -= judgmentLoop;
        }

        public void Reset()
        {
            loopStop();

            for (int i = 0; i < noteInLines.Length; i++)
            {
                for (int j = 0; j < noteInLines[i].Count; j++)
                {
                    Destroy(noteInLines[i].Dequeue().gameObject);
                }
                noteInLines[i].Clear();
                currentIndex[i] = 0;
            }
            PlayManager.Position = Position = 0;

            measureCount = 0;

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
            PlayManager.Stop();
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
                grooveLightAnim.Play("GrooveLight");

                // HP跳动
                if (hpBeatCoroutine != null) StopCoroutine(hpBeatCoroutine);
                hpBeatCoroutine = StartCoroutine(hpBeat());

                // 节奏线跳动
                foreach (var measure in MeasureLine.MeasureLines)
                {
                    measure.PlayAnim();
                }
            }

            // 按键表现
            for (int i = 0; i < PlayManager.NumLines; i++)
            {
                if (linesUpdate[i])
                {
                    if (Master.KeysState[i])
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
            if (PlayManager.IsPlayBGA)
            {
                PlayManager.IsPlayBGA = false;
                VideoPlayer.Play();
            }

            // Unity Delta Time Position 用于消除音符抖动
            if (isStarted)
            {
                PositionDelta = Time.unscaledDeltaTime * PlayManager.TickPerSecond;
                Position += PositionDelta;
                // 消除误差，同步时间轴
                if (System.Math.Abs(Position - PlayManager.Position) > 1)
                {
                    Position = PlayManager.Position;
                }

                // 记录时间
                time += Time.unscaledDeltaTime;
                if (!bgaPlayed && !(PlayManager.GameType == GameType.DJMAX &&
                PlayManager.GameMode < EZR.GameMode.Mode.FourKey) &&
                PlayManager.BGADelay > 0 && PlayManager.BGADelay <= time)
                {
                    VideoPlayer.Play();
                    bgaPlayed = true;
                }

                // 算模拟同步垂直延迟
                if (PlayManager.IsSimVsync)
                    PlayManager.SimVsyncDelta = PlayManager.PreSimVsyncDelay * PlayManager.TickPerSecond;
            }

            // 插值下落速度
            PlayManager.RealFallSpeed = Mathf.Lerp(PlayManager.RealFallSpeed, PlayManager.FallSpeed,
                Mathf.Min(Time.unscaledDeltaTime * 12, 1)
            );

            // 播放头
            header.localPosition = new Vector3(0,
                -(float)(Position * PlayManager.GetSpeed()),
                0
            );

            // 生成实时音符
            for (int i = 0; i < PlayManager.NumLines; i++)
            {
                var screenHeight = noteArea.sizeDelta.y / PlayManager.GetSpeed();
                while (currentIndex[i] < PlayManager.TimeLines.Lines[i].Notes.Count &&
                PlayManager.TimeLines.Lines[i].Notes[currentIndex[i]].position - Position < screenHeight)
                {
                    // 测试长音符
                    // if (PlayManager.TimeLines.Lines[i].Notes[currentIndex[i]].length <= 6)
                    // {
                    //     currentIndex[i]++;
                    //     continue;
                    // }
                    var note = Instantiate(Notes[PlayManager.NumLines - 4].NotePrefab[i]);
                    note.transform.SetParent(header, false);
                    // 新产生的音符永远在最下层
                    note.transform.SetSiblingIndex(0);

                    Pattern.Note patternNote = PlayManager.TimeLines.Lines[i].Notes[currentIndex[i]];

                    var noteInLine = note.GetComponent<NoteInLine>();
                    noteInLine.Init(currentIndex[i], patternNote.position, noteScale, patternNote.length, linesAnim[i].transform.localPosition.x, this);
                    noteInLines[i].Enqueue(noteInLine);

                    currentIndex[i]++;
                }
                int currentMeasureCount = (int)((Position + screenHeight) / PatternUtils.Pattern.TickPerMeasure);
                if (currentMeasureCount > measureCount)
                {
                    var measureDelta = currentMeasureCount - measureCount;
                    for (int j = 0; j < measureDelta; j++)
                    {
                        var measureInst = Instantiate(measureLine);
                        measureInst.transform.SetParent(header, false);
                        measureInst.transform.SetSiblingIndex(0);
                        measureInst.GetComponent<MeasureLine>().Index = measureCount + j + 1;
                    }
                    measureCount = currentMeasureCount;
                }
            }

            // 移除长音符音符
            for (int i = 0; i < PlayManager.NumLines; i++)
            {
                if (noteInLines[i].Count > 0)
                {
                    if (noteInLines[i].Peek() == null ||
                    (noteInLines[i].Peek().Position + noteInLines[i].Peek().NoteLength - PlayManager.Position < -(JudgmentDelta.Miss + 1)))
                    {
                        noteInLines[i].Dequeue();
                    }
                }
            }

            // 连击动画
            if (combo != PlayManager.Combo)
            {
                if (PlayManager.Combo == 0)
                {
                    comboCounter.Clear();
                }
                else
                {
                    combo = PlayManager.Combo;
                    comboCounter.SetCombo(combo);
                }
            }

            // 分数
            scoreText.text = PlayManager.Score.GetScore().ToString();
            // 最大连击
            maxComboText.text = PlayManager.Score.MaxCombo.ToString();

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
            Judgment.Loop(noteInLines, judgmentAnim, flarePlayList, LongflarePlayList);
        }

        void inputEvent(int keyId, bool state)
        {
            if (PlayManager.IsAutoPlay) return;
            linesUpdate[keyId] = true;
            Judgment.InputEvent(state, keyId, noteInLines, judgmentAnim, flarePlayList, LongflarePlayList);
        }

        IEnumerator hpBeat()
        {
            for (float i = 0; i < 1; i += Time.unscaledDeltaTime * 2)
            {
                if (i >= 1) i -= 1;
                yield return null;
                HP.localScale = Vector3.Lerp(
                    new Vector3(1, PlayManager.HP / PlayManager.MaxHp + 0.1f, 1),
                    new Vector3(1, PlayManager.HP / PlayManager.MaxHp, 1),
                    Mathf.Sqrt(1 - Mathf.Pow(1 - i, 2))
                );
            }
        }
    }
}
