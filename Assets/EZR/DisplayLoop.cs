using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using PatternUtils;
using System.IO;
using RenderHeads.Media.AVProVideo;

namespace EZR
{
    public class DisplayLoop : MonoBehaviour
    {
        public static string PanelResource = "R14";
        public static string NoteResource = "Note_04";

        double position = 0;
        public double Position => position * JudgmentDelta.MeasureScale;
        [HideInInspector]
        public double PositionDelta;
        float time = 0;
        bool bgaPlayed = false;

        bool isStarted = false;

        int readyFrame;

        bool grooveDisplay = false;
        Animation grooveLightAnim;

        RectTransform HP;
        float HPHeight;
        Coroutine hpBeatCoroutine;

        bool[] linesUpdate;
        Animation[] linesAnim;

        RectTransform noteArea;

        FlareAnimCTL[] flarePlayList;
        FlareAnimCTL[] longflarePlayList;
        JudgmentAnimCTL judgmentAnim;

        ComboCounter comboCounter;
        int combo = 0;

        Text scoreText;
        Text maxComboText;

        NoteType.Note[] notes;
        Animation noteTargetAnim;

        GameObject measureLine;
        int measureCount = 0;

        [HideInInspector]
        public bool NoteUseScale = false;
        [HideInInspector]
        public float NoteSize;

        int[] currentIndex;
        Queue<NoteInLine>[] noteInLines;
        public ObjectPool notePoolA;
        public ObjectPool notePoolB;
        public ObjectPool notePoolC;
        public ObjectPool measureLinePool;

        MediaPlayer mediaPlayer;
        DisplayUGUI displayUGUI;

        GameObject autoObj;

