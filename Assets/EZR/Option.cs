using System.Collections.Generic;
using UnityEngine;

namespace EZR
{
    public class Option
    {
        public FullScreenMode FullScreenMode = FullScreenMode.ExclusiveFullScreen;
        public Resolution Resolution = Screen.resolutions[Screen.resolutions.Length - 1];
        public SystemLanguage Language = SystemLanguage.ChineseSimplified;
        public int TimePrecision = 1;
        public bool FrostedGlassEffect = true;
        public bool VSync = true;
        public bool LimitFPS = false;
        public int TargetFrameRate = 60;
    }
}