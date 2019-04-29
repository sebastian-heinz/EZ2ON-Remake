using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboCounter : MonoBehaviour
{
    public Sprite[] Sprites = new Sprite[10];
    public Image[] Place = new Image[5];
    int combo = 0;
    bool isPlay = false;
    bool isClear = false;
    GameObject comboHead;
    Animation anim;
    Animation comboHeadAnim;
    void Start()
    {
        comboHead = transform.parent.Find("Head").gameObject;
        comboHead.SetActive(false);
        foreach (var item in Place)
        {
            item.gameObject.SetActive(false);
        }
        anim = GetComponent<Animation>();
        comboHeadAnim = comboHead.GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isClear)
        {
            isClear = false;
            comboHead.SetActive(false);
            if (Place[0].gameObject.activeSelf) Place[0].gameObject.SetActive(false);
            if (Place[1].gameObject.activeSelf) Place[1].gameObject.SetActive(false);
            if (Place[2].gameObject.activeSelf) Place[2].gameObject.SetActive(false);
            if (Place[3].gameObject.activeSelf) Place[3].gameObject.SetActive(false);
            if (Place[4].gameObject.activeSelf) Place[4].gameObject.SetActive(false);
            return;
        }
        if (isPlay)
        {
            isPlay = false;
            if (!comboHead.activeSelf) comboHead.SetActive(true);

            anim["ComboCounter"].time = 0;
            anim.Play("ComboCounter");
            comboHeadAnim["Combo"].time = 0;
            comboHeadAnim.Play("Combo");

            if (combo < 10)
            {
                if (!Place[0].gameObject.activeSelf) Place[0].gameObject.SetActive(true);
                if (Place[1].gameObject.activeSelf) Place[1].gameObject.SetActive(false);
                if (Place[2].gameObject.activeSelf) Place[2].gameObject.SetActive(false);
                if (Place[3].gameObject.activeSelf) Place[3].gameObject.SetActive(false);
                if (Place[4].gameObject.activeSelf) Place[4].gameObject.SetActive(false);

                Place[0].sprite = Sprites[combo % 10];
            }
            else if (combo >= 10 && combo < 100)
            {
                if (!Place[0].gameObject.activeSelf) Place[0].gameObject.SetActive(true);
                if (!Place[1].gameObject.activeSelf) Place[1].gameObject.SetActive(true);
                if (Place[2].gameObject.activeSelf) Place[2].gameObject.SetActive(false);
                if (Place[3].gameObject.activeSelf) Place[3].gameObject.SetActive(false);
                if (Place[4].gameObject.activeSelf) Place[4].gameObject.SetActive(false);

                Place[0].sprite = Sprites[combo % 10];
                Place[1].sprite = Sprites[combo % 100 / 10];
            }
            else if (combo >= 100 && combo < 1000)
            {
                if (!Place[0].gameObject.activeSelf) Place[0].gameObject.SetActive(true);
                if (!Place[1].gameObject.activeSelf) Place[1].gameObject.SetActive(true);
                if (!Place[2].gameObject.activeSelf) Place[2].gameObject.SetActive(true);
                if (Place[3].gameObject.activeSelf) Place[3].gameObject.SetActive(false);
                if (Place[4].gameObject.activeSelf) Place[4].gameObject.SetActive(false);

                Place[0].sprite = Sprites[combo % 10];
                Place[1].sprite = Sprites[combo % 100 / 10];
                Place[2].sprite = Sprites[combo % 1000 / 100];
            }
            else if (combo >= 1000 && combo < 10000)
            {
                if (!Place[0].gameObject.activeSelf) Place[0].gameObject.SetActive(true);
                if (!Place[1].gameObject.activeSelf) Place[1].gameObject.SetActive(true);
                if (!Place[2].gameObject.activeSelf) Place[2].gameObject.SetActive(true);
                if (!Place[3].gameObject.activeSelf) Place[3].gameObject.SetActive(true);
                if (Place[4].gameObject.activeSelf) Place[4].gameObject.SetActive(false);

                Place[0].sprite = Sprites[combo % 10];
                Place[1].sprite = Sprites[combo % 100 / 10];
                Place[2].sprite = Sprites[combo % 1000 / 100];
                Place[3].sprite = Sprites[combo % 10000 / 1000];
            }
            else if (combo >= 10000)
            {
                if (!Place[0].gameObject.activeSelf) Place[0].gameObject.SetActive(true);
                if (!Place[1].gameObject.activeSelf) Place[1].gameObject.SetActive(true);
                if (!Place[2].gameObject.activeSelf) Place[2].gameObject.SetActive(true);
                if (!Place[3].gameObject.activeSelf) Place[3].gameObject.SetActive(true);
                if (!Place[4].gameObject.activeSelf) Place[4].gameObject.SetActive(true);

                Place[0].sprite = Sprites[combo % 10];
                Place[1].sprite = Sprites[combo % 100 / 10];
                Place[2].sprite = Sprites[combo % 1000 / 100];
                Place[3].sprite = Sprites[combo % 10000 / 1000];
                Place[4].sprite = Sprites[combo / 10000];
            }
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
