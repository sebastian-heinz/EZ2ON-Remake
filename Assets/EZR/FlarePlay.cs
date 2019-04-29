using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlarePlay : MonoBehaviour
{
    public string PlayAnimName = "";
    public string StopAnimName = "";
    public bool isPlay = false;
    public bool isStop = false;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if (isPlay)
        {
            isPlay = false;
            anim.Play(PlayAnimName, 0, 0);
        }
        if (isStop)
        {
            isStop = false;
            anim.Play(StopAnimName, 0, 0);
        }
    }
}
