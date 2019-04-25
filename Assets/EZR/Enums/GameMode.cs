namespace EZR
{
    public class GameMode
    {
        public enum Mode
        {
            RubyMixON,
            RubyMixDJ,
            StreetMixON,
            StreetMixDJ,
            ClubMix,
            ClubMix8,
        }
        public static int GetNumLines(Mode mode)
        {
            switch (mode)
            {
                case Mode.RubyMixON:
                    return 4;
                case Mode.RubyMixDJ:
                    return 7;
                case Mode.StreetMixON:
                    return 5;
                case Mode.StreetMixDJ:
                    return 7;
                case Mode.ClubMix:
                    return 6;
                case Mode.ClubMix8:
                    return 8;
                default:
                    return 0;
            }
        }
        public static string GetString(Mode mode)
        {
            switch (mode)
            {
                case Mode.RubyMixON:
                    return "4-";
                case Mode.RubyMixDJ:
                    return "rubymix1p-";
                case Mode.StreetMixON:
                    return "6-";
                case Mode.StreetMixDJ:
                    return "streetmix1p-";
                case Mode.ClubMix:
                    return "8-";
                case Mode.ClubMix8:
                    return "8-";
                default:
                    return null;
            }
        }
    }
}
