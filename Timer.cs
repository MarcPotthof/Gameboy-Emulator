namespace GBEmulator
{
    public class Timer(Main main)
    {
        public const float FREQUENCY = 4.194304f * 1000000; //4 MHz frequency
        public const float REFRESH_RATE = 59.727500569606f; //59 frames per second
        public const float CYCLES_PER_REFRESH = FREQUENCY / REFRESH_RATE;

        Main baseclass = main;

        private readonly int[] clockSelect =
        {
            1024, //00 Increment every 256 MCycles
            16, //01 Increment every 4 MCycles
            64, //10 Increment every 16 MCycles
            256, //11 Increment every 64 MCycles
        };

        int divCycles, TIMACycles;
        public void update(int cycles)
        {
            Memory memory = baseclass.memory;

            divCycles += cycles;
            while (divCycles >= 256)
            {
                memory.DIV++;
                divCycles -= 256;
            }

            TIMACycles += cycles;
            if (memory.TAC < 4)
            {
                while (TIMACycles >= clockSelect[memory.TAC])
                {
                    TIMACycles -= clockSelect[memory.TAC];
                    if (memory.TIMA >= byte.MaxValue)
                    {
                        memory.TIMA = memory.TMA;
                        memory.CallInterrupt(Memory.Interrupt.Timer);
                    }
                    else memory.TIMA++;
                }
            }
        }
    }
}