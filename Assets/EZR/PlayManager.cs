using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace EZR
{
    public static partial class PlayManager
    {
        public static double Position = 0d;
        public static float MeasureScale = 2;
        static float fallSpeed = 2;
        public static float FallSpeed
        {
            get => fallSpeed;
            set
            {
                if (value >= 0.25f)
                    fallSpeed = value;
                else
                    fallSpeed = 0.25f;
            }
        }
        public static float RealFallSpeed = FallSpeed;

        public static GameType GameType = EZR.GameType.EZ2ON;
        public static string SongName = "";
        public static int NumLines = 4;
        public static int MaxLines { get => 8; }
        public static GameMode.Mode GameMode = EZR.GameMode.Mode.RubyMixON;
        public static GameDifficulty.Difficulty GameDifficult = EZR.GameDifficulty.Difficulty.EZ;

        public static TimeLines TimeLines;

        public static bool IsAutoPlay = false;
        public static float BGADelay = 0;

        public static int Combo = 0;

        public static Score Score = new Score();

        public static void LoadPattern()
        {
            string jsonPath;
            if (GameType != EZR.GameType.DJMAX)
            {
                jsonPath = Path.Combine(
                   EZR.Master.GameResourcesFolder,
                   GameType.ToString(),
                   "Songs",
                   SongName,
                   EZR.GameMode.GetString(GameMode) + SongName + EZR.GameDifficulty.GetString(GameDifficult) +
                   ".json"
               );
            }
            else
            {
                jsonPath = Path.Combine(
                     EZR.Master.GameResourcesFolder,
                     GameType.ToString(),
                     "Songs",
                     SongName,
                    SongName + EZR.GameMode.GetString(GameMode) + EZR.GameDifficulty.GetString(GameDifficult) +
                     ".json"
                 );
            }
            Debug.Log(jsonPath);

            if (!File.Exists(jsonPath))
            {
                // TODO 曲谱不存在需要处理
                return;
            }

            var pattern = PatternUtils.Pattern.Parse(File.ReadAllText(jsonPath));
            if (pattern == null) return;
            // 读取所有音频
            for (int i = 0; i < pattern.SoundList.Count; i++)
            {
                string extName = "wav";
                for (int j = 0; j < 3; j++)
                {
                    switch (j)
                    {
                        case 1:
                            extName = "mp3";
                            break;
                        case 2:
                            extName = "ogg";
                            break;
                    }
                    string soundPath = Path.Combine(
                        EZR.Master.GameResourcesFolder,
                        GameType.ToString(),
                        "Songs",
                        SongName,
                        Path.ChangeExtension(pattern.SoundList[i].filename, extName)
                    );
                    if (File.Exists(soundPath))
                    {
                        MemorySound.LoadSound(i, File.ReadAllBytes(soundPath));
                        break;
                    };
                }
            }

            // 清空lines
            TimeLines = new TimeLines();
            TimeLines.Clear();

            TimeLines.SoundList = pattern.SoundList;
            // bpm
            foreach (var bpm in pattern.BPMList)
            {
                if (bpm.type == 3)
                {
                    TimeLines.BPMList.Add(bpm);
                }
            }
            // 映射Lines
            for (int i = 0; i < pattern.TrackList.Count; i++)
            {
                if (pattern.TrackList[i].Notes.Count > 0)
                {
                    int mapping;
                    if (GameType == EZR.GameType.DJMAX &&
                    GameMode != EZR.GameMode.Mode.EightButtons)
                    {
                        mapping = PatternUtils.Pattern.Mapping(GameType, i) - 1;
                        if (mapping == 7) mapping = 8;
                    }
                    else
                    {
                        mapping = PatternUtils.Pattern.Mapping(GameType, i);
                    }

                    for (int j = 0; j < pattern.TrackList[i].Notes.Count; j++)
                    {
                        if (pattern.TrackList[i].Notes[j].type == 1)
                        {
                            TimeLines.Lines[mapping].Notes.Add(pattern.TrackList[i].Notes[j]);
                        }
                    }
                }
            }
            // 排序
            TimeLines.SortLines();

            NumLines = EZR.GameMode.GetNumLines(GameMode);

            // 读BGA ini文件 修正bga延迟
            if (GameType == GameType.EZ2ON)
            {
                var iniPath = Path.Combine(EZR.Master.GameResourcesFolder, GameType.ToString(), "Ingame", SongName + ".ini");
                if (File.Exists(iniPath))
                {
                    try
                    {
                        BGADelay = System.Convert.ToInt32(File.ReadAllText(iniPath)) / 1000f;
                    }
                    catch
                    {
                        BGADelay = 0;
                    }
                }
                else
                    BGADelay = 0;
            }
            else
                BGADelay = 0;
        }

        public static void Reset()
        {
            Position = 0d;
            lastTime = 0d;
            Stopwatch.Restart();

            beat = 0d;
            Score.Reset();
            Combo = 0;

            if (TimeLines != null)
                TimeLines.Reset();

            MemorySound.Main.stop();
            MemorySound.BGM.stop();
        }

        public static float GetSpeed()
        {
            return MeasureScale * RealFallSpeed;
        }

        // 分数公式
        public static void AddScore(JudgmentType judgment)
        {
            // if (IsAutoPlay) return;
            switch (judgment)
            {
                case JudgmentType.Kool:
                    Score.Kool++;
                    Score.RawScore += 170 + 17 * Mathf.Log(Combo, 2);
                    break;
                case JudgmentType.Cool:
                    Score.Cool++;
                    Score.RawScore += 100 + 10 * Mathf.Log(Combo, 2);
                    break;
                case JudgmentType.Good:
                    Score.Good++;
                    Score.RawScore += 40 + 4 * Mathf.Log(Combo, 2);
                    break;
                case JudgmentType.Miss:
                    Score.Miss++;
                    break;
                case JudgmentType.Fail:
                    Score.Fail++;
                    break;
            }
        }
        public static void AddCombo()
        {
            Combo++;
            // if (IsAutoPlay) return;
            if (Combo > Score.MaxCombo)
                Score.MaxCombo = Combo;
        }

        public static void ComboBreak()
        {
            Combo = 0;
        }
    }
}