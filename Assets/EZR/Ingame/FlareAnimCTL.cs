using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareAnimCTL : MonoBehaviour
{
    public string PlayAnimName = "";
    public string StopAnimName = "";
    public bool isPlay = false;
    public bool isStop = false;
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
        if (isPlay)
        {
            isPlay = false;
            transform.localScale = new Vector2(initScale * scale, initScale * scale);
            anim.Play(PlayAnimName, 0, 0);
        }
        if (isStop)
        {
            isStop = false;
            anim.Play(StopAnimName, 0, 0);
        }
    }
    public void Play()
    {
        isPlay = true;
        scale = 1;
    }
    public void Play(float scale)
    {
        isPlay = true;
        this.scale = scale;
    }
}
