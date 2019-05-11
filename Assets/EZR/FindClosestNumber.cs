using UnityEngine;

namespace EZR
{
    public static partial class Utils
    {
        public static float FindClosestNumber(float num, float[] arr, bool isBiger)
        {
            float dis = Mathf.Infinity;
            float result = num;
            foreach (var num2 in arr)
            {
                if (isBiger)
                {
                    if (num2 > num)
                    {
                        if (Mathf.Abs(num - num2) < dis)
                        {
                            dis = Mathf.Abs(num - num2);
                            result = num2;
                        }
                    }
                }
                else
                {
                    if (num2 < num)
                    {
                        if (Mathf.Abs(num - num2) < dis)
                        {
                            dis = Mathf.Abs(num - num2);
                            result = num2;
                        }
                    }
                }
            }
            if (dis == Mathf.Infinity)
            {
                if (isBiger) return Mathf.Max(arr);
                else return Mathf.Min(arr);
            }
            return result;
        }
    }
}