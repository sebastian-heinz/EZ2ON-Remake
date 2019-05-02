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
    Animation animKool;
    Animation animAny;
    Image image;
    EZR.JudgmentType sprite = EZR.JudgmentType.Kool;

    [HideInInspector]
    public bool IsPlayKOOL = false;
    [HideInInspector]
    public bool IsPlayAny = false;
    // Start is called before the first frame update
    void Start()
    {
        animKool = transform.Find("KOOL").GetComponent<Animation>();
        if (animKool != null)
        {
            animKool["KOOL"].normalizedTime = 1;
            animKool.Play("KOOL");
        }
        animAny = transform.Find("Any").GetComponent<Animation>();
        if (animAny != null)
        {
            animAny["Any"].normalizedTime = 1;
            animAny.Play("Any");
        }
        image = transform.Find("Any").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlayKOOL || IsPlayAny)
        {
            switch (sprite)
            {
                case EZR.JudgmentType.Cool:
                    image.sprite = Cool;
                    break;
                case EZR.JudgmentType.Good:
                    image.sprite = Good;
                    break;
                case EZR.JudgmentType.Miss:
                    image.sprite = Miss;
                    break;
                case EZR.JudgmentType.Fail:
                    image.sprite = Fail;
                    break;
            }
        }

        if (IsPlayKOOL)
        {
            IsPlayKOOL = false;
            animKool["KOOL"].normalizedTime = 0;
            animAny["Any"].normalizedTime = 1;
            animKool.Play("KOOL");
        }
        else if (IsPlayAny)
        {
            IsPlayAny = false;
            animAny["Any"].normalizedTime = 0;
            animKool["KOOL"].normalizedTime = 1;
            animAny.Play("Any");
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
