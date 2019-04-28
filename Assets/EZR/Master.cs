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

        [DllImport("user32.dll")]
        public static extern short GetKeyState(int vKey);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
#endif

        public static event Action MainLoop;

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
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)

                    if (GetKeyState(68) < 0)
                    {
                        if (!Key1State)
                        {
                            Key1State = true;
                            if (InputEvent != null)
                                InputEvent(0, Key1State);
                        }
                    }
                    else
                    {
                        if (Key1State)
                        {
                            Key1State = false;
                            if (InputEvent != null)
                                InputEvent(0, Key1State);
                        }
                    }

                    if (GetKeyState(70) < 0)
                    {
                        if (!Key2State)
                        {
                            Key2State = true;
                            if (InputEvent != null)
                                InputEvent(1, Key2State);
                        }
                    }
                    else
                    {
                        if (Key2State)
                        {
                            Key2State = false;
                            if (InputEvent != null)
                                InputEvent(1, Key2State);
                        }
                    }

                    if (GetKeyState(74) < 0)
                    {
                        if (!Key3State)
                        {
                            Key3State = true;
                            if (InputEvent != null)
                                InputEvent(2, Key3State);
                        }
                    }
                    else
                    {
                        if (Key3State)
                        {
                            Key3State = false;
                            if (InputEvent != null)
                                InputEvent(2, Key3State);
                        }
                    }

                    if (GetKeyState(75) < 0)
                    {
                        if (!Key4State)
                        {
                            Key4State = true;
                            if (InputEvent != null)
                                InputEvent(3, Key4State);
                        }
                    }
                    else
                    {
                        if (Key4State)
                        {
                            Key4State = false;
                            if (InputEvent != null)
                                InputEvent(3, Key4State);
                        }
                    }
#endif

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

#if (!UNITY_STANDALONE_WIN && !UNITY_EDITOR_WIN)
            void Update()
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    if (!Key1State)
                    {
                        Key1State = true;
                        if (InputEvent != null)
                            InputEvent(0, Key1State);
                    }
                }
                if (Input.GetKeyUp(KeyCode.D))
                {
                    if (Key1State)
                    {
                        Key1State = false;
                        if (InputEvent != null)
                            InputEvent(0, Key1State);
                    }
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (!Key2State)
                    {
                        Key2State = true;
                        if (InputEvent != null)
                            InputEvent(1, Key2State);
                    }
                }
                if (Input.GetKeyUp(KeyCode.F))
                {
                    if (Key2State)
                    {
                        Key2State = false;
                        if (InputEvent != null)
                            InputEvent(1, Key2State);
                    }
                }

                if (Input.GetKeyDown(KeyCode.J))
                {
                    if (!Key3State)
                    {
                        Key3State = true;
                        if (InputEvent != null)
                            InputEvent(2, Key3State);
                    }
                }
                if (Input.GetKeyUp(KeyCode.J))
                {
                    if (Key3State)
                    {
                        Key3State = false;
                        if (InputEvent != null)
                            InputEvent(2, Key3State);
                    }
                }

                if (Input.GetKeyDown(KeyCode.K))
                {
                    if (!Key4State)
                    {
                        Key4State = true;
                        if (InputEvent != null)
                            InputEvent(3, Key4State);
                    }
                }
                if (Input.GetKeyUp(KeyCode.K))
                {
                    if (Key4State)
                    {
                        Key4State = false;
                        if (InputEvent != null)
                            InputEvent(3, Key4State);
                    }
                }
            }
#endif
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