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
            StreetSeven,
            ClubMix,
            ClubMix8,
            FourButtons,
            FiveButtons,
            SixButtons,
            EightButtons
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
                case Mode.FourButtons:
                    return 4;
                case Mode.FiveButtons:
                    return 5;
                case Mode.SixButtons:
                    return 6;
                case Mode.EightButtons:
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
                case Mode.StreetSeven:
                    return "7streetmix1p-";
                case Mode.ClubMix:
                    return "8-";
                case Mode.ClubMix8:
                    return "8-";
                case Mode.FourButtons:
                    return "_4b";
                case Mode.FiveButtons:
                    return "_5b";
                case Mode.SixButtons:
                    return "_6b";
                case Mode.EightButtons:
                    return "_8b";
                default:
                    return null;
            }
        }
    }
}
