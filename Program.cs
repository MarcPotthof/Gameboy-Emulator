

using Microsoft.Win32;
using System.Runtime.CompilerServices;

internal class Program
{
    private static void Main(string[] args)
    {
        byte num1 = byte.MaxValue;
        byte num2 = 25;
        Console.WriteLine(checked((byte)(num1 + num2)));
    }
}

namespace GBEmulator
{
    public struct Register
    {
        public byte a;
        public byte b;
        public byte c;
        public byte d;
        public byte e;
        public FlagRegister f;
        public byte h;
        public byte l;

        public ushort af { get { return (UInt16)((a << 8) | f.As_byte()); } set {a = (byte)((value & 0xff00) >> 8);f.From_byte((byte)(value & 0xff));} }
        public ushort bc { get { return (UInt16)((b << 8) | c); } set { b = (byte)((value & 0xff00) >> 8);c = (byte)(value & 0xff);} }
        public ushort de { get { return (UInt16)((d << 8) | e); } set { d = (byte)((value & 0xff00) >> 8);e = (byte)(value & 0xff);} }
        public ushort hl { get { return (UInt16)((h << 8) | l); } set { h = (byte)((value & 0xff00) >> 8); l = (byte)(value & 0xff); } }
    }
    public struct FlagRegister
    {
        public const byte zero_flag_position = 7;
        public const byte subtract_flag_position = 6;
        public const byte half_carry_flag_position = 5;
        public const byte carry_flag_position = 4;

        public bool zero;
        public bool subtract;
        public bool half_carry;
        public bool carry;
    }
    public static class RegisterExtensions
    {
        //Enabling the Flagsregister to be used as a byte
        public static byte As_byte(this FlagRegister flags)
        {
            return (byte)((flags.zero ? 1 : 0 << FlagRegister.zero_flag_position) |
                          (flags.subtract ? 1 : 0 << FlagRegister.subtract_flag_position) |
                          (flags.half_carry ? 1 : 0 << FlagRegister.half_carry_flag_position) |
                          (flags.carry ? 1 : 0 << FlagRegister.carry_flag_position));
        }
        public static void From_byte(this FlagRegister flags, byte value)
        {
            flags.zero = (value >> FlagRegister.zero_flag_position & 0x1) == 1;
            flags.subtract = (value >> FlagRegister.subtract_flag_position & 0x1) == 1;
            flags.half_carry = (value >> FlagRegister.half_carry_flag_position & 0x1) == 1;
            flags.carry = (value >> FlagRegister.carry_flag_position & 0x1) == 1;
        }

        //virtual registers: treat 2 seperate 8 bit registers as one 16 bit
        public static UInt16 Get_af(this Register register)
        {
            return (UInt16)((register.a << 8) | register.f.As_byte());
        }
        public static UInt16 Get_bc(this Register register)
        {
            return (UInt16)((register.b << 8) | register.c);
        }
        public static UInt16 Get_de(this Register register)
        {
            return (UInt16)((register.d << 8) | register.e);
        }
        public static UInt16 Get_hl(this Register register)
        {
            return (UInt16)((register.h << 8) | register.l);
        }

        public static void Set_af(this Register register, UInt16 value)
        {
            register.a = (byte)((value & 0xff00) >> 8);
            register.f.From_byte((byte)(value & 0xff));
        }
        public static void Set_bc(this Register register, UInt16 value)
        {
            register.b = (byte)((value & 0xff00) >> 8);
            register.c = (byte)(value & 0xff);
        }
        public static void Set_de(this Register register, UInt16 value)
        {
            register.d = (byte)((value & 0xff00) >> 8);
            register.e = (byte)(value & 0xff);
        }
        public static void Set_hl(this Register register, UInt16 value)
        {
            register.h = (byte)((value & 0xff00) >> 8);
            register.l = (byte)(value & 0xff);
        }
    }
    public struct CPU
    {
        public Register registers;

        //program counter
        public ushort pc = 0x100;
        //program memory
        public byte[] memory = new byte[0xFFFF];
        //stack pointer
        public ushort sp = 0xFFFE;

