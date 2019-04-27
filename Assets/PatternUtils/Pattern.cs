﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace PatternUtils
{
    public partial class Pattern
    {
        public static int MeasureLength
        {
            get
            {
                return 192;
            }
        }
        public class Sound
        {
            public string id = "";
            public int type = 0;
            public string filename = "";
        }
        public List<Sound> SoundList = new List<Sound>();

        public class BPM
        {
            public int position = 0;
            public int type = 0;
            public float bpm = 0;
        }

        public List<BPM> BPMList = new List<BPM>();

        public class Note
        {
            public int position = 0;
            public int type = 0;
            public int id = 0;
            public float vol = 0;
            public float pan = 0;
            public int length = 0;
        }
        public class Track
        {
            public string name = "";
            public List<Note> Notes = new List<Note>();
        }

        public List<Track> TrackList = new List<Track>();
    }
}