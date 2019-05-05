using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRoom : MonoBehaviour
{
    void Start()
    {
        EZR.PlayManager.GameType = EZR.GameType.DJMAX;
        EZR.PlayManager.SongName = "fallenangel";
        EZR.PlayManager.GameMode = EZR.GameMode.Mode.FiveButtons;
        EZR.PlayManager.GameDifficult = EZR.GameDifficult.Difficult.DJMAX_HD;

        EZR.PlayManager.Reset();
        EZR.PlayManager.LoadPattern();

        GetComponent<DisplayLoop>().enabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F4))
            EZR.PlayManager.FallSpeed += 0.25f;
        if (Input.GetKeyDown(KeyCode.F3))
            EZR.PlayManager.FallSpeed -= 0.25f;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GetComponent<DisplayLoop>().Reset();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            EZR.PlayManager.IsAutoPlay = !EZR.PlayManager.IsAutoPlay;
        }
    }
}