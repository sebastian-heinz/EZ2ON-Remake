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

    void Start()
    {
        EZR.PlayManager.LoopStop += loopStop;
        EZR.PlayManager.DebugEvent += debugEvent;
    }

    // Update is called once per frame
    void Update()
    {
        frameTime += Time.unscaledDeltaTime;
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
            "\n[Speed: " + EZR.PlayManager.FallSpeed + "]";
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
