

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
    public class CPU
    {
        public Register registers;

        public ushort pc = 0x100;
        public byte[] memory = new byte[0xFFFF];

        public ushort sp = 0xFFFE;
    }
    public static class Instructions
    {
        public static ushort ExecuteInstruction(this CPU CPU, ushort pc)
        {
            switch (CPU.memory[pc])
            {
                default: Console.WriteLine("undefined or empty opcode"); return 0x0000;
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