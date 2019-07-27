using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PatternUtils;

namespace EZR
{
    public class TimeLines
    {
        public int MaxLines => 9;
        public float BPM = 120f;

        public int EndTick = 0;
        public int BPMIndex = 0;
        public List<Pattern.BPM> BPMList = new List<Pattern.BPM>();
        public int[] LinesIndex;
        public Pattern.Track[] Lines;
        public List<Pattern.Sound> SoundList = new List<Pattern.Sound>();
        public int TotalNote = 0;

        public void SortLines()
        {
            foreach (var line in Lines)
                line.Notes.Sort((a, b) => a.position.CompareTo(b.position));
            BPMList.Sort((a, b) => a.position.CompareTo(b.position));
        }

        public void Reset()
        {
            BPM = 120f;
            BPMIndex = 0;
            LinesIndex = new int[MaxLines];
        }

        public void Clear()
        {
            Reset();
            BPMList = new List<Pattern.BPM>();
            TotalNote = 0;
            EndTick = 0;
            Lines = new Pattern.Track[MaxLines];
            for (int i = 0; i < Lines.Length; i++)
            {
                Lines[i] = new Pattern.Track();
            }
            SoundList = new List<Pattern.Sound>();
        }
    }
}