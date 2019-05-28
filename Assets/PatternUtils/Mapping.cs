namespace PatternUtils
{
    public partial class Pattern
    {
        public static int Mapping(int trackNum, EZR.GameType type, EZR.GameMode.Mode mode)
        {
            switch (type)
            {
                case EZR.GameType.EZ2ON:
                    switch (trackNum)
                    {
                        case 21:
                            return 0;
                        case 22:
                            return 1;
                        case 23:
                            return 2;
                        case 24:
                            return 3;
                        case 25:
                            return 4;
                        case 26:
                            return 5;
                        case 27:
                            return 6;
                        case 28:
                            return 7;
                        default:
                            return 8;
                    }
                case EZR.GameType.EZ2DJ:
                    switch (trackNum)
                    {
                        case 10:
                            return 0;
                        case 3:
                            return 1;
                        case 4:
                            return 2;
                        case 5:
                            return 3;
                        case 6:
                            return 4;
                        case 7:
                            return 5;
                        case 11:
                            return 6;
                        default:
                            return 8;
                    }
                case EZR.GameType.DJMAX:
                    if (mode == EZR.GameMode.Mode.FourButton ||
                    mode == EZR.GameMode.Mode.FiveButton ||
                    mode == EZR.GameMode.Mode.SixButton ||
                    mode == EZR.GameMode.Mode.FourKey ||
                    mode == EZR.GameMode.Mode.FiveKey ||
                    mode == EZR.GameMode.Mode.SixKey)
                    {
                        switch (trackNum)
                        {
                            case 3:
                                return 0;
                            case 4:
                                return 1;
                            case 5:
                                return 2;
                            case 6:
                                return 3;
                            case 7:
                                return 4;
                            case 8:
                                return 5;
                            default:
                                return 8;
                        }
                    }
                    else if (mode == EZR.GameMode.Mode.SevenKey)
                    {
                        switch (trackNum)
                        {
                            case 10:
                                return 0;
                            case 3:
                                return 1;
                            case 4:
                                return 2;
                            case 5:
                                return 3;
                            case 6:
                                return 4;
                            case 7:
                                return 5;
                            case 11:
                                return 6;
                            default:
                                return 8;
                        }
                    }
                    else if (mode == EZR.GameMode.Mode.EightKey ||
                    mode == EZR.GameMode.Mode.EightButton)
                    {
                        switch (trackNum)
                        {
                            case 10:
                                return 0;
                            case 3:
                                return 1;
                            case 4:
                                return 2;
                            case 5:
                                return 3;
                            case 6:
                                return 4;
                            case 7:
                                return 5;
                            case 8:
                                return 6;
                            case 11:
                                return 7;
                            default:
                                return 8;
                        }
                    }
                    return 8;
                default:
                    return 8;
            }
        }
    }
}