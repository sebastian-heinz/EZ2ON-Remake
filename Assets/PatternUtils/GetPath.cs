using System.IO;

namespace PatternUtils
{
    public partial class Pattern
    {
        public static string GetPath(string songName, EZR.GameType type, EZR.GameMode.Mode mode, EZR.GameDifficulty.Difficulty difficulty)
        {
            string path;
            if (type != EZR.GameType.DJMAX)
            {
                path = Path.Combine(
                   EZR.Master.GameResourcesFolder,
                   type.ToString(),
                   "Songs",
                   songName,
                   EZR.GameMode.GetString(mode) + songName + EZR.GameDifficulty.GetString(difficulty) +
                   ".json"
               );
            }
            else
            {
                path = Path.Combine(
                     EZR.Master.GameResourcesFolder,
                     type.ToString(),
                     "Songs",
                     songName,
                    songName + EZR.GameMode.GetString(mode) + EZR.GameDifficulty.GetString(difficulty) +
                     ".json"
                 );
            }
            return path;
        }
    }
}