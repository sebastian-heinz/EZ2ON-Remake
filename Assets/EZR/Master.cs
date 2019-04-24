using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EZR
{
    public static class Master
    {
        public static string Version = "";
        public static string GameResourcesFolder = "";
#if (UNITY_EDITOR)
        public static bool DEBUG = true;
#else
        public static bool DEBUG = false;
#endif
        static Master()
        {

        }
    }
}