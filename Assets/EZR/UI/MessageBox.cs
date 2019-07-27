using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EZR
{
    public class MessageBox : MonoBehaviour
    {
        [HideInInspector]
        public string Text = "Message box.";
        void Start()
        {
            transform.Find("Text").GetComponent<Text>().text = Text + "\n\n";
            EZR.MemorySound.PlaySound("e_error");

            var option = UserSaveData.GetOption();

            // 找毛玻璃
            var frostedGlass = transform.Find("FrostedGlass").gameObject;
            frostedGlass.SetActive(option.FrostedGlassEffect);
        }

        public void BtnOk()
        {
            Destroy(gameObject);
            EZR.MemorySound.PlaySound("e_click");
        }
    }
}
