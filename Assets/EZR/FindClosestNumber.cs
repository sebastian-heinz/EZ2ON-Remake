using UnityEngine;

namespace EZR
{
    public static partial class Utils
    {
        public static float FindClosestNumber(float num, float[] arr)
        {
            float dis = Mathf.Infinity;
            float result = num;
            foreach (var num2 in arr)
            {
                if (Mathf.Abs(num - num2) < dis)
                {
                    dis = Mathf.Abs(num - num2);
                    result = num2;
                }
            }
            return result;
        }
    }
}