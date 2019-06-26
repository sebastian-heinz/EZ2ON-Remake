using System.Collections.Generic;
using UnityEngine;

namespace EZR
{
    public class Option
    {
        public enum PanelPositionEnum
        {
            Left = -680,
            Center = 0,
            Right = 680
        }
        public enum TargetLineTypeEnum
        {
            Classic,
            New = 30
        }
        public FullScreenMode FullScreenMode = FullScreenMode.FullScreenWindow;
        public Resolution Resolution = Screen.resolutions[Screen.resolutions.Length - 1];
        public SystemLanguage Language = SystemLanguage.ChineseSimplified;
        public int TimePrecision = 1;
        public bool FrostedGlassEffect = false;
        public bool VSync = true;
        public bool LimitFPS = false;
        public int TargetFrameRate = 60;
        public PanelPositionEnum PanelPosition = PanelPositionEnum.Center;
        public TargetLineTypeEnum TargetLineType = TargetLineTypeEnum.Classic;
        public int JudgmentOffset = 0;
        public bool ShowFastSlow = false;
    }
}