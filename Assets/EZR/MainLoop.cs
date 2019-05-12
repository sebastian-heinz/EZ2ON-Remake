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
        public static double PositionDelta = 0d;
        static double lastTime = 0d;

        public static event Action Groove;
        static double beat = 0d;

        public static event Action<string, int> DebugEvent;
        public static event Action LoopStop;
        public static bool IsPlayBGA;

        public static void Start()
        {
            Reset();
            Stopwatch.Start();
            lastTime = Stopwatch.ElapsedTicks;
            Master.MainLoop += MainLoop;
        }

        public static void Stop()
        {
            // Reset();
            Stopwatch.Stop();
            // TimeLines.Clear();
            Master.MainLoop -= MainLoop;
        }

        public static void MainLoop()
        {
            // bpm
            while (TimeLines.BPMIndex < TimeLines.BPMList.Count && TimeLines.BPMList[TimeLines.BPMIndex].position <= Position)
            {
                TimeLines.BPM = TimeLines.BPMList[TimeLines.BPMIndex].bpm;
                TimeLines.BPMIndex++;
            }

            bool isEnd = true;
            // 播放音符
            for (int i = 0; i < TimeLines.MaxLines; i++)
            {
                var line = TimeLines.Lines[i];

                while (TimeLines.LinesIndex[i] < line.Notes.Count && line.Notes[TimeLines.LinesIndex[i]].position <= Position)
                {
                    // 跳过可玩轨道
                    if (i > 7)
                    {
                        var note = line.Notes[TimeLines.LinesIndex[i]];

                        if (PlayManager.GameType == GameType.DJMAX && note.id == 0)
                            IsPlayBGA = true;

                        MemorySound.PlaySound(note.id, note.vol, note.pan, MemorySound.BGM);

                        // debug事件
                        if (DebugEvent != null)
                        {
                            DebugEvent(string.Format(
                                "[{3}] Sound: {0}\n[vol: {1}] [pan:{2}]",
                                TimeLines.SoundList[note.id].filename,
                                (int)(note.vol * 100),
                                (int)(note.pan * 100),
                                (int)Position
                            ), note.id);
                        }
                    }

                    TimeLines.LinesIndex[i]++;
                }

                if (TimeLines.LinesIndex[i] < line.Notes.Count) isEnd = false;
            }

            // 检测结束
            if (isEnd)
            {
                EZR.MemorySound.BGM.isPlaying(out bool isPlaying);
                if (!isPlaying)
                {
                    Stop();
                    if (LoopStop != null)
                        LoopStop();
                    return;
                }
            }

            long now = Stopwatch.ElapsedTicks;
            DeltaTime = (now - lastTime) / 10000000d;
            lastTime = now;

            PositionDelta = DeltaTime * ((TimeLines.BPM / 4d / 60d) * PatternUtils.Pattern.MeasureLength);
            Position += PositionDelta;

            beat += DeltaTime * (TimeLines.BPM / 60d);

            // 节奏事件
            if (beat >= 1)
            {
                beat -= 1;
                if (Groove != null)
                    Groove();
            }
        }
    }
}

