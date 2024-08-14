using System.Runtime.CompilerServices;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}

namespace Emulator
{
    public struct Register
    {
        public byte a;
        public byte b;
        public byte c;
        public byte d;
        public byte e;
        public FlagsRegister f;
        public byte h;
        public byte l;
    }
    public struct FlagsRegister
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
    public static class RegisterOperations
    {
        //FlagsRegister: leaves out the unused 4 bits of the register
        public static byte As_byte(this FlagsRegister flags)
        {
            return (byte)((flags.zero ? 1 : 0 << FlagsRegister.zero_flag_position) |
                          (flags.subtract ? 1 : 0 << FlagsRegister.subtract_flag_position) |
                          (flags.half_carry ? 1 : 0 << FlagsRegister.half_carry_flag_position) |
                          (flags.carry ? 1 : 0 << FlagsRegister.carry_flag_position));
        }
        public static void From_byte(this FlagsRegister flags, byte value)
        {
            flags.zero = (value >> FlagsRegister.zero_flag_position & 0x1) == 1;
            flags.subtract = (value >> FlagsRegister.subtract_flag_position & 0x1) == 1;
            flags.half_carry = (value >> FlagsRegister.half_carry_flag_position & 0x1) == 1;
            flags.carry = (value >> FlagsRegister.carry_flag_position & 0x1) == 1;
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
}