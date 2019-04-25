using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PatternUtils;

namespace EZR
{
    public class TimeLines
    {
        public int MaxLines
        {
            get
            {
                return 9;
            }
        }
        public float BPM = 120f;
        public int BPMIndex = 0;
        public List<Pattern.BPM> BPMList = new List<Pattern.BPM>();
        public int[] LinesIndex;
        public Pattern.Track[] Lines;
        public List<Pattern.Sound> SoundList = new List<Pattern.Sound>();

        public void SortLines()
        {
            foreach (var line in Lines)
            {
                Pattern.Note temp;
                for (int i = 0; i < line.Notes.Count - 1; i++)
                {
                    for (int j = 0; j < line.Notes.Count - 1 - i; j++)
                    {
                        if (line.Notes[j].position > line.Notes[j + 1].position)
                        {
                            temp = line.Notes[j];
                            line.Notes[j] = line.Notes[j + 1];
                            line.Notes[j + 1] = temp;
                        }
                    }
                }
            }
            {
                Pattern.BPM temp;
                for (int i = 0; i < BPMList.Count - 1; i++)
                {
                    for (int j = 0; j < BPMList.Count - 1 - i; j++)
                    {
                        if (BPMList[j].position > BPMList[j + 1].position)
                        {
                            temp = BPMList[j];
                            BPMList[j] = BPMList[j + 1];
                            BPMList[j + 1] = temp;
                        }
                    }
                }
            }
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
            Lines = new Pattern.Track[MaxLines];
            for (int i = 0; i < Lines.Length; i++)
            {
                Lines[i] = new Pattern.Track();
            }
            SoundList = new List<Pattern.Sound>();
        }
    }
}