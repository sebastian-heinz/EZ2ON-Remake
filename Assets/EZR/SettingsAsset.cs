using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZR
{
    [CreateAssetMenu(fileName = "EZR_Settings", menuName = "EZR/Settings")]
    public class SettingsAsset : ScriptableObject
    {
        public string Version = "";
        public string devGameResourcesFolder = "";
        public int TimePrecision = 10;
        public string LaunchScene = "";
        public TextAsset SongsList;
    }
}
