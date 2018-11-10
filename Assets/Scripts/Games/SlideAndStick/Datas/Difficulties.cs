namespace SlideAndStick {
    public static class Difficulties {
        public const int Undefined = 0;
        public const int Beginner = 1;
        public const int Easy = 2;
        public const int Med = 3;
        public const int Hard = 4;
        public const int DoubleHard = 5;
        public const int Impossible = 6;
        public const int DoubleImpossible = 7;
    
        public static string GetName(int difficulty) {
            switch (difficulty) {
                case Beginner:      return "beginner";
                case Easy:          return "easy";
                case Med:           return "med";
                case Hard:          return "hard";
                case DoubleHard:    return "double hard";
                case Impossible:    return "impossible";
                case DoubleImpossible: return "double impossible";
                default:            return "undefined";
            }
        }
    }
}