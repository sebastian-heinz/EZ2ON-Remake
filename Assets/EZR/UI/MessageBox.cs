using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EZR
{
    public class MessageBox : MonoBehaviour
    {
        public string Text = "Message box.";
        void Start()
        {
            transform.Find("Text").GetComponent<Text>().text = Text + "\n\n";
            EZR.MemorySound.PlaySound("e_error");
        }

        public void BtnOk()
        {
            Destroy(gameObject);
            EZR.MemorySound.PlaySound("e_click");
        }
    }
}
