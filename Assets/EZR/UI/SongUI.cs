using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace EZR
{
    public class SongUI : MonoBehaviour
    {
        [FormerlySerializedAs("NormalColor")]
        public Color NormalColor = new Color(0, 0.8f, 1);
        [HideInInspector]
        public int Index = 0;
        [HideInInspector]
        public int SongIndex = 0;
    }
}
