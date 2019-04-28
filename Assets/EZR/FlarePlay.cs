using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlarePlay : MonoBehaviour
{
    public bool isPlay = false;
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
            anim.Play("Note_04_flare", 0, 0);
        }
    }
}
