using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace EZR
{
    public static partial class PlayManager
    {
        public static double Position = 0d;

        public static GameType GameType = EZR.GameType.EZ2ON;
        public static string SongName = "";
        public static int NumLines = 4;
        public static GameMode.Mode GameMode = EZR.GameMode.Mode.RubyMixON;
        public static GameDifficult.Difficult GameDifficult = EZR.GameDifficult.Difficult.EZ;

        public static TimeLines TimeLines;

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
                   EZR.GameMode.GetString(GameMode) + SongName + EZR.GameDifficult.GetString(GameDifficult) +
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
                    SongName + EZR.GameMode.GetString(GameMode) + EZR.GameDifficult.GetString(GameDifficult) +
                     ".json"
                 );
            }
            Debug.Log(jsonPath);

            if (!File.Exists(jsonPath)) return;

            var pattern = PatternUtils.Pattern.Parse(File.ReadAllText(jsonPath));
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
        }

        public static void Reset()
        {
            Position = 0d;
            DeltaTime = 0d;
            lastTime = 0d;
            beat = 0d;
            if (TimeLines != null)
                TimeLines.Reset();
        }
    }
}