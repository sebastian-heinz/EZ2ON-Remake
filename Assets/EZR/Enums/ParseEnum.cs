using System;

namespace EZR
{
    public static class Utils
    {
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }
    }
}