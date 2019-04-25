namespace PatternUtils
{
    public partial class Pattern
    {
        public static int Mapping(EZR.GameType type, int trackNum)
        {
            switch (type)
            {
                case EZR.GameType.EZ2ON:
                    switch (trackNum)
                    {
                        case 20:
                            return 0;
                        case 21:
                            return 1;
                        case 22:
                            return 2;
                        case 23:
                            return 3;
                        case 24:
                            return 4;
                        case 25:
                            return 5;
                        case 26:
                            return 6;
                        case 27:
                            return 7;
                        default:
                            return 8;
                    }
                case EZR.GameType.EZ2DJ:
                    switch (trackNum)
                    {
                        case 9:
                            return 0;
                        case 2:
                            return 1;
                        case 3:
                            return 2;
                        case 4:
                            return 3;
                        case 5:
                            return 4;
                        case 6:
                            return 5;
                        case 10:
                            return 6;
                        default:
                            return 8;
                    }
                default:
                    return 8;
            }
        }
    }
}