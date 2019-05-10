using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SinglePlay : MonoBehaviour
{
    bool isFinished = false;
    DisplayLoop displayLoop;

    void Start()
    {
        EZR.PlayManager.Reset();
        EZR.PlayManager.LoadPattern();

        EZR.PlayManager.LoopStop += loopStop;

        displayLoop = GetComponent<DisplayLoop>();
        displayLoop.enabled = true;
    }

    void Update()
    {

        if (isFinished)
        {
            isFinished = false;
            finished();
            return;
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            speedAdd(0.25f);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            speedAdd(-0.25f);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            finished();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            EZR.PlayManager.IsAutoPlay = !EZR.PlayManager.IsAutoPlay;
        }

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                EZR.PlayManager.FallSpeed += 0.01f;
                EZR.MemorySound.PlaySound("e_count_1");
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                EZR.PlayManager.FallSpeed -= 0.01f;
                EZR.MemorySound.PlaySound("e_count_1");
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                speedAdd(0.25f);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                speedAdd(-0.25f);
            }
        }
    }

    void speedAdd(float val)
    {
        var decimalPart = EZR.PlayManager.FallSpeed % 1;
        var closest = EZR.Utils.FindClosestNumber(decimalPart, EZR.PlayManager.FallSpeedStep);
        EZR.PlayManager.FallSpeed = ((int)EZR.PlayManager.FallSpeed + closest) + val;
        EZR.MemorySound.PlaySound("e_count_1");
    }

    void loopStop()
    {
        EZR.PlayManager.LoopStop -= loopStop;
        isFinished = true;
    }

    void finished()
    {
        EZR.PlayManager.LoopStop -= loopStop;
        displayLoop.Stop();

        EZR.MemorySound.Main.stop();
        EZR.MemorySound.BGM.stop();
        EZR.MemorySound.UnloadAllSound();

        SceneManager.LoadScene("SelectSongs");
    }
}