using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

namespace EZR
{
    public static partial class PlayManager
    {
        public static Stopwatch Stopwatch = new Stopwatch();
        public static double DeltaTime = 0d;
        static double lastTime = 0d;

        public static event Action Groove;
        static double beat = 0d;

        public static event Action<string, int> DebugEvent;
        public static event Action LoopStop;

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
                    if (i > 3)
                    {
                        int id = line.Notes[TimeLines.LinesIndex[i]].id;
                        float vol = line.Notes[TimeLines.LinesIndex[i]].vol;
                        float pan = line.Notes[TimeLines.LinesIndex[i]].pan;
                        if (TimeLines.SoundList[line.Notes[TimeLines.LinesIndex[i]].id].type == 1)
                            MemorySound.playSound(id, vol, pan, MemorySound.BGM);
                        else
                            MemorySound.playSound(id, vol, pan, MemorySound.Main);

                        // debug事件
                        if (DebugEvent != null)
                        {
                            DebugEvent(string.Format(
                                "Sound: {0}\n[vol: {1}] [pan:{2}]",
                                TimeLines.SoundList[id].filename,
                                (int)(vol * 100),
                                (int)(pan * 100)
                            ), id);
                        }
                    }

                    TimeLines.LinesIndex[i]++;
                }
            }

            if (isEnd)
            {
                Stop();
                if (LoopStop != null)
                    LoopStop();
                return;
            }

            long now = Stopwatch.ElapsedTicks;
            DeltaTime = (now - lastTime) / 10000000d;
            lastTime = now;

            Position += DeltaTime * ((TimeLines.BPM / 4d / 60d) * PatternUtils.Pattern.MeasureLength);

            beat += DeltaTime * (TimeLines.BPM / 60d);

            // 节奏事件
            if (beat > 1)
            {
                beat -= 1d;
                if (Groove != null)
                    Groove();
            }
        }
    }
}

