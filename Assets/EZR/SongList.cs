using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace EZR
{
    public static class SongList
    {
        public enum SortMode
        {
            ByName,
            ByDifficult
        }

        public static decimal Version = 1;
        public static string MinVer => "5.0";
        public static List<SongInfo> List;
        public static int CurrentIndex = 0;
        public static SortMode CurrentSortMode = SortMode.ByName;
        public static bool IsAscending = true;

        public class SongInfo
        {
            public string name = "";
            public string displayName = "";
            public string composer = "";
            public GameType gameType = GameType.EZ2ON;
            public bool isNew = false;
            public int bpm = 0;
            public Dictionary<GameMode.Mode, Dictionary<GameDifficult.Difficult, int>> difficult;
            public string sha2 = "";
            public bool isHidden = false;
            public string bgaName = "";

            public GameMode.Mode GetCurrentMode(GameMode.Mode mode, GameDifficult.Difficult diff)
            {
                if (difficult.ContainsKey(mode) && difficult[mode].ContainsKey(diff))
                    return mode;
                else if (difficult.ContainsKey(DJMaxModeMapping(mode)) && difficult[DJMaxModeMapping(mode)].ContainsKey(diff))
                    return DJMaxModeMapping(mode);
                else
                    return GameMode.Mode.None;
            }
        }

        public static EZR.GameMode.Mode DJMaxModeMapping(EZR.GameMode.Mode mode)
        {
            switch (mode)
            {
                case EZR.GameMode.Mode.FourKey:
                    return EZR.GameMode.Mode.FourButton;
                case EZR.GameMode.Mode.FiveKey:
                    return EZR.GameMode.Mode.FiveButton;
                case EZR.GameMode.Mode.SixKey:
                    return EZR.GameMode.Mode.SixButton;
                case EZR.GameMode.Mode.EightKey:
                    return EZR.GameMode.Mode.EightButton;
                default:
                    return EZR.GameMode.Mode.None;
            }
        }

        public static void Parse(string data)
        {
            try
            {
                var jobj = JObject.Parse(data);

                Version = Utils.Version2Decmal((string)jobj["version"]);

                List = new List<SongInfo>();
                foreach (var info in jobj["list"].Children())
                {
                    var composer = "";
                    if (info["composer"].Type == JTokenType.String)
                    {
                        composer = (string)info["composer"];
                    }
                    else if (info["composer"].Type == JTokenType.Array)
                    {
                        foreach (string c in info["composer"])
                        {
                            if (string.IsNullOrEmpty(composer))
                                composer = c;
                            else composer += ", " + c;
                        }
                    }
                    var name = (string)info["name"];
                    var songInfo = new SongInfo
                    {
                        name = name,
                        displayName = (string)info["displayName"],
                        composer = composer,
                        gameType = Utils.ParseEnum<GameType>((string)info["gameType"]),
                        isNew = (bool)(info["isNew"] ?? false),
                        isHidden = (bool)(info["isHidden"] ?? false),
                        bpm = (int)info["bpm"],
                        difficult = new Dictionary<GameMode.Mode, Dictionary<GameDifficult.Difficult, int>>(),
                        sha2 = (string)info["sha2"],
                        bgaName = (string)(info["bgaName"] ?? name)
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
                Debug.LogWarning(ex.Message + '\n' + ex.StackTrace);
                List = new List<SongInfo>();
            }
        }
    }
}
