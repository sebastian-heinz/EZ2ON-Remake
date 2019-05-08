using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace EZR
{
    public class ButtonExtension : MonoBehaviour
    {
        public Sprite NormalSprite;
        public Sprite SelectedSprite;

        public string Group = "";

        public static Dictionary<string, List<ButtonExtension>> GroupMaster = new Dictionary<string, List<ButtonExtension>>();

        bool isSelected = false;

        public bool IsSelected { get => isSelected; }

        Image image;

        void Awake()
        {
            if (!GroupMaster.ContainsKey(Group))
                GroupMaster[Group] = new List<ButtonExtension>();
            GroupMaster[Group].Add(this);

            image = (Image)GetComponent<Button>().targetGraphic;
        }

        public void SetSelected(bool isSelected)
        {
            this.isSelected = isSelected;

            if (this.isSelected)
            {
                foreach (var btn in GroupMaster[Group])
                {
                    if (btn != this)
                        btn.SetSelected(false);
                }
                image.sprite = SelectedSprite;
            }
            else
                image.sprite = NormalSprite;
        }

        void OnDestroy()
        {
            if (GroupMaster[Group].Contains(this))
                GroupMaster[Group].Remove(this);
        }
    }
}