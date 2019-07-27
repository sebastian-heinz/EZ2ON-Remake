using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SinglePlay : MonoBehaviour
{
    bool isFinished = false;
    EZR.DisplayLoop displayLoop;

    void Start()
    {
        EZR.PlayManager.Reset();
        EZR.PlayManager.LoadPattern();

        EZR.PlayManager.LoopStop += loopStop;

        displayLoop = GetComponent<EZR.DisplayLoop>();
        displayLoop.enabled = true;
    }

    Coroutine speedPressedCoroutine;
    void Update()
    {

        if (isFinished)
        {
            isFinished = false;
            finished(true);
            return;
        }

        // 关门
        if (EZR.PlayManager.HP == 0)
        {
            finished(true);
            EZR.MemorySound.PlaySound("e_die");
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
            finished(false);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            EZR.PlayManager.IsAutoPlay = !EZR.PlayManager.IsAutoPlay;
            EZR.MemorySound.PlaySound("e_count_1");
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

        if (Input.GetKeyDown(KeyCode.F12))
        {
            var Info = GameObject.Find("DebugCanvas").transform.Find("Info").gameObject;
            Info.SetActive(!Info.activeSelf);
            EZR.MemorySound.PlaySound("e_count_1");
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            var bga = GameObject.Find("Canvas").transform.Find("BGA");
            if (bga != null && bga.gameObject.activeSelf)
            {
                Destroy(bga.gameObject);
                EZR.MemorySound.PlaySound("e_count_1");
            }
        }
    }

    IEnumerator speedPressed(float val)
    {
        yield return new WaitForSeconds(0.2f);
        for (; ; )
        {
            yield return new WaitForSeconds(0.075f);
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

    void finished(bool isResult)
    {
        EZR.PlayManager.LoopStop -= loopStop;
        displayLoop.Stop();

        EZR.MemorySound.Main.stop();
        EZR.MemorySound.BGM.stop();
        EZR.MemorySound.UnloadAllSound();

        if (isResult)
            SceneManager.LoadScene("SingleResult");
        else
            SceneManager.LoadScene("SingleSelectSongs");
    }
}