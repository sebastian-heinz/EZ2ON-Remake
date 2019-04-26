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
    }

}