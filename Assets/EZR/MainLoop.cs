using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace EZR
{
    public static partial class PlayManager
    {
        public static Stopwatch Stopwatch = new Stopwatch();
        public static double DeltaTime = 0d;
        public static string DebugMessage = "";
        static double lastTime = 0d;

        public static void Start()
        {
            Reset();
            Stopwatch.Start();
            lastTime = Stopwatch.ElapsedTicks;
            EZR.Master.MainLoop += MainLoop;
        }

        public static void Stop()
        {
            Reset();
            Stopwatch.Stop();
            TimeLines.Clear();
            EZR.Master.MainLoop -= MainLoop;
        }

        public static void MainLoop()
        {
            bool isEnd = true;

            // bpm
            while (TimeLines.BPMIndex < TimeLines.BPMList.Count && TimeLines.BPMList[TimeLines.BPMIndex].position <= Position)
            {
                TimeLines.BPM = TimeLines.BPMList[TimeLines.BPMIndex].bpm;
                TimeLines.BPMIndex++;
            }

            // 播放音符
            for (int i = 0; i < TimeLines.MaxLines; i++)
            {
                var line = TimeLines.Lines[i];
                if (TimeLines.LinesIndex[i] < line.Notes.Count) isEnd = false;

                while (TimeLines.LinesIndex[i] < line.Notes.Count && line.Notes[TimeLines.LinesIndex[i]].position <= Position)
                {
                    int id = line.Notes[TimeLines.LinesIndex[i]].id;
                    float vol = line.Notes[TimeLines.LinesIndex[i]].vol;
                    float pan = line.Notes[TimeLines.LinesIndex[i]].pan;
                    if (TimeLines.SoundList[line.Notes[TimeLines.LinesIndex[i]].id].type == 1)
                        MemorySound.playSound(id, vol, pan, MemorySound.BGM);
                    else
                        MemorySound.playSound(id, vol, pan, MemorySound.Main);

                    DebugMessage = string.Format(
                        "Sound: {0}\n[vol: {1}] [pan:{2}]",
                        TimeLines.SoundList[id].filename,
                        (int)(vol * 100),
                        (int)(pan * 100)
                    );
                    TestRoom.RoundCount.Add(id);
                    TimeLines.LinesIndex[i]++;
                }
            }

            if (isEnd)
            {
                Stop();
            }

            long now = Stopwatch.ElapsedTicks;
            DeltaTime = (now - lastTime) / 10000000d;
            lastTime = now;

            Position += DeltaTime * ((TimeLines.BPM / 4d / 60d) * PatternUtils.Pattern.MeasureLength);
        }
    }
}
