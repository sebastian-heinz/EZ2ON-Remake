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

    Coroutine speedPressedCoroutine;
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
                speedAddSmall(0.01f);
                if (speedPressedCoroutine != null) StopCoroutine(speedPressedCoroutine);
                speedPressedCoroutine = StartCoroutine(speedPressed(0.01f));
            }
            else if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                if (speedPressedCoroutine != null) StopCoroutine(speedPressedCoroutine);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                speedAddSmall(-0.01f);
                if (speedPressedCoroutine != null) StopCoroutine(speedPressedCoroutine);
                speedPressedCoroutine = StartCoroutine(speedPressed(-0.01f));
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                if (speedPressedCoroutine != null) StopCoroutine(speedPressedCoroutine);
            }
        }
        else
        {
            if (speedPressedCoroutine != null) StopCoroutine(speedPressedCoroutine);
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

    IEnumerator speedPressed(float val)
    {
        yield return new WaitForSecondsRealtime(0.2f);
        for (; ; )
        {
            yield return new WaitForSecondsRealtime(0.075f);
            speedAddSmall(val);
        }
    }

    void speedAddSmall(float val)
    {
        EZR.PlayManager.FallSpeed += val;
        EZR.MemorySound.PlaySound("e_count_1");
    }

    void speedAdd(float val)
    {
        var decimalPart = EZR.PlayManager.FallSpeed % 1;
        float closest;
        if (val > 0)
            closest = EZR.Utils.FindClosestNumber(decimalPart, EZR.PlayManager.FallSpeedStep, true);
        else
            closest = EZR.Utils.FindClosestNumber(decimalPart, EZR.PlayManager.FallSpeedStep, false);
        if (Mathf.Abs(((int)EZR.PlayManager.FallSpeed + closest) - EZR.PlayManager.FallSpeed) > 0.009f)
            EZR.PlayManager.FallSpeed = ((int)EZR.PlayManager.FallSpeed + closest);
        else
            EZR.PlayManager.FallSpeed = ((int)EZR.PlayManager.FallSpeed + closest) + val;
        EZR.MemorySound.PlaySound("e_count_1");
    }

    void loopStop()
    {
        EZR.PlayManager.LoopStop -= loopStop;
        isFinished = true;
        EZR.PlayManager.Score.IsClear = true;
    }

    void finished()
    {
        EZR.PlayManager.LoopStop -= loopStop;
        displayLoop.Stop();

        EZR.MemorySound.Main.stop();
        EZR.MemorySound.BGM.stop();
        EZR.MemorySound.UnloadAllSound();

        SceneManager.LoadScene("SingleResult");
    }
}