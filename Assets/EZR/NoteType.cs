using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZR
{
    [CreateAssetMenu(fileName = "Note", menuName = "EZR/Note", order = 1)]
    public class NoteType : ScriptableObject
    {
        public Note[] Notes = new Note[PlayManager.MaxLines - 3];

        [System.Serializable]
        public class Note
        {
            public GameObject[] NotePrefab;
        }

        public GameObject Flare;

        public float[] NoteScale = new float[PlayManager.MaxLines - 3];

        public GameObject[] Target = new GameObject[PlayManager.MaxLines - 3];
    }
}

