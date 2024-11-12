

namespace GBEmulator
{
    public class Main
    {
        public CPU cpu;
        public Joypad joypad;
        public Memory memory;
        public PPU ppu;
        public GBEmulator.Timer timer;

        public void Start()
        {
            bool powerSwitch = true;

            //main loop
            int cpuCycles = 0;

            while (powerSwitch)
            {
                while (cpuCycles < GBEmulator.Timer.CYCLES_PER_REFRESH)
                {
                    int cycles = cpu.ExecuteNext();
                    cpuCycles += cycles;
                    ppu.update();
                    timer.update(cycles);

                    for (int i = 0; i < 5; i++)
                    {
                        memory.CallInterrupt((Memory.Interrupt)i);
                    }
                }
                cpuCycles = 0;
            }
        }

        public Main()
        {
            cpu = new CPU(this);
            joypad = new Joypad();
            memory = new Memory(cpu, joypad);
            ppu = new PPU();
            timer = new GBEmulator.Timer(this);
        }
    }
}

