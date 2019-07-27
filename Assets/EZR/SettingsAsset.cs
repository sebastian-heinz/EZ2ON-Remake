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
        public string LaunchScene = "";
        public GameObject MessageBox;
        public GameObject Tooltips;
    }
}