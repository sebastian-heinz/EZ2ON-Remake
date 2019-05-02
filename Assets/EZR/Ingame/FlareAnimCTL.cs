using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareAnimCTL : MonoBehaviour
{
    public string PlayAnimName = "";
    public string StopAnimName = "";
    public float[] InitScale = new float[EZR.PlayManager.MaxLines - 3];

    [HideInInspector]
    public bool IsPlay = false;
    [HideInInspector]
    public bool IsStop = false;

    float scale = 1;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if (IsPlay)
        {
            IsPlay = false;
            var realScale = InitScale[EZR.PlayManager.NumLines - 4] * scale;
            transform.localScale = new Vector2(realScale, realScale);
            anim.Play(PlayAnimName, 0, 0);
        }
        if (IsStop)
        {
            IsStop = false;
            anim.Play(StopAnimName, 0, 0);
        }
    }
    public void Play()
    {
        IsPlay = true;
        scale = 1;
    }
    public void Play(string judgment)
    {
        IsPlay = true;
        if (judgment == "good")
            scale = 0.6f;
        else
            scale = 1;
    }
}
