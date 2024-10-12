namespace GBEmulator
{
    public class Timer
    {
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