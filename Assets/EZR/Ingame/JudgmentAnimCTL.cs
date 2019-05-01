using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JudgmentAnimCTL : MonoBehaviour
{
    public Sprite KOOL;
    public Sprite COOL;
    public Sprite GOOD;
    public Sprite MISS;
    public Sprite FAIL;
    Animation animKool;
    Animation animAny;
    Image image;
    string sprite = "kool";
    public bool IsPlayKOOL = false;
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
                case "cool":
                    image.sprite = COOL;
                    break;
                case "good":
                    image.sprite = GOOD;
                    break;
                case "miss":
                    image.sprite = MISS;
                    break;
                case "fail":
                    image.sprite = FAIL;
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

    public void Play(string Judgment)
    {
        if (Judgment == "kool")
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
