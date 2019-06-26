using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZR
{
    public static class JudgmentDelta
    {
        public enum Difficult
        {
            Standard,
            SuddenEZ,
            SuddenNM,
            SuddenHD,
            SuddenSHD
        }

        public enum Mode
        {
            Normal,
            Easy,
            Hard
        }

        public static float Kool { get; private set; }
        public static float Cool { get; private set; }
        public static float Good { get; private set; }
        public static float Miss { get; private set; }
        public static float MeasureScale { get; private set; }
        static float globalScale = 0.8f;

        public static bool CompareJudgmentDelta(double delta, JudgmentType judgment, float scale)
        {
            switch (judgment)
            {
                case JudgmentType.Kool:
                    return delta <= GetJudgmentDelta(JudgmentType.Kool, scale);
                case JudgmentType.Cool:
                    return delta > GetJudgmentDelta(JudgmentType.Kool, scale) && delta <= GetJudgmentDelta(JudgmentType.Cool, scale);
                case JudgmentType.Good:
                    return delta > GetJudgmentDelta(JudgmentType.Cool, scale) && delta <= GetJudgmentDelta(JudgmentType.Good, scale);
                case JudgmentType.Miss:
                    return delta > GetJudgmentDelta(JudgmentType.Good, scale) && delta <= GetJudgmentDelta(JudgmentType.Miss, scale);
                default:
                    return false;
            }
        }

        public static float GetJudgmentDelta(JudgmentType judgment, float scale)
        {
            switch (judgment)
            {
                case JudgmentType.Kool:
                    return Kool * scale * globalScale;
                case JudgmentType.Cool:
                    return Cool * scale * globalScale;
                case JudgmentType.Good:
                    return Good * scale * globalScale;
                case JudgmentType.Miss:
                    return Miss * scale * globalScale;
                default:
                    return -1;
            }
        }

        public static void SetJudgmentDelta(Difficult difficult, Mode mode)
        {
            // var JudgmentDeltaDataPath = System.IO.Path.Combine(Application.dataPath, "..", "JudgmentDelta.json");
            // Newtonsoft.Json.Linq.JObject jobj = new Newtonsoft.Json.Linq.JObject();
            // if (System.IO.File.Exists(JudgmentDeltaDataPath))
            // {
            //     try
            //     {
            //         jobj = Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText(JudgmentDeltaDataPath));
            //     }
            //     catch { }
            // }
            switch (difficult)
            {
                case Difficult.Standard:
                    switch (mode)
                    {
                        case Mode.Normal:
                            Kool = 8;
                            Cool = 24;
                            Good = 50;
                            Miss = 66;
                            break;
                        case Mode.Easy:
                            Kool = 12;
                            Cool = 32;
                            Good = 60;
                            Miss = 76;
                            break;
                        case Mode.Hard:
                            Kool = 8;
                            Cool = 24;
                            Good = 40;
                            Miss = 56;
                            break;
                    }
                    // if (jobj.ContainsKey("Kool")) Kool = (int)jobj["Kool"];
                    // if (jobj.ContainsKey("Cool")) Cool = (int)jobj["Cool"];
                    // if (jobj.ContainsKey("Good")) Good = (int)jobj["Good"];
                    // if (jobj.ContainsKey("Miss")) Miss = (int)jobj["Miss"];
                    break;
                case Difficult.SuddenEZ:
                    Kool = 8;
                    Cool = 24;
                    Good = 40;
                    Miss = 56;
                    break;
                case Difficult.SuddenNM:
                    Kool = 4;
                    Cool = 12;
                    Good = 20;
                    Miss = 28;
                    break;
                case Difficult.SuddenHD:
                    Kool = 2;
                    Cool = 3;
                    Good = 5;
                    Miss = 7;
                    break;
                case Difficult.SuddenSHD:
                    Kool = 1;
                    Cool = 2;
                    Good = 3;
                    Miss = 4;
                    break;
            }
            switch (mode)
            {
                case Mode.Normal:
                    MeasureScale = 1.8f;
                    break;
                case Mode.Easy:
                    MeasureScale = 1.6f;
                    break;
                case Mode.Hard:
                    MeasureScale = 2.0f;
                    break;
            }
            // if (jobj.ContainsKey("MeasureScale")) MeasureScale = (float)jobj["MeasureScale"];
            // if (jobj.ContainsKey("GlobalScale")) globalScale = (float)jobj["GlobalScale"];
        }
    }
}