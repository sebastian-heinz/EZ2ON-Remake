namespace EZR
{
    public class GameMode
    {
        public enum Mode
        {
            None,
            RubyMixON,
            RubyMixDJ,
            StreetMixON,
            StreetMixDJ,
            StreetSeven,
            ClubMix,
            ClubMix8,
            FourButton,
            FiveButton,
            SixButton,
            EightButton,
            FourKey,
            FiveKey,
            SixKey,
            SevenKey,
            EightKey
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
                case Mode.FourButton:
                    return 4;
                case Mode.FiveButton:
                    return 5;
                case Mode.SixButton:
                    return 6;
                case Mode.EightButton:
                    return 8;
                case Mode.FourKey:
                    return 4;
                case Mode.FiveKey:
                    return 5;
                case Mode.SixKey:
                    return 6;
                case Mode.SevenKey:
                    return 7;
                case Mode.EightKey:
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
                case Mode.FourButton:
                    return "_4b";
                case Mode.FiveButton:
                    return "_5b";
                case Mode.SixButton:
                    return "_6b";
                case Mode.EightButton:
                    return "_8b";
                case Mode.FourKey:
                    return "_4key";
                case Mode.FiveKey:
                    return "_5key";
                case Mode.SixKey:
                    return "_6key";
                case Mode.SevenKey:
                    return "_7key";
                case Mode.EightKey:
                    return "_8key";
                default:
                    return null;
            }
        }
    }
}
