namespace GBEmulator
{
    public class Timer
    {
        public const float FREQUENCY = 4.194304f * 1000000; //4 MHz frequency
        public const float REFRESH_RATE = 59.727500569606f; //59 frames per second
        public const float CYCLES_PER_REFRESH = FREQUENCY / REFRESH_RATE;

        private readonly int[] clockSelect =
        {
            1024, //00 Increment every 256 MCycles
            16, //01 Increment every 4 MCycles
            64, //10 Increment every 16 MCycles
            256, //11 Increment every 64 MCycles
        };
        public void update(int cycles, Memory memory)
        {
             
        }
    }
}