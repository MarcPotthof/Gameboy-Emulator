

using GBEmulator;
using Microsoft.Win32;
using System.ComponentModel.Design;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

internal class Program
{
    static CPU cpu = new CPU();
    static Memory memory = new Memory(cpu);
    static Joypad joypad = new Joypad();
    static PPU ppu = new PPU();
    static GBEmulator.Timer timer = new GBEmulator.Timer();

    private static void Main(string[] args)
    {
        bool powerSwitch = true;
        
        int cpuCycles = 0;

        while (powerSwitch)
        {
            while (cpuCycles < GBEmulator.Timer.CYCLES_PER_REFRESH)
            {
                int cycles = cpu.ExecuteNext();
                cpuCycles += cycles;
                ppu.update();
                timer.update(cycles, memory);
               
                for (int i = 0; i < 5; i++)
                {
                    memory.CallInterrupt((Memory.Interrupt)i);
                }
            }
            cpuCycles = 0;
        }
    }
}