        public CPU() { }
    }
    public static class Instructions
    {
        public static byte ReadAt(this CPU cpu, ushort index)
        {
            return cpu.memory[index]; 
        }
        public static byte ReadByte(this CPU cpu, bool readNext = false)
        {
            if (readNext) cpu.pc++;
            return cpu.memory[cpu.pc++];
        }
        public static ushort ReadShortAt(this CPU cpu, ushort index)
        { 
            return (ushort)((cpu.memory[index] << 8 | cpu.memory[index + 1]));
        }
        public static ushort ReadUshort(this CPU cpu, bool readNext = false)
        {
            if (readNext) cpu.pc++;
            return (ushort)((cpu.memory[cpu.pc++] << 8 | cpu.memory[cpu.pc++]));
        }

        public static void WriteAt(this CPU cpu, ushort index, byte value)
        {

        }

        public static void ExecuteInstruction(this CPU cpu)
        {
            switch (cpu.memory[cpu.pc]) 
            {
                //Instruction list:
                //http://marc.rawer.de/Gameboy/Docs/GBCPUman.pdf

                //8-Bit Loads

                //LD nn,n
                case 0x06: cpu.registers.b = cpu.ReadByte(true); cpu.pc++; break;
                case 0x0E: cpu.registers.c = cpu.ReadByte(true); cpu.pc++; break;
                case 0x16: cpu.registers.d = cpu.ReadByte(true); cpu.pc++; break;
                case 0x1E: cpu.registers.e = cpu.ReadByte(true); cpu.pc++; break;
                case 0x26: cpu.registers.h = cpu.ReadByte(true); cpu.pc++; break;
                case 0x2E: cpu.registers.l = cpu.ReadByte(true); cpu.pc++; break;
                //LD r1,r2
                case 0x7F: cpu.registers.a = cpu.registers.a; cpu.pc++; break;
                case 0x78: cpu.registers.a = cpu.registers.b; cpu.pc++; break;
                case 0x79: cpu.registers.a = cpu.registers.c; cpu.pc++; break;
                case 0x7A: cpu.registers.a = cpu.registers.d; cpu.pc++; break;
                case 0x7B: cpu.registers.a = cpu.registers.e; cpu.pc++; break;
                case 0x7C: cpu.registers.a = cpu.registers.h; cpu.pc++; break;
                case 0x7D: cpu.registers.a = cpu.registers.l; cpu.pc++; break;
                case 0x7E: cpu.registers.a = cpu.ReadAt(cpu.registers.hl); cpu.pc++; break;

                case 0x40: cpu.registers.b = cpu.registers.b; cpu.pc++; break;
                case 0x41: cpu.registers.b = cpu.registers.c; cpu.pc++; break;
                case 0x42: cpu.registers.b = cpu.registers.d; cpu.pc++; break;
                case 0x43: cpu.registers.b = cpu.registers.e; cpu.pc++; break;
                case 0x44: cpu.registers.b = cpu.registers.h; cpu.pc++; break;
                case 0x45: cpu.registers.b = cpu.registers.l; cpu.pc++; break;
                case 0x46: cpu.registers.b = cpu.ReadAt(cpu.registers.hl); cpu.pc++; break;

                case 0x48: cpu.registers.c = cpu.registers.b; cpu.pc++; break;
                case 0x49: cpu.registers.c = cpu.registers.c; cpu.pc++; break;
                case 0x4A: cpu.registers.c = cpu.registers.d; cpu.pc++; break;
                case 0x4B: cpu.registers.c = cpu.registers.e; cpu.pc++; break;
                case 0x4C: cpu.registers.c = cpu.registers.h; cpu.pc++; break;
                case 0x4D: cpu.registers.c = cpu.registers.l; cpu.pc++; break;
                case 0x4E: cpu.registers.c = cpu.ReadAt(cpu.registers.hl); cpu.pc++; break;

                case 0x50: cpu.registers.d = cpu.registers.b; cpu.pc++; break;
                case 0x51: cpu.registers.d = cpu.registers.c; cpu.pc++; break;
                case 0x52: cpu.registers.d = cpu.registers.d; cpu.pc++; break;
                case 0x53: cpu.registers.d = cpu.registers.e; cpu.pc++; break;
                case 0x54: cpu.registers.d = cpu.registers.h; cpu.pc++; break;
                case 0x55: cpu.registers.d = cpu.registers.l; cpu.pc++; break;
                case 0x56: cpu.registers.d = cpu.ReadAt(cpu.registers.hl); cpu.pc++; break;

                case 0x58: cpu.registers.e = cpu.registers.b; cpu.pc++; break;
                case 0x59: cpu.registers.e = cpu.registers.c; cpu.pc++; break;
                case 0x5A: cpu.registers.e = cpu.registers.d; cpu.pc++; break;
                case 0x5B: cpu.registers.e = cpu.registers.e; cpu.pc++; break;
                case 0x5C: cpu.registers.e = cpu.registers.h; cpu.pc++; break;
                case 0x5D: cpu.registers.e = cpu.registers.l; cpu.pc++; break;
                case 0x5E: cpu.registers.e = cpu.ReadAt(cpu.registers.hl); cpu.pc++; break;

                case 0x60: cpu.registers.h = cpu.registers.b; cpu.pc++; break;
                case 0x61: cpu.registers.h = cpu.registers.c; cpu.pc++; break;
                case 0x62: cpu.registers.h = cpu.registers.d; cpu.pc++; break;
                case 0x63: cpu.registers.h = cpu.registers.e; cpu.pc++; break;
                case 0x64: cpu.registers.h = cpu.registers.h; cpu.pc++; break;
                case 0x65: cpu.registers.h = cpu.registers.l; cpu.pc++; break;
                case 0x66: cpu.registers.h = cpu.ReadAt(cpu.registers.hl); cpu.pc++; break;

                case 0x68: cpu.registers.l = cpu.registers.b; cpu.pc++; break;
                case 0x69: cpu.registers.l = cpu.registers.c; cpu.pc++; break;
                case 0x6A: cpu.registers.l = cpu.registers.d; cpu.pc++; break;
                case 0x6B: cpu.registers.l = cpu.registers.e; cpu.pc++; break;
                case 0x6C: cpu.registers.l = cpu.registers.h; cpu.pc++; break;
                case 0x6D: cpu.registers.l = cpu.registers.l; cpu.pc++; break;
                case 0x6E: cpu.registers.l = cpu.ReadAt(cpu.registers.hl); cpu.pc++; break;

                case 0x70: cpu.WriteAt(cpu.registers.hl, cpu.registers.b); cpu.pc++; break;
                case 0x71: cpu.WriteAt(cpu.registers.hl, cpu.registers.c); cpu.pc++; break;
                case 0x72: cpu.WriteAt(cpu.registers.hl, cpu.registers.d); cpu.pc++; break;
                case 0x73: cpu.WriteAt(cpu.registers.hl, cpu.registers.e); cpu.pc++; break;
                case 0x74: cpu.WriteAt(cpu.registers.hl, cpu.registers.h); cpu.pc++; break;
                case 0x75: cpu.WriteAt(cpu.registers.hl, cpu.registers.l); cpu.pc++; break;

                case 0x36: cpu.registers.hl = cpu.ReadByte(true); break;

                //LD A,n
                case 0x0A: cpu.registers.a = (byte)(cpu.registers.bc >> 8); cpu.pc++; break;
                case 0x1A: cpu.registers.a = (byte)

                default: Console.WriteLine("undefined or empty opcode"); cpu.pc++; break;
            }
        }

        //stack operations
        public static void Push(this CPU CPU, byte value)
        {

        }
        public static void Call(this CPU CPU, byte value)
        {

        }
        public static void Rst(this CPU CPU, byte value)
        {

        }

        public static byte Pop(this CPU CPU)
        {
            return 0x00;
        }
        public static byte Ret(this CPU CPU)
        {
            return 0x00;
        }
        public static byte Reti(this CPU CPU)
        {
            return 0x00;
        }
    }
}