using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

public class TestRoom : MonoBehaviour
{
    void Start()
    {
        EZR.PlayManager.GameType = EZR.GameType.EZ2ON;
        EZR.PlayManager.SongName = "20000000000";
        EZR.PlayManager.GameMode = EZR.GameMode.Mode.StreetMixON;
        EZR.PlayManager.GameDifficult = EZR.GameDifficult.Difficult.SHD;

        EZR.PlayManager.Reset();
        EZR.PlayManager.LoadPattern();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
            EZR.PlayManager.FallSpeed += 0.25f;
        if (Input.GetKeyDown(KeyCode.F4))
            EZR.PlayManager.FallSpeed -= 0.25f;
    }
}