using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace EZR
{
    public static class SongsList
    {
        public static List<SongInfo> List;
        public static int currentIndex = 0;

        public class SongInfo
        {
            public string name = "";
            public string displayName = "";
            public GameType gameType = GameType.EZ2ON;
            public bool isNew = false;
            public int bpm = 0;
            public Dictionary<GameMode.Mode, Dictionary<GameDifficulty.Difficulty, int>> difficulty;
        }

        public static void Parse(string data)
        {
            try
            {
                var jobj = JObject.Parse(data);

                var version = (string)jobj["version"];

                List = new List<SongInfo>();
                foreach (var info in jobj["list"].Children())
                {
                    var songInfo = new SongInfo
                    {
                        name = (string)info["name"],
                        displayName = (string)info["displayName"],
                        gameType = Utils.ParseEnum<GameType>((string)info["gameType"]),
                        isNew = (bool)info["isNew"],
                        bpm = (int)info["bpm"],
                        difficulty = new Dictionary<GameMode.Mode, Dictionary<GameDifficulty.Difficulty, int>>()
                    };

                    foreach (JProperty difficultyInfo in info["difficulty"].Children())
                    {
                        var mode = Utils.ParseEnum<GameMode.Mode>(difficultyInfo.Name);
                        songInfo.difficulty[mode] = new Dictionary<GameDifficulty.Difficulty, int>();
                        foreach (JProperty difficulty in difficultyInfo.Value.Children())
                        {
                            songInfo.difficulty[mode][Utils.ParseEnum<GameDifficulty.Difficulty>(difficulty.Name)] = (int)difficulty.Value;
                        }
                    }

                    List.Add(songInfo);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message + '\n' + ex.StackTrace);
                List = new List<SongInfo>();
            }
        }
    }
}
