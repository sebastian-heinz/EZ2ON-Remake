using System.IO;

namespace PatternUtils
{
    public partial class Pattern
    {
        public static string GetFileName(string songName, EZR.GameType type, EZR.GameMode.Mode mode, EZR.GameDifficult.Difficult difficult)
        {
            string fileName;
            if (type != EZR.GameType.DJMAX)
            {
                fileName = Path.Combine(EZR.GameMode.GetString(mode) + songName + EZR.GameDifficult.GetString(difficult) + ".json");
            }
            else
            {
                fileName = Path.Combine(songName + EZR.GameMode.GetString(mode) + EZR.GameDifficult.GetString(difficult) + ".json");
            }
            return fileName;
        }
    }
}