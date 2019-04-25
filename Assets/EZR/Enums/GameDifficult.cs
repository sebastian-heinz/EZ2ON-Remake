namespace EZR
{
    public class GameDifficult
    {
        public enum Difficult
        {
            EZ,
            NM,
            HD,
            SHD
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
                default:
                    return null;
            }
        }
    }

}