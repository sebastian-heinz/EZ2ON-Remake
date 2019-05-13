using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EZR
{
    public class Panel : MonoBehaviour
    {
        public GameObject Judgment;
        public GameObject LongFlare;
        public GameObject MeasureLine;
        public Text ScoreText;
        public Text MaxComboText;

        public GameObject[] Lines = new GameObject[PlayManager.MaxLines - 3];
    }
}
