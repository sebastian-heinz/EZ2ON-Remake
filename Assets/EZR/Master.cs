using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Runtime.InteropServices;

namespace EZR
{
    public static class Master
    {
        public static string Version = "";
        public static string GameResourcesFolder = "";
#if (UNITY_EDITOR)
        public static bool IsDebug = true;
#else
        public static bool IsDebug = false;
#endif
        public static int TimePrecision = 10;

        static byte TaskExitFlag = 0;

        // win平台手动调整线程片段时间
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        [DllImport("winmm.dll")]
        internal static extern uint timeBeginPeriod(uint period);

        [DllImport("winmm.dll")]
        internal static extern uint timeEndPeriod(uint period);
#endif

        public static event Action MainLoop;

        static Master()
        {
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            timeBeginPeriod(1);
#endif

            Task.Run(() =>
            {
                while (TaskExitFlag == 0)
                {
                    MainLoop();
                    Thread.Sleep(TimePrecision);
                    Debug.Log("loop");
                }
            });

            GameObject ListenUnityEvents = new GameObject();
            ListenUnityEvents.name = "ListenUnityEvents";
            ListenUnityEvents.AddComponent<listenEvents>();

            Debug.Log("EZR.Master done!");
        }

        class listenEvents : MonoBehaviour
        {
            void Awake()
            {
                DontDestroyOnLoad(this);
            }
            void OnDestroy()
            {
                TaskExitFlag = 1;
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
                timeEndPeriod(1);
#endif
            }
        }
    }
}