using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZR
{
    public class Score
    {
        public float RawScore = 0;
        public int Kool = 0;
        public int Cool = 0;
        public int Good = 0;
        public int Miss = 0;
        public int Fail = 0;
        public int MaxCombo = 0;

        public int GetScore()
        {
            return (int)Mathf.Round(RawScore);
        }

        public void Reset()
        {
            RawScore = 0;
            Kool = 0;
            Cool = 0;
            Good = 0;
            Miss = 0;
            Fail = 0;
            MaxCombo = 0;
        }
    }
}
