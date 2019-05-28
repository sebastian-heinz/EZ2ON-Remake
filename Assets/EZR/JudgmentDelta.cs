using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZR
{
    public static class JudgmentDelta
    {
        public static int Kool = 8;
        public static int Cool = 24;
        public static int Good = 60;
        public static int Miss = 76;
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
    }
}