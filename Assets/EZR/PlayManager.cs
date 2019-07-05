using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EZR
{
    public static partial class PlayManager
    {
        public static double UnscaledPosition = 0;
        public static double Position => UnscaledPosition * JudgmentDelta.MeasureScale;
        static float fallSpeed = 2;
        public static float FallSpeed
        {
            get => fallSpeed;
            set { fallSpeed = Mathf.Max(value, 0.25f); }
        }
        public static float RealFallSpeed = FallSpeed;
        static float speedScale = 1.8f;
        public static float GetSpeed()
        {
            return RealFallSpeed * speedScale;
        }

        static float[] fallSpeedStep = new float[] { 0, 0.25f, 0.5f, 0.75f, 1 };
        public static float[] FallSpeedStep => fallSpeedStep;

        public static GameType GameType = EZR.GameType.EZ2ON;
        public static string SongName = "";
        public static int NumLines = 4;
        public static int MaxLines => 8;
        public static GameMode.Mode GameMode = EZR.GameMode.Mode.RubyMixON;
        public static GameDifficult.Difficult GameDifficult = EZR.GameDifficult.Difficult.EZ;

        public static TimeLines TimeLines;

        public static bool IsAutoPlay = false;
        public static float BGADelay = 0;

        public static int Combo = 0;

        public static Score Score = new Score();

        public static float MaxHp = 50;
        static float hp = MaxHp;
        public static float HP
        {
            get => hp;
            set { hp = Mathf.Clamp(value, 0, MaxHp); }
        }

        public static JudgmentDelta.Mode JudgmentMode = JudgmentDelta.Mode.Normal;

        public static Option.PanelPositionEnum PanelPosition = Option.PanelPositionEnum.Center;
        public static Option.TargetLineTypeEnum TargetLineType = Option.TargetLineTypeEnum.Classic;
        public static int JudgmentOffset = 0;

        public static void LoadPattern()
        {
            string jsonPath = PatternUtils.Pattern.GetFileName(SongName, GameType, GameMode, GameDifficult);
            string zipPath = Path.Combine(Master.GameResourcesFolder, GameType.ToString(), "Songs", SongName + ".zip");
            var buffer = ZipLoader.LoadFileSync(zipPath, jsonPath);
            Debug.Log(jsonPath);
            if (buffer == null)
            {
                throw new System.Exception("JSON file does not exist.");
            }
            // 歌曲特别判定
            if (GameType == GameType.EZ2ON && SongName.ToLower() == "sudden")
            {
                switch (GameDifficult)
                {
                    case EZR.GameDifficult.Difficult.EZ:
                        JudgmentDelta.SetJudgmentDelta(JudgmentDelta.Difficult.SuddenEZ, JudgmentMode);
                        break;
                    case EZR.GameDifficult.Difficult.NM:
                        JudgmentDelta.SetJudgmentDelta(JudgmentDelta.Difficult.SuddenNM, JudgmentMode);
                        break;
                    case EZR.GameDifficult.Difficult.HD:
                        JudgmentDelta.SetJudgmentDelta(JudgmentDelta.Difficult.SuddenHD, JudgmentMode);
                        break;
                    case EZR.GameDifficult.Difficult.SHD:
                        JudgmentDelta.SetJudgmentDelta(JudgmentDelta.Difficult.SuddenSHD, JudgmentMode);
                        break;
                }
            }
            else if (GameType == GameType.EZ2ON && SongName.ToLower() == "tomylove")
            {
                switch (GameDifficult)
                {
                    case EZR.GameDifficult.Difficult.SHD:
                        JudgmentDelta.SetJudgmentDelta(JudgmentDelta.Difficult.SuddenSHD, JudgmentMode);
                        break;
                    default:
                        JudgmentDelta.SetJudgmentDelta(JudgmentDelta.Difficult.Standard, JudgmentMode);
                        break;
                }
            }
            else
            {
                JudgmentDelta.SetJudgmentDelta(JudgmentDelta.Difficult.Standard, JudgmentMode);
            }
            var pattern = PatternUtils.Pattern.Parse(Encoding.UTF8.GetString(buffer));
            if (pattern == null) return;

            // 读取所有音频
            ZipLoader.OpenZip(zipPath);
            for (int i = 0; i < pattern.SoundList.Count; i++)
            {
                var fileName = pattern.SoundList[i].filename;
                for (int j = 0; j < 4; j++)
                {
                    switch (j)
                    {
                        case 1:
                            fileName = Path.ChangeExtension(pattern.SoundList[i].filename, "wav");
                            break;
                        case 2:
                            fileName = Path.ChangeExtension(pattern.SoundList[i].filename, "ogg");
                            break;
                        case 3:
                            fileName = Path.ChangeExtension(pattern.SoundList[i].filename, "mp3");
                            break;
                    }
                    if (ZipLoader.Exists(fileName))
                    {
                        buffer = ZipLoader.LoadFile(fileName);
                        MemorySound.LoadSound(i, buffer);
                        break;
                    }
                }
            }
            ZipLoader.CloseZip();

            // 清空lines
            TimeLines = new TimeLines();
            TimeLines.Clear();

            // 结束tick
            TimeLines.EndTick = pattern.EndTick;
            TimeLines.SoundList = pattern.SoundList;
            // bpm
            TimeLines.BPMList = pattern.BPMList;
            // 映射Lines
            for (int i = 0; i < pattern.TrackList.Count; i++)
            {
                if (pattern.TrackList[i].Notes.Count > 0)
                {
                    int mapping = PatternUtils.Pattern.Mapping(i, GameType, GameMode);

                    for (int j = 0; j < pattern.TrackList[i].Notes.Count; j++)
                    {
                        var note = pattern.TrackList[i].Notes[j];

                        TimeLines.Lines[mapping].Notes.Add(note);
                        if (mapping <= 7)
                        {
                            if (note.length > 6)
                            {
                                TimeLines.TotalNote += note.length / Judgment.LongNoteComboStep;
                            }
                            else
                                TimeLines.TotalNote++;
                        }
                    }
                }
            }
            // 总音符数
            Score.TotalNote = TimeLines.TotalNote;
            Debug.Log("Total note: " + Score.TotalNote);
            // 排序
            TimeLines.SortLines();

            NumLines = EZR.GameMode.GetNumLines(GameMode);

            // 读BGA ini文件 修正bga延迟
            var iniPath = Path.Combine(Master.GameResourcesFolder, GameType.ToString(), "Ingame", SongName + ".ini");
            if (File.Exists(iniPath))
            {
                try
                {
                    BGADelay = System.Convert.ToSingle(File.ReadAllText(iniPath).Split(
                        new string[] { "\r\n", "\n" },
                        System.StringSplitOptions.None
                    )[0].Trim()) / 1000f;
                }
                catch
                {
                    BGADelay = 0;
                }
            }
            else
                BGADelay = 0;
        }

        public static void Reset()
        {
            UnscaledPosition = 0;
            lastTime = 0;
            Stopwatch.Reset();

            beat = 0;
            Score.Reset();
            HP = MaxHp;
            Combo = 0;

            if (TimeLines != null)
                TimeLines.Reset();

            MemorySound.Main.stop();
            MemorySound.BGM.stop();
        }

        public static void AddScore(JudgmentType judgment)
        {
            if (IsAutoPlay) return;
            Score.AddScore(judgment, Combo);
        }

        public static void AddCombo()
        {
            if (IsAutoPlay) return;
            Combo++;
            if (Combo > Score.MaxCombo)
                Score.MaxCombo = Combo;
        }

        public static void ComboBreak()
        {
            Combo = 0;
        }
    }
}