        void Start()
        {
            // 读取设置
            var option = UserSaveData.GetOption();
            PlayManager.PanelPosition = option.PanelPosition;
            PlayManager.TargetLineType = option.TargetLineType;
            PlayManager.JudgmentOffset = option.JudgmentOffset;

            // 初始化面板
            var panel = Instantiate(Resources.Load<GameObject>("Skin/Panel/" + PanelResource));
            panel.transform.SetParent(GameObject.Find("Canvas").transform, false);
            // 面板位置
            panel.transform.localPosition = new Vector3(
                (int)PlayManager.PanelPosition,
                panel.transform.localPosition.y,
                0
            );

            // 初始化音符
            var noteType = Resources.Load<NoteType>("Skin/Note/" + NoteResource);
            notes = noteType.Notes;
            NoteUseScale = noteType.UseScale;
            NoteSize = noteType.NoteSize[PlayManager.NumLines - 4];
            var target = Instantiate(noteType.Target[PlayManager.NumLines - 4]);
            target.transform.SetParent(panel.transform.Find("Target"), false);
            noteTargetAnim = target.GetComponent<Animation>();

            // 判定线偏移
            target.transform.parent.localPosition = new Vector3(
                target.transform.parent.localPosition.x,
                target.transform.parent.localPosition.y + (int)PlayManager.TargetLineType,
                0);

            // 找音符节点
            noteArea = (RectTransform)panel.transform.Find("NoteArea");

            // 节奏线
            measureLine = panel.GetComponent<Panel>().MeasureLine;

            // 初始化对象池
            for (int i = 0; i < PlayManager.NumLines; i++)
            {
                switch (notes[PlayManager.NumLines - 4].NotePrefab[i].GetComponent<NoteInLine>().Type)
                {
                    case NoteInLine.NoteType.A:
                        if (notePoolA == null)
                        {
                            notePoolA = new ObjectPool();
                            for (int j = 0; j < 20; j++)
                            {
                                var note = Instantiate(notes[PlayManager.NumLines - 4].NotePrefab[i]);
                                note.transform.SetParent(noteArea, false);
                                note.GetComponent<NoteInLine>().Recycle(notePoolA);
                            }
                        }
                        break;
                    case NoteInLine.NoteType.B:
                        if (notePoolB == null)
                        {
                            notePoolB = new ObjectPool();
                            for (int j = 0; j < 15; j++)
                            {
                                var note = Instantiate(notes[PlayManager.NumLines - 4].NotePrefab[i]);
                                note.transform.SetParent(noteArea, false);
                                note.GetComponent<NoteInLine>().Recycle(notePoolB);
                            }
                        }
                        break;
                    case NoteInLine.NoteType.C:
                        if (notePoolC == null)
                        {
                            notePoolC = new ObjectPool();
                            for (int j = 0; j < 10; j++)
                            {
                                var note = Instantiate(notes[PlayManager.NumLines - 4].NotePrefab[i]);
                                note.transform.SetParent(noteArea, false);
                                note.GetComponent<NoteInLine>().Recycle(notePoolC);
                            }
                        }
                        break;
                    default: continue;
                }
            }
            if (measureLinePool == null)
            {
                measureLinePool = new ObjectPool();
                for (int i = 0; i < 2; i++)
                {
                    var line = Instantiate(measureLine);
                    line.transform.SetParent(noteArea, false);
                    line.GetComponent<MeasureLine>().Recycle(measureLinePool);
                }
            }

            // 找节奏灯
            grooveLightAnim = panel.transform.Find("Groove").GetComponent<Animation>();
            // 找HP
            HP = (RectTransform)panel.transform.Find("HP");
            HPHeight = HP.sizeDelta.y;

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

            // 初始化按键火花特效
            flarePlayList = new FlareAnimCTL[PlayManager.NumLines];
            longflarePlayList = new FlareAnimCTL[PlayManager.NumLines];
            for (int i = 0; i < PlayManager.NumLines; i++)
            {
                var flare = Instantiate(noteType.Flare);
                var longFlare = Instantiate(panel.GetComponent<Panel>().LongFlare);
                flarePlayList[i] = flare.GetComponent<FlareAnimCTL>();
                longflarePlayList[i] = longFlare.GetComponent<FlareAnimCTL>();
                flare.transform.SetParent(panel.transform, false);
                longFlare.transform.SetParent(panel.transform, false);
                var panelTarget = panel.transform.Find("Target");
                flare.transform.position = new Vector3(linesAnim[i].transform.position.x, panelTarget.GetChild(0).position.y, 0);
                longFlare.transform.position = new Vector3(linesAnim[i].transform.position.x, panelTarget.GetChild(0).position.y, 0);
            }

            // 初始化判定字动画
            judgmentAnim = panel.transform.Find("Judgment").GetComponent<JudgmentAnimCTL>();
            judgmentAnim.transform.Find("FastSlow").gameObject.SetActive(option.ShowFastSlow);

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

            mediaPlayer = GameObject.Find("MediaPlayer").GetComponent<MediaPlayer>();
            displayUGUI = GameObject.Find("Canvas").transform.Find("BGA").GetComponent<DisplayUGUI>();

            string bgaUrl;
            if (PlayManager.GameType == GameType.EZ2ON || PlayManager.GameType == GameType.EZ2DJ)
                bgaUrl = Path.Combine(
                    Master.GameResourcesFolder,
                    "EZ2Series", "Ingame",
                    SongList.List[SongList.CurrentIndex].bgaName + ".mp4"
                );
            else
                bgaUrl = Path.Combine(
                    Master.GameResourcesFolder,
                    PlayManager.GameType.ToString(),
                    "Ingame",
                    SongList.List[SongList.CurrentIndex].bgaName + ".mp4"
                );
            string genericBgaUrl = Path.Combine(Master.GameResourcesFolder, "GenericBGA.mp4");
            if (File.Exists(bgaUrl))
            {
                // 初始化BGA
                if (Master.IsOldWin)
                    mediaPlayer.PlatformOptionsWindows.videoApi = Windows.VideoApi.DirectShow;

                mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, bgaUrl, false);
            }
            else if (File.Exists(genericBgaUrl))
            {
                // fallback通用bga
                if (Master.IsOldWin)
                    mediaPlayer.PlatformOptionsWindows.videoApi = Windows.VideoApi.DirectShow;

                mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, genericBgaUrl, false);
                mediaPlayer.Control.SetLooping(true);
            }
            else
            {
                Destroy(mediaPlayer);
                Destroy(displayUGUI);
            }

