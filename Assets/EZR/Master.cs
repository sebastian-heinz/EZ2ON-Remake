using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        public static GameObject MessageBox;

        static byte TaskExitFlag = 0;

        // win平台手动调整线程片段时间
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        [DllImport("winmm.dll")]
        internal static extern uint timeBeginPeriod(uint period);

        [DllImport("winmm.dll")]
        internal static extern uint timeEndPeriod(uint period);

        // 捕获底层键盘输入
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
#endif

        public static event Action MainLoop;

        public static event Action<int, bool> InputEvent;
        public static bool[] KeysState = new bool[PlayManager.MaxLines];

        public static char[][] DefaultKeyCodeMapping = new char[][]{
            new char[]{'D','F','J','K'},
            new char[]{'D','F',(char)32,'J','K'},
            new char[]{'S','D','F','J','K','L'},
            new char[]{'S','D','F',(char)32,'J','K','L'},
            new char[]{'A','S','D','F','J','K','L',(char)186}
        };

        public static char[][] KeyCodeMapping;

        static Master()
        {
            // 全屏
            Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length - 1].width, Screen.resolutions[Screen.resolutions.Length - 1].height, FullScreenMode.ExclusiveFullScreen);

            // 初始化按键映射
            KeyCodeMapping = new char[DefaultKeyCodeMapping.Length][];
            for (int i = 0; i < DefaultKeyCodeMapping.Length; i++)
            {
                KeyCodeMapping[i] = new char[DefaultKeyCodeMapping[i].Length];
                for (int j = 0; j < DefaultKeyCodeMapping[i].Length; j++)
                {
                    KeyCodeMapping[i][j] = DefaultKeyCodeMapping[i][j];
                }
            }

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            timeBeginPeriod(1);
#endif

            Task.Run(() =>
            {
#if (UNITY_EDITOR)
                try
                {
#endif
                    while (TaskExitFlag == 0)
                    {
                        // 捕获按键
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
                        for (int i = 0; i < PlayManager.NumLines; i++)
                        {
                            var keyCode = KeyCodeMapping[PlayManager.NumLines - 4][i];
                            var isDown = GetAsyncKeyState(keyCode) < 0;
                            if (isDown != KeysState[i])
                            {
                                KeysState[i] = isDown;
                                if (InputEvent != null)
                                    InputEvent(i, KeysState[i]);
                            }
                        }
#endif

                        if (MainLoop != null)
                            MainLoop();

                        Thread.Sleep(TimePrecision);
                    }
#if (UNITY_EDITOR)
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message + "\n" + ex.StackTrace);
                }
#endif
            });

            var listenUnityEvents = new GameObject();
            listenUnityEvents.name = "ListenUnityEvents";
            listenUnityEvents.AddComponent<listenUnityEvents>();

            Debug.Log("EZR.Master done!");
        }

        class listenUnityEvents : MonoBehaviour
        {
            void Awake()
            {
                DontDestroyOnLoad(this);
                SceneManager.sceneUnloaded += (scene) =>
                {
                    Resources.UnloadUnusedAssets();
                };
            }

            // 不是win平台
            // TODO 需要制作一个ASCII转Unity Key code映射表
#if (!UNITY_STANDALONE_WIN && !UNITY_EDITOR_WIN)
            void Update()
            {
                for (int i = 0; i < PlayManager.NumLines; i++)
                {
                    var keyCode = KeyCodeMapping[PlayManager.NumLines - 4][i];
                    keyCode = keyCode.ToString().ToLower()[0];
                    var isDown = Input.GetKey((KeyCode)keyCode);
                    if (isDown != KeysState[i])
                    {
                        KeysState[i] = isDown;
                        if (InputEvent != null)
                            InputEvent(i, KeysState[i]);
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
                MemorySound.StopSound();
            }
        }
    }
}