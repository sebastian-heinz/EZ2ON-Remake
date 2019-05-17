using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace EZR
{
    public static class SongsList
    {
        public enum SortMode
        {
            ByName,
            ByDifficult
        }

        public static List<SongInfo> List;
        public static int CurrentIndex = 0;
        public static SortMode CurrentSortMode = SortMode.ByName;
        public static bool IsAscending = true;

        public class SongInfo
        {
            public string name = "";
            public string displayName = "";
            public GameType gameType = GameType.EZ2ON;
            public bool isNew = false;
            public int bpm = 0;
            public Dictionary<GameMode.Mode, Dictionary<GameDifficult.Difficult, int>> difficult;
            public string sha1 = "";
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
                        difficult = new Dictionary<GameMode.Mode, Dictionary<GameDifficult.Difficult, int>>(),
                        sha1 = (string)info["sha1"]
                    };

                    foreach (JProperty difficultInfo in info["difficult"].Children())
                    {
                        var mode = Utils.ParseEnum<GameMode.Mode>(difficultInfo.Name);
                        songInfo.difficult[mode] = new Dictionary<GameDifficult.Difficult, int>();
                        foreach (JProperty difficult in difficultInfo.Value.Children())
                        {
                            songInfo.difficult[mode][Utils.ParseEnum<GameDifficult.Difficult>(difficult.Name)] = (int)difficult.Value;
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
