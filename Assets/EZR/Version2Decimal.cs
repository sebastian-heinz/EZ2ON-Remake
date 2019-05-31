namespace EZR
{
    public static partial class Utils
    {
        public static decimal Version2Decmal(string version)
        {
            var verArr = version.Split('.');
            var result = verArr[0] + ".";
            for (int i = 1; i < verArr.Length; i++)
            {
                result += verArr[i];
            }
            return System.Convert.ToDecimal(result);
        }
    }
}