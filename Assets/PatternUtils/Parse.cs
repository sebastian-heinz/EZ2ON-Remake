using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace PatternUtils
{
    public partial class Pattern
    {
        public static Pattern Parse(string data)
        {
            Pattern pattern = new Pattern();

            try
            {
                var jobj = JObject.Parse(data);

                pattern.EndTick = (int)jobj["headerData"]["endTick"];
                pattern.BeatsPerMinute = (float)jobj["headerData"]["bpm"];
                // 读取声音列表
                foreach (var sound in jobj["soundList"].Children())
                {
                    pattern.SoundList.Add(new Sound
                    {
                        id = (string)(sound["id"] ?? ""),
                        type = (int)(sound["type"] ?? 0),
                        filename = (string)(sound["filename"] ?? "")
                    });
                }
                // 读取bpm列表
                foreach (var bpm in jobj["bpmList"].Children())
                {
                    pattern.BPMList.Add(new BPM
                    {
                        position = (int)bpm["position"],
                        bpm = (float)bpm["bpm"]
                    });
                }
                if (jobj["beatList"] != null)
                {
                    // 读取Beat列表
                    foreach (var beat in jobj["beatList"].Children())
                    {
                        pattern.BeatList.Add(new Beat
                        {
                            position = (int)beat["position"],
                            beat = (int)beat["beat"]
                        });
                    }
                }
                // 读取轨道列表
                foreach (var track in jobj["trackList"].Children())
                {
                    pattern.TrackList.Add(new Track { name = (string)track["name"] });
                    foreach (var note in track["notes"].Children())
                    {
                        pattern.TrackList[pattern.TrackList.Count - 1].Notes.Add(new Note
                        {
                            position = (int)note["position"],
                            id = (int)note["id"],
                            vol = getVol((int)note["vol"]),
                            pan = getPan((int)note["pan"]),
                            length = (int)note["length"],
                        });
                    }
                }
            }
            catch
            {
                return null;
            }

            return pattern;
        }

        static float getVol(int vol)
        {
            return Mathf.Pow(vol / 127f, 4f);
        }

        static float getPan(int pan)
        {
            float val = pan - 64;
            val = val < 0 ? (val / 64f) : val / 63f;
            return Mathf.Sign(val) * Mathf.Pow(Mathf.Abs(val), 0.5f);
        }
    }
}
