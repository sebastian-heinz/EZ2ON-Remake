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
        public class VolumeClass
        {
            public int Master = 100;
            public int Game = 100;
            public int Main = 100;
            public int BGM = 100;
            public bool Live3D = false;
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
        public VolumeClass Volume = new VolumeClass();

        public static void ApplyOption(Option option)
        {
            // 设置画面模式
            if (option.VSync) QualitySettings.vSyncCount = 1;
            else QualitySettings.vSyncCount = 0;
            if (option.LimitFPS)
                Application.targetFrameRate = option.TargetFrameRate;
            else
                Application.targetFrameRate = -1;
            if (option.Resolution.width != Screen.currentResolution.width ||
            option.Resolution.height != Screen.currentResolution.height ||
            option.FullScreenMode != Screen.fullScreenMode)
                Screen.SetResolution(option.Resolution.width, option.Resolution.height, option.FullScreenMode);
            // 设置音量
            EZR.MemorySound.MasterVolume = option.Volume.Master / 100f * 0.7f;
            EZR.MemorySound.GameVolume = option.Volume.Game / 100f;
            EZR.MemorySound.MainVolume = option.Volume.Main / 100f;
            EZR.MemorySound.BGMVolume = option.Volume.BGM / 100f;
            if (option.Volume.Live3D)
            {
                var prop = FMOD.PRESET.CONCERTHALL();
                FMODUnity.RuntimeManager.LowlevelSystem.setReverbProperties(0, ref prop);
            }
            else
            {
                var prop = FMOD.PRESET.OFF();
                FMODUnity.RuntimeManager.LowlevelSystem.setReverbProperties(0, ref prop);
            }
        }
    }
}