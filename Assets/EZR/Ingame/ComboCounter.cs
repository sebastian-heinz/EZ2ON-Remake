using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EZR
{
    public class ComboCounter : MonoBehaviour
    {
        int combo = 0;
        Text comboText;
        bool isPlay = false;
        bool isClear = false;
        Image comboHead;
        Animation anim;
        Animation comboHeadAnim;
        void Start()
        {
            comboHead = transform.parent.Find("Head/Image").GetComponent<Image>();
            comboHead.color = Color.black;
            comboText = transform.Find("Text").GetComponent<Text>();
            comboText.color = Color.black;
            anim = GetComponent<Animation>();
            comboHeadAnim = transform.parent.Find("Head").GetComponent<Animation>();
        }

        // Update is called once per frame
        void Update()
        {
            if (isClear)
            {
                isClear = false;
                anim["ComboCounter"].normalizedTime = 1;
                comboHeadAnim["Combo"].normalizedTime = 1;
                comboHead.color = Color.black;
                comboText.color = Color.black;
                return;
            }
            if (isPlay)
            {
                isPlay = false;
                comboHead.color = Color.white;
                comboText.color = Color.white;

                anim["ComboCounter"].time = 0;
                anim.Play("ComboCounter");
                comboHeadAnim["Combo"].time = 0;
                comboHeadAnim.Play("Combo");

                comboText.text = combo.ToString();
            }
        }

        public void SetCombo(int combo)
        {
            this.combo = combo;
            isPlay = true;
        }

        public void Clear()
        {
            combo = 0;
            isClear = true;
        }
    }
}