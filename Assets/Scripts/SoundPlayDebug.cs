using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundPlayDebug : MonoBehaviour
{
    public Text DebugText;

    string debugMessage = "";
    float frameTime = 0;
    int frames = 0;
    int fps = 0;
    EZR.DisplayLoop displayLoop;

    void Start()
    {
        EZR.PlayManager.LoopStop += loopStop;
        EZR.PlayManager.DebugEvent += debugEvent;
        displayLoop = GameObject.Find("ScriptObject").GetComponent<EZR.DisplayLoop>();
    }

    // Update is called once per frame
    void Update()
    {
        frameTime += Time.deltaTime;
        frames++;
        if (frameTime >= 1)
        {
            frameTime -= 1;
            fps = frames;
            frames = 0;
        }
    }

    void OnGUI()
    {
        DebugText.text = "[FPS: " + fps + "]\n" +
            debugMessage + "\ndelta time (ms): " +
            EZR.PlayManager.DeltaTime * 1000 +
            "\nKool: " + EZR.PlayManager.Score.Kool +
            "\nCool: " + EZR.PlayManager.Score.Cool +
            "\nGood: " + EZR.PlayManager.Score.Good +
            "\nMiss: " + EZR.PlayManager.Score.Miss +
            "\nFail: " + EZR.PlayManager.Score.Fail +
            "\n[Speed: " + EZR.PlayManager.FallSpeed + "]" +
            "\nNote pool A: " + displayLoop.notePoolA.Count +
            "\nNote pool B: " + displayLoop.notePoolB.Count +
            "\nNote pool C: " + (displayLoop.notePoolC != null ? displayLoop.notePoolC.Count : 0) +
            "\nMeasure line pool: " + displayLoop.measureLinePool.Count;
    }

    void debugEvent(string message, int id)
    {
        debugMessage = message;
    }

    void loopStop()
    {
        EZR.PlayManager.LoopStop -= loopStop;
        EZR.PlayManager.DebugEvent -= debugEvent;
    }
}
