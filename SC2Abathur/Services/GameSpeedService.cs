namespace SC2Abathur.Services
{
    public static class GameSpeedService {
        public static int MillisecondsBetweenSteps(GameSpeed speed)
            => (int) (1f / GameloopsPerSecond(speed) * 1000f);
        public static float GameloopsPerSecond(GameSpeed speed) {
            switch(speed) {
                case GameSpeed.Slower:
                    return 9.6f;
                case GameSpeed.Slow:
                    return 12.8f;
                case GameSpeed.Normal:
                    return 16f;
                case GameSpeed.Fast:
                    return 19.2f;
                case GameSpeed.Faster:
                    return 22.4f;
                case GameSpeed.MaximumSpeed:
                    return float.MaxValue;
                default:
                    throw new System.NotImplementedException();
            }
        }
        public enum GameSpeed { Slower, Slow, Normal, Fast, Faster, MaximumSpeed }
    }
}
