using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace EZR
{
    public class ButtonExtension : MonoBehaviour
    {
        public Sprite NormalSprite;
        public Sprite SelectedSprite;

        public class ButtonExtensionGroup
        {
            public ButtonExtension CurrentSelected;
            public List<ButtonExtension> List = new List<ButtonExtension>();
        }

        public string Group = "";

        public static Dictionary<string, ButtonExtensionGroup> GroupMaster = new Dictionary<string, ButtonExtensionGroup>();

        public bool IsSelected { get; private set; }

        Image image;

        void Awake()
        {
            if (!GroupMaster.ContainsKey(Group))
                GroupMaster[Group] = new ButtonExtensionGroup();
            GroupMaster[Group].List.Add(this);

            image = (Image)GetComponent<Button>().targetGraphic;
        }

        public void SetSelected(bool isSelected)
        {
            IsSelected = isSelected;

            if (IsSelected)
            {
                if (GroupMaster[Group].CurrentSelected != null && GroupMaster[Group].CurrentSelected != this)
                    GroupMaster[Group].CurrentSelected.SetSelected(false);
                GroupMaster[Group].CurrentSelected = this;
                image.sprite = SelectedSprite;
            }
            else
                image.sprite = NormalSprite;
        }

        void OnDestroy()
        {
            if (GroupMaster[Group].List.Contains(this))
                GroupMaster[Group].List.Remove(this);
        }
    }
}