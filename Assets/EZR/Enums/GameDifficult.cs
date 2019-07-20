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
            EX,
            DJMAX_EZ,
            DJMAX_NM,
            DJMAX_HD,
            DJMAX_MX,
            DJMAX_SC
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
                case Difficult.EX:
                    return "-ex";
                case Difficult.DJMAX_EZ:
                    return "_ez";
                case Difficult.DJMAX_NM:
                    return "_nm";
                case Difficult.DJMAX_HD:
                    return "_hd";
                case Difficult.DJMAX_MX:
                    return "_mx";
                case Difficult.DJMAX_SC:
                    return "_sc";
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
                    return "SuperHard mix";
                case Difficult.EX:
                    return "Extreme mix";
                case Difficult.DJMAX_EZ:
                    return "Easy mode";
                case Difficult.DJMAX_NM:
                    return "Normal mode";
                case Difficult.DJMAX_HD:
                    return "Hard mode";
                case Difficult.DJMAX_MX:
                    return "Maximum mode";
                case Difficult.DJMAX_SC:
                    return "SuperCrazy mode";
                default:
                    return null;
            }
        }
    }
}