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
        public static float Scale = 0.5f;

        public static bool CompareJudgmentDelta(double delta, JudgmentType judgment, float uniqueScale)
        {
            switch (judgment)
            {
                case JudgmentType.Kool:
                    return delta <= GetJudgmentDelta(JudgmentType.Kool, uniqueScale);
                case JudgmentType.Cool:
                    return delta > GetJudgmentDelta(JudgmentType.Kool, uniqueScale) && delta <= GetJudgmentDelta(JudgmentType.Cool, uniqueScale);
                case JudgmentType.Good:
                    return delta > GetJudgmentDelta(JudgmentType.Cool, uniqueScale) && delta <= GetJudgmentDelta(JudgmentType.Good, uniqueScale);
                case JudgmentType.Miss:
                    return delta > GetJudgmentDelta(JudgmentType.Good, uniqueScale) && delta <= GetJudgmentDelta(JudgmentType.Miss, uniqueScale);
                default:
                    return false;
            }
        }

        public static float GetJudgmentDelta(JudgmentType judgment, float uniqueScale)
        {
            switch (judgment)
            {
                case JudgmentType.Kool:
                    return Kool * uniqueScale * Scale;
                case JudgmentType.Cool:
                    return Cool * uniqueScale * Scale;
                case JudgmentType.Good:
                    return Good * uniqueScale * Scale;
                case JudgmentType.Miss:
                    return Miss * uniqueScale * Scale;
                default:
                    return -1;
            }
        }

        public static void SetJudgmentDelta(Difficult difficult, Mode mode)
        {

            switch (difficult)
            {
                case Difficult.Standard:
                    Kool = 8;
                    Cool = 24;
                    if (mode == Mode.Hard)
                    {
                        Good = 40;
                        Miss = 56;
                    }
                    else
                    {
                        Good = 60;
                        Miss = 76;
                    }
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
                case Mode.Easy:
                    Kool *= 2;
                    Cool *= 2;
                    break;
                case Mode.Hard:
                    Kool *= 0.5f;
                    Cool *= 0.5f;
                    break;
            }
        }
    }
}