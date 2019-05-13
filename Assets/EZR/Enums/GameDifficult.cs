namespace EZR
{
    public class GameDifficult
    {
        public enum Difficult
        {
            EZ,
            NM,
            HD,
            SHD,
            DJMAX_NM,
            DJMAX_HD,
            DJMAX_MX
        }

        public static string GetString(Difficult difficult)
        {
            switch (difficult)
            {
                case Difficult.EZ:
                    return "-ez";
                case Difficult.NM:
                    return "";
                case Difficult.HD:
                    return "-hd";
                case Difficult.SHD:
                    return "-shd";
                case Difficult.DJMAX_NM:
                    return "_nm";
                case Difficult.DJMAX_HD:
                    return "_hd";
                case Difficult.DJMAX_MX:
                    return "_mx";
                default:
                    return null;
            }
        }

        public static string GetFullName(Difficult difficult)
        {
            switch (difficult)
            {
                case Difficult.EZ:
                    return "Easy mix";
                case Difficult.NM:
                    return "Normal mix";
                case Difficult.HD:
                    return "Hard mix";
                case Difficult.SHD:
                    return "Superhard mix";
                case Difficult.DJMAX_NM:
                    return "Normal mode";
                case Difficult.DJMAX_HD:
                    return "Hard mode";
                case Difficult.DJMAX_MX:
                    return "Maximum mode";
                default:
                    return null;
            }
        }
    }
}