using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareAnimCTL : MonoBehaviour
{
    public string PlayAnimName = "";
    public string StopAnimName = "";
    public bool IsPlay = false;
    public bool IsStop = false;
    float initScale;
    float scale = 1;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        initScale = transform.localScale.x;
    }
    void Update()
    {
        if (IsPlay)
        {
            IsPlay = false;
            transform.localScale = new Vector2(initScale * scale, initScale * scale);
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
