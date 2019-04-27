using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Runtime.InteropServices;
using UnityEngine.Experimental.Input;

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

        static Keyboard keyboard = Keyboard.current;
        public static event Action<int, bool> InputEvent;
        public static bool Key1State = false;
        public static bool Key2State = false;
        public static bool Key3State = false;
        public static bool Key4State = false;

        static Master()
        {
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            timeBeginPeriod(1);
#endif

            Task.Run(() =>
            {
                while (TaskExitFlag == 0)
                {
                    // 发送按键状态
                    if (Key1State != keyboard.dKey.isPressed)
                    {
                        Key1State = keyboard.dKey.isPressed;
                        if (InputEvent != null)
                            InputEvent(0, Key1State);
                    }

                    if (Key2State != keyboard.fKey.isPressed)
                    {
                        Key2State = keyboard.fKey.isPressed;
                        if (InputEvent != null)
                            InputEvent(1, Key2State);
                    }

                    if (Key3State != keyboard.jKey.isPressed)
                    {
                        Key3State = keyboard.jKey.isPressed;
                        if (InputEvent != null)
                            InputEvent(2, Key3State);
                    }

                    if (Key4State != keyboard.kKey.isPressed)
                    {
                        Key4State = keyboard.kKey.isPressed;
                        if (InputEvent != null)
                            InputEvent(3, Key4State);
                    }

                    if (MainLoop != null)
                        MainLoop();

                    Thread.Sleep(TimePrecision);
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