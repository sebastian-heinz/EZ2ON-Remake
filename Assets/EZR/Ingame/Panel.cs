using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    public GameObject Judgment;
    public GameObject LongFlare;
    public Text ScoreText;
    public Text MaxComboText;

    public GameObject[] Lines = new GameObject[EZR.PlayManager.MaxLines - 3];
}
