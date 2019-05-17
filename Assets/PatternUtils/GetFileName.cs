using System.IO;

namespace PatternUtils
{
    public partial class Pattern
    {
        public static string GetFileName(string songName, EZR.GameType type, EZR.GameMode.Mode mode, EZR.GameDifficult.Difficult difficult)
        {
            string fileName;
            if (type == EZR.GameType.DJMAX)
            {
                if (mode == EZR.GameMode.Mode.FourKey ||
                    mode == EZR.GameMode.Mode.FiveKey ||
                    mode == EZR.GameMode.Mode.SixKey ||
                    mode == EZR.GameMode.Mode.SevenKey ||
                    mode == EZR.GameMode.Mode.EightKey)
                {
                    fileName = Path.Combine(songName + "_ORG" + EZR.GameDifficult.GetString(difficult) + EZR.GameMode.GetString(mode) + ".json");
                }
                else
                {
                    fileName = Path.Combine(songName + EZR.GameMode.GetString(mode) + EZR.GameDifficult.GetString(difficult) + ".json");
                }
            }
            else
            {
                fileName = Path.Combine(EZR.GameMode.GetString(mode) + songName + EZR.GameDifficult.GetString(difficult) + ".json");
            }
            return fileName;
        }
    }
}