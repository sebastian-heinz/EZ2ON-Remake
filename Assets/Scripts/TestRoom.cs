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
        EZR.PlayManager.GameMode = EZR.GameMode.Mode.ClubMix8;
        EZR.PlayManager.GameDifficult = EZR.GameDifficult.Difficult.SHD;

        EZR.PlayManager.Reset();
        EZR.PlayManager.LoadPattern();
    }
}