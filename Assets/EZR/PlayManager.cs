using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

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
            set { fallSpeed = Mathf.Max(value, 0.25f); }
        }
        public static float RealFallSpeed = FallSpeed;

        static float[] fallSpeedStep = new float[] { 0, 0.25f, 0.5f, 0.75f, 1 };
        public static float[] FallSpeedStep { get => fallSpeedStep; }

        public static GameType GameType = EZR.GameType.EZ2ON;
        public static string SongName = "";
        public static int NumLines = 4;
        public static int MaxLines { get => 8; }
        public static GameMode.Mode GameMode = EZR.GameMode.Mode.RubyMixON;
        public static GameDifficult.Difficult GameDifficult = EZR.GameDifficult.Difficult.EZ;

        public static TimeLines TimeLines;

        public static bool IsAutoPlay = false;
        public static float BGADelay = 0;

        public static int Combo = 0;

        public static Score Score = new Score();

        public static async void LoadPattern()
        {
            string jsonPath = PatternUtils.Pattern.GetFileName(SongName, GameType, GameMode, GameDifficult);
            string zipPath = Path.Combine(EZR.Master.GameResourcesFolder, GameType.ToString(), "Songs", SongName + ".zip");
            var buffer = await ZipLoader.LoadFile(zipPath, jsonPath);
            Debug.Log(jsonPath);
            if (buffer == null)
            {
                throw new System.Exception("JSON file does not exist.");
            }

            var pattern = PatternUtils.Pattern.Parse(Encoding.UTF8.GetString(buffer));
            if (pattern == null) return;
            // 读取所有音频
            EZR.ZipLoader.OpenZip(zipPath);
            for (int i = 0; i < pattern.SoundList.Count; i++)
            {
                string ext = "wav";
                for (int j = 0; j < 3; j++)
                {
                    switch (j)
                    {
                        case 1:
                            ext = "mp3";
                            break;
                        case 2:
                            ext = "ogg";
                            break;
                    }
                    var fileName = Path.ChangeExtension(pattern.SoundList[i].filename, ext);
                    if (ZipLoader.Exists(fileName))
                    {
                        buffer = EZR.ZipLoader.LoadFile(fileName);
                        MemorySound.LoadSound(i, buffer);
                        break;
                    }
                }
            }
            EZR.ZipLoader.CloseZip();

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
                        var note = pattern.TrackList[i].Notes[j];
                        if (note.type == 1)
                        {
                            TimeLines.Lines[mapping].Notes.Add(note);
                            if (mapping <= 7)
                            {
                                if (note.length > 6)
                                {
                                    TimeLines.TotalNote += 1 + note.length / Judgment.LongNoteComboStep;
                                }
                                else
                                    TimeLines.TotalNote++;
                            }
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