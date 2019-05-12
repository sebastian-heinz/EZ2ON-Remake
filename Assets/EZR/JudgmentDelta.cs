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

        public static bool CompareJudgmentDelta(double delta, JudgmentType judgment)
        {
            switch (judgment)
            {
                case JudgmentType.Kool:
                    return delta <= GetJudgmentDelta(JudgmentType.Kool);
                case JudgmentType.Cool:
                    return delta > GetJudgmentDelta(JudgmentType.Kool) && delta <= GetJudgmentDelta(JudgmentType.Cool);
                case JudgmentType.Good:
                    return delta > GetJudgmentDelta(JudgmentType.Cool) && delta <= GetJudgmentDelta(JudgmentType.Good);
                case JudgmentType.Miss:
                    return delta > GetJudgmentDelta(JudgmentType.Good) && delta <= GetJudgmentDelta(JudgmentType.Miss);
                default:
                    return false;
            }
        }

        public static float GetJudgmentDelta(JudgmentType judgment)
        {
            switch (judgment)
            {
                case JudgmentType.Kool:
                    return Kool * 0.5f * Scale;
                case JudgmentType.Cool:
                    return Cool * 0.5f * Scale;
                case JudgmentType.Good:
                    return Good * 0.5f * Scale;
                case JudgmentType.Miss:
                    return Miss * 0.5f * Scale;
                default:
                    return -1;
            }
        }
    }
}