            // 找毛玻璃
            var frostedGlass = panel.transform.Find("FrostedGlass").gameObject;
            frostedGlass.SetActive(option.FrostedGlassEffect);

            // 找 auto play 对象
            autoObj = panel.transform.Find("Auto").gameObject;
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
            PlayManager.BGADelay <= 0)
            {
                if (mediaPlayer != null && displayUGUI != null)
                {
                    mediaPlayer.Control.Play();
                    displayUGUI.color = Color.white;
                    mediaPlayer.Control.Seek(-PlayManager.BGADelay * 1000);
                }
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
                    noteInLines[i].Dequeue().Recycle();
                }
                noteInLines[i].Clear();
                currentIndex[i] = 0;
            }
            PlayManager.UnscaledPosition = position = 0;

            measureCount = 0;

            readyFrame = 0;
            isStarted = false;

            time = 0;
            bgaPlayed = false;

            if (mediaPlayer != null)
                mediaPlayer.Control.Pause();
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
                {
                    if (mediaPlayer != null && displayUGUI != null)
                    {
                        if (mediaPlayer.VideoOpened) StartPlay();
                    }
                    else StartPlay();
                }
            }


            if (grooveDisplay)
            {
                grooveDisplay = false;

                // 消除误差，同步时间轴
                if (System.Math.Abs(position - PlayManager.UnscaledPosition) > 1)
                {
                    position = PlayManager.UnscaledPosition;
                }

                // 节奏灯
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

                // 音符目标跳动
                noteTargetAnim[noteTargetAnim.clip.name].time = 0;
                noteTargetAnim.Play();
            }

            // auto play滚屏效果
            if (PlayManager.IsAutoPlay != autoObj.activeSelf)
            {
                autoObj.SetActive(PlayManager.IsAutoPlay);
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
                if (mediaPlayer != null && displayUGUI != null)
                {
                    mediaPlayer.Control.Play();
                    displayUGUI.color = Color.white;
                }
            }

            // Unity smoothDeltaTime计算Position 用于消除音符抖动
            if (isStarted)
            {
                PositionDelta = Time.smoothDeltaTime * PlayManager.TickPerSecond;
                position += PositionDelta;

                // 记录时间
                time += Time.deltaTime;
                if (!bgaPlayed && !(PlayManager.GameType == GameType.DJMAX &&
                PlayManager.GameMode < EZR.GameMode.Mode.FourKey) &&
                PlayManager.BGADelay > 0 && PlayManager.BGADelay <= time)
                {
                    if (mediaPlayer != null && displayUGUI != null)
                    {
                        mediaPlayer.Control.Play();
                        displayUGUI.color = Color.white;
                    }
                    bgaPlayed = true;
                }
            }

            // 插值下落速度
            PlayManager.RealFallSpeed = Mathf.Lerp(PlayManager.RealFallSpeed, PlayManager.FallSpeed,
                Mathf.Min(Time.deltaTime * 12, 1)
            );

            var screenHeight = (noteArea.sizeDelta.y +
                (PlayManager.IsAutoPlay ? 0 : PlayManager.JudgmentOffset)) /
                PlayManager.GetSpeed() / JudgmentDelta.MeasureScale;
            // 生成实时音符
            for (int i = 0; i < PlayManager.NumLines; i++)
            {
                while (currentIndex[i] < PlayManager.TimeLine.Lines[i].Notes.Count &&
                PlayManager.TimeLine.Lines[i].Notes[currentIndex[i]].position - position < screenHeight)
                {
                    // 测试长音符
                    // if (PlayManager.TimeLines.Lines[i].Notes[currentIndex[i]].length <= 6)
                    // {
                    //     currentIndex[i]++;
                    //     continue;
                    // }

                    // 选择对象池;
                    ObjectPool pool;
                    switch (notes[PlayManager.NumLines - 4].NotePrefab[i].GetComponent<NoteInLine>().Type)
                    {
                        case NoteInLine.NoteType.A:
                            pool = notePoolA;
                            break;
                        case NoteInLine.NoteType.B:
                            pool = notePoolB;
                            break;
                        case NoteInLine.NoteType.C:
                            pool = notePoolC;
                            break;
                        default:
                            currentIndex[i]++;
                            continue;
                    }
                    GameObject note;
                    if (pool.Count == 0)
                    {
                        note = Instantiate(notes[PlayManager.NumLines - 4].NotePrefab[i]);
                    }
                    else
                        note = pool.Get();

                    note.transform.SetParent(noteArea, false);
                    // 新产生的音符永远在最下层
                    note.transform.SetSiblingIndex(0);

                    Pattern.Note patternNote = PlayManager.TimeLine.Lines[i].Notes[currentIndex[i]];

                    var noteInLine = note.GetComponent<NoteInLine>();
                    noteInLine.Init(currentIndex[i], patternNote.position, patternNote.length, linesAnim[i].transform.localPosition.x, this, pool);
                    noteInLines[i].Enqueue(noteInLine);

                    currentIndex[i]++;
                }
            }
            // 生成节奏线
            int currentMeasureCount = (int)((position + screenHeight) / (PatternUtils.Pattern.TickPerMeasure * (PlayManager.TimeLine.Beat * 0.25f)));
            if (currentMeasureCount > measureCount)
            {
                var measureDelta = currentMeasureCount - measureCount;
                for (int j = 0; j < measureDelta; j++)
                {
                    GameObject measureLineInstance;
                    if (measureLinePool.Count == 0)
                    {
                        measureLineInstance = Instantiate(measureLine);
                    }
                    else
                        measureLineInstance = measureLinePool.Get();

                    measureLineInstance.transform.SetParent(noteArea, false);
                    measureLineInstance.transform.SetSiblingIndex(0);
                    var measureLineComponent = measureLineInstance.GetComponent<MeasureLine>();
                    measureLineComponent.Init(measureCount + j + 1, this, measureLinePool);
                }
                measureCount = currentMeasureCount;
            }

            // 移除长音符音符
            for (int i = 0; i < PlayManager.NumLines; i++)
            {
                if (noteInLines[i].Count > 0)
                {
                    var noteInLine = noteInLines[i].Peek();
                    if (noteInLine == null ||
                    noteInLine.Position + noteInLine.NoteLength - PlayManager.Position < -(JudgmentDelta.Miss + 1))
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
            scoreText.text = Mathf.Round(PlayManager.Score.RawScore).ToString();
            // 最大连击
            maxComboText.text = PlayManager.Score.MaxCombo.ToString();
        }

        void groove()
        {
            grooveDisplay = true;
        }

        void judgmentLoop()
        {
            Judgment.Loop(noteInLines, judgmentAnim, flarePlayList, longflarePlayList);
        }

        void inputEvent(int keyId, bool state)
        {
            if (PlayManager.IsAutoPlay) return;
            linesUpdate[keyId] = true;
            Judgment.InputEvent(state, keyId, noteInLines, judgmentAnim, flarePlayList, longflarePlayList);
        }

        IEnumerator hpBeat()
        {
            for (float i = 0; i < 1; i += Time.deltaTime * 2)
            {
                if (i >= 1) i -= 1;
                yield return null;
                HP.sizeDelta = Vector2.Lerp(
                    new Vector2(HP.sizeDelta.x, (PlayManager.HP / PlayManager.MaxHp + 0.1f) * HPHeight),
                    new Vector2(HP.sizeDelta.x, (PlayManager.HP / PlayManager.MaxHp) * HPHeight),
                    Mathf.Sqrt(1 - Mathf.Pow(1 - i, 2))
                );
            }
        }
    }
}
