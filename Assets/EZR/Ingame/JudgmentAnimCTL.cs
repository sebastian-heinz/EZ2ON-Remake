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
    public bool isPlayKOOL = false;
    public bool isPlayAny = false;
    // Start is called before the first frame update
    void Start()
    {
        animKool = transform.Find("KOOL").GetComponent<Animation>();
        animAny = transform.Find("Any").GetComponent<Animation>();
        image = transform.Find("Any").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayKOOL || isPlayAny)
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

        if (isPlayKOOL)
        {
            isPlayKOOL = false;
            animKool["KOOL"].normalizedTime = 0;
            animAny["Any"].normalizedTime = 1;
            animKool.Play("KOOL");
        }
        else if (isPlayAny)
        {
            isPlayAny = false;
            animAny["Any"].normalizedTime = 0;
            animKool["KOOL"].normalizedTime = 1;
            animAny.Play("Any");
        }
    }

    public void Play(string Judgment)
    {
        if (Judgment == "kool")
        {
            isPlayKOOL = true;
            sprite = Judgment;
        }
        else
        {
            isPlayAny = true;
            sprite = Judgment;
        }
    }
}
