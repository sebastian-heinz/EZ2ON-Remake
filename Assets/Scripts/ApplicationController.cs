using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    // 退出游戏
    public void BtnExit()
    {
        EZR.MemorySound.PlaySound("e_motion");
        Application.Quit();
    }
}
