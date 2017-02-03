namespace MoveLikeJogger.DataContracts.Utils
{
    public static class MoveUtility
    {
        public static float HumanSpeedWorldRecord = 44.71f; // Usain Bolt at 100m distance, 2009 Olympics

        public static float CalculateSpeedKmh(int distanceMeters, int durationMinutes)
        {
            if (distanceMeters <= 0 || durationMinutes <= 0)
            {
                return 0.0f;
            }

            return distanceMeters/((float) durationMinutes/60)/1000;
        }
    }
}