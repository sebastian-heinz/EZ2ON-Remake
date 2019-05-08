namespace EZR
{
    public class GameDifficulty
    {
        public enum Difficulty
        {
            EZ,
            NM,
            HD,
            SHD,
            DJMAX_NM,
            DJMAX_HD,
            DJMAX_MX
        }

        public static string GetString(Difficulty difficult)
        {
            switch (difficult)
            {
                case Difficulty.EZ:
                    return "-ez";
                case Difficulty.NM:
                    return "";
                case Difficulty.HD:
                    return "-hd";
                case Difficulty.SHD:
                    return "-shd";
                case Difficulty.DJMAX_NM:
                    return "_nm";
                case Difficulty.DJMAX_HD:
                    return "_hd";
                case Difficulty.DJMAX_MX:
                    return "_mx";
                default:
                    return null;
            }
        }
    }
}