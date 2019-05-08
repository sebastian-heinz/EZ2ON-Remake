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
            EZR.MemorySound.BGM.isPlaying(out bool isPlaying);
            if (!isPlaying)
            {
                isFinished = false;
                finished();
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.F4))
            EZR.PlayManager.FallSpeed += 0.25f;
        if (Input.GetKeyDown(KeyCode.F3))
            EZR.PlayManager.FallSpeed -= 0.25f;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            finished();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            EZR.PlayManager.IsAutoPlay = !EZR.PlayManager.IsAutoPlay;
        }
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