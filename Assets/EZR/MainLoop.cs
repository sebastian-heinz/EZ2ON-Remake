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
        public static double DeltaTime = 0;
        public static double PositionDelta = 0;
        public static double TickPerSecond = 0;
        static double lastTime = 0;

        public static event Action Groove;
        static double beat = 0;

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
            while (TimeLine.BPMIndex < TimeLine.BPMList.Count && TimeLine.BPMList[TimeLine.BPMIndex].position <= UnscaledPosition)
            {
                TimeLine.BPM = TimeLine.BPMList[TimeLine.BPMIndex].bpm;
                TimeLine.BPMIndex++;
            }
            // Beat
            while (TimeLine.BeatIndex < TimeLine.BeatList.Count && TimeLine.BeatList[TimeLine.BeatIndex].position <= UnscaledPosition)
            {
                TimeLine.Beat = TimeLine.BeatList[TimeLine.BeatIndex].beat;
                TimeLine.BeatIndex++;
            }

            // 播放音符
            for (int i = 0; i < TimeLine.MaxLines; i++)
            {
                var line = TimeLine.Lines[i];

                while (TimeLine.LinesIndex[i] < line.Notes.Count && line.Notes[TimeLine.LinesIndex[i]].position <= UnscaledPosition)
                {
                    // 跳过可玩轨道
                    if (i > 7)
                    {
                        var note = line.Notes[TimeLine.LinesIndex[i]];

                        if (PlayManager.GameType == GameType.DJMAX &&
                        PlayManager.GameMode < EZR.GameMode.Mode.FourKey &&
                        note.id == 0)
                            IsPlayBGA = true;

                        MemorySound.PlaySound(note.id, note.volume, note.pan, MemorySound.BGM);

                        // debug事件
                        if (DebugEvent != null)
                        {
                            DebugEvent(string.Format(
                                "[{3}] Sound: {0}\n[vol: {1}] [pan:{2}]",
                                TimeLine.SoundList[note.id].filename,
                                (int)(note.volume * 100),
                                (int)(note.pan * 100),
                                (int)Position
                            ), note.id);
                        }
                    }

                    TimeLine.LinesIndex[i]++;
                }
            }

            long now = Stopwatch.ElapsedTicks;
            DeltaTime = (now - lastTime) / 10000000d;
            lastTime = now;

            TickPerSecond = TimeLine.BPM * 0.25d * PatternUtils.Pattern.TickPerMeasure / 60d;
            PositionDelta = DeltaTime * TickPerSecond;
            UnscaledPosition += PositionDelta;

            // 检测结束
            if (UnscaledPosition >= TimeLine.EndTick)
            {
                Stop();
                if (LoopStop != null)
                    LoopStop();
                return;
            }

            beat += DeltaTime * (TimeLine.BPM / 60d);

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

