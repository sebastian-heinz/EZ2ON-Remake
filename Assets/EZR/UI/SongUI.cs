using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZR
{
    public class SongUI : MonoBehaviour
    {
        [HideInInspector]
        public string SongName = "";
        [HideInInspector]
        public string DisplayName = "";
        [HideInInspector]
        public int BPM = 0;
        [HideInInspector]
        public int DifficultyLevel = 0;
    }
}
