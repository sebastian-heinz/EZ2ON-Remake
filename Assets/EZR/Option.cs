using System.Collections.Generic;
using UnityEngine;

namespace EZR
{
    public class Option
    {
        public FullScreenMode FullScreenMode = FullScreenMode.FullScreenWindow;
        public Resolution Resolution = Screen.resolutions[Screen.resolutions.Length - 1];
        public SystemLanguage Language = SystemLanguage.ChineseSimplified;
        public int TimePrecision = 1;
        public bool FrostedGlassEffect = false;
        public bool VSync = true;
        public bool SimVSync = false;
        public bool LimitFPS = false;
        public int TargetFrameRate = 60;
    }
}