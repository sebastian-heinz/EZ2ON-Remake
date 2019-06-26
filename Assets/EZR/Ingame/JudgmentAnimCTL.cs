using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EZR
{
    public class JudgmentAnimCTL : MonoBehaviour
    {
        public Sprite Cool;
        public Sprite Good;
        public Sprite Miss;
        public Sprite Fail;
        public Sprite Fast;
        public Sprite Slow;

        Animation anim;
        Animation animFastSlow;
        Image image;
        Image imagefastSlow;
        JudgmentType judgmentType = JudgmentType.Kool;

        bool isPlay = false;
        bool? isFast = null;

        void Start()
        {
            anim = GetComponent<Animation>();
            anim["KOOL"].normalizedTime = 1;
            anim.Play("KOOL");

            animFastSlow = transform.Find("FastSlow").GetComponent<Animation>();
            animFastSlow["FastSlow"].normalizedTime = 1;
            animFastSlow.Play("FastSlow");

            image = transform.Find("Parent/Image").GetComponent<Image>();
            imagefastSlow = transform.Find("FastSlow").GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            if (isPlay)
            {
                isPlay = false;

                if (judgmentType == JudgmentType.Kool)
                {
                    image.overrideSprite = null;
                    anim["KOOL"].normalizedTime = 0;
                    anim.Play("KOOL");
                }
                else
                {
                    switch (judgmentType)
                    {
                        case JudgmentType.Cool:
                            image.overrideSprite = Cool;
                            break;
                        case JudgmentType.Good:
                            image.overrideSprite = Good;
                            break;
                        case JudgmentType.Miss:
                            image.overrideSprite = Miss;
                            break;
                        case JudgmentType.Fail:
                            image.overrideSprite = Fail;
                            break;
                    }
                    anim["Any"].normalizedTime = 0;
                    anim.Play("Any");
                }
            }

            if (isFast != null)
            {
                switch (isFast)
                {
                    case true:
                        imagefastSlow.sprite = Fast;
                        imagefastSlow.SetNativeSize();
                        break;
                    case false:
                        imagefastSlow.sprite = Slow;
                        imagefastSlow.SetNativeSize();
                        break;
                }
                animFastSlow["FastSlow"].normalizedTime = 0;
                animFastSlow.Play("FastSlow");
                isFast = null;
            }
        }

        public void Play(JudgmentType Judgment)
        {
            isPlay = true;
            judgmentType = Judgment;
        }

        public void ShowFastSlow(bool isFast)
        {
            this.isFast = isFast;
        }
    }
}
