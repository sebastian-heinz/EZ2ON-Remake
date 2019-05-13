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

        Animation anim;
        Image image;
        JudgmentType sprite = JudgmentType.Kool;

        bool isPlay = false;

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
            if (isPlay)
            {
                isPlay = false;

                if (sprite == JudgmentType.Kool)
                {
                    image.overrideSprite = null;
                    anim["KOOL"].normalizedTime = 0;
                    anim.Play("KOOL");
                }
                else
                {
                    switch (sprite)
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
        }

        public void Play(JudgmentType Judgment)
        {
            isPlay = true;
            sprite = Judgment;
        }
    }
}
