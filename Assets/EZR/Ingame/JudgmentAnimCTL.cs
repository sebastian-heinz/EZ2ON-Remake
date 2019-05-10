using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JudgmentAnimCTL : MonoBehaviour
{
    public Sprite Cool;
    public Sprite Good;
    public Sprite Miss;
    public Sprite Fail;

    Animation anim;
    Image image;
    EZR.JudgmentType sprite = EZR.JudgmentType.Kool;

    [HideInInspector]
    public bool IsPlayKOOL = false;
    [HideInInspector]
    public bool IsPlayAny = false;

    void Start()
    {
        anim = GetComponent<Animation>();
        anim["KOOL"].normalizedTime = 1;
        anim.Play("KOOL");

        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlayKOOL || IsPlayAny)
        {
            switch (sprite)
            {
                case EZR.JudgmentType.Kool:
                    image.overrideSprite = null;
                    break;
                case EZR.JudgmentType.Cool:
                    image.overrideSprite = Cool;
                    break;
                case EZR.JudgmentType.Good:
                    image.overrideSprite = Good;
                    break;
                case EZR.JudgmentType.Miss:
                    image.overrideSprite = Miss;
                    break;
                case EZR.JudgmentType.Fail:
                    image.overrideSprite = Fail;
                    break;
            }
        }

        if (IsPlayKOOL)
        {
            IsPlayKOOL = false;
            anim["KOOL"].normalizedTime = 0;
            anim.Play("KOOL");
        }
        else if (IsPlayAny)
        {
            IsPlayAny = false;
            anim["Any"].normalizedTime = 0;
            anim.Play("Any");
        }
    }

    public void Play(EZR.JudgmentType Judgment)
    {
        if (Judgment == EZR.JudgmentType.Kool)
        {
            IsPlayKOOL = true;
            sprite = Judgment;
        }
        else
        {
            IsPlayAny = true;
            sprite = Judgment;
        }
    }
}
