

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

        public ushort af { get { return (UInt16)((a << 8) | f.As_byte()); } set { a = (byte)((value & 0xff00) >> 8); f.From_byte((byte)(value & 0xff)); } }
        public ushort bc { get { return (UInt16)((b << 8) | c); } set { b = (byte)((value & 0xff00) >> 8); c = (byte)(value & 0xff); } }
        public ushort de { get { return (UInt16)((d << 8) | e); } set { d = (byte)((value & 0xff00) >> 8); e = (byte)(value & 0xff); } }
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
            return cpu.memory[cpu.pc];
        }
        public static ushort ReadShortAt(this CPU cpu, ushort index)
        {
            return (ushort)((cpu.memory[index] << 8 | cpu.memory[index + 1]));
        }
        public static ushort ReadUshort(this CPU cpu, bool readNext = false)
        {
            if (readNext) cpu.pc++;
            return (ushort)((cpu.memory[cpu.pc] << 8 | cpu.memory[cpu.pc]));
        }

        public static void WriteAt(this CPU cpu, ushort index, byte value)
        {
            cpu.memory[index] = value;
        }
        public static void WriteShortAt(this CPU cpu, ushort index, ushort value)
        {
            cpu.memory[index] = (byte)(value >> 8);
            cpu.memory[index + 1] = (byte)(value & 0xFF);
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

                case 0x36: cpu.WriteAt(cpu.registers.hl, cpu.ReadByte(true)); cpu.pc++; break;

                //LD A,n
                case 0x0A: cpu.registers.a = cpu.ReadAt(cpu.registers.bc); cpu.pc++; break;
                case 0x1A: cpu.registers.a = cpu.ReadAt(cpu.registers.de); cpu.pc++; break;
                case 0xFA: cpu.registers.a = cpu.ReadAt(cpu.ReadUshort(true)); cpu.pc += 2; break;
                case 0x3E: cpu.registers.a = cpu.ReadByte(true); cpu.pc++; break;

                //LD n,A
                case 0x47: cpu.registers.b = cpu.registers.a; cpu.pc++; break;
                case 0x4F: cpu.registers.c = cpu.registers.a; cpu.pc++; break;
                case 0x57: cpu.registers.d = cpu.registers.a; cpu.pc++; break;
                case 0x5F: cpu.registers.e = cpu.registers.a; cpu.pc++; break;
                case 0x67: cpu.registers.h = cpu.registers.a; cpu.pc++; break;
                case 0x6F: cpu.registers.l = cpu.registers.a; cpu.pc++; break;
                case 0x02: cpu.WriteAt(cpu.registers.bc, cpu.registers.a); cpu.pc++; break;
                case 0x12: cpu.WriteAt(cpu.registers.de, cpu.registers.a); cpu.pc++; break;
                case 0x77: cpu.WriteAt(cpu.registers.hl, cpu.registers.a); cpu.pc++; break;
                case 0xEA: cpu.WriteAt(cpu.ReadUshort(true), cpu.registers.a); cpu.pc += 2; break;


                case 0xF2: cpu.registers.a = cpu.ReadAt((ushort)(0xFF00 + cpu.registers.c)); cpu.pc++; break;//LD A,(C) put value at address 0xFF00 + C into A                
                case 0xE2: cpu.WriteAt((ushort)(0xFF00 + cpu.registers.c), cpu.registers.a); cpu.pc++; break;//LD (C), A put A into 0xFF00 + C                
                case 0x3A: cpu.registers.a = cpu.ReadAt(cpu.registers.hl--); cpu.pc++; break;//LDD A,(HL) the command has two alternate forms, check datasheet for them                
                case 0x2A: cpu.registers.a = cpu.ReadAt(cpu.registers.hl++); cpu.pc++; break;//LDI A,(HL) the command has two alternate forms, check datasheet for them                
                case 0x22: cpu.WriteAt(cpu.registers.hl++, cpu.registers.a); cpu.pc++; break;//LDI (HL),A
                case 0xE0: cpu.WriteAt((byte)(0xFF00 + cpu.ReadByte(true)), cpu.registers.a); break;//LDH (n),A
                case 0xF0: cpu.registers.a = cpu.ReadAt((byte)(0xFF00 + cpu.ReadByte(true))); cpu.pc++; break;//LDH A,(n)

                //16-bit Loads
                //LD n,nn
                case 0x01: cpu.registers.bc = cpu.ReadUshort(true); cpu.pc += 2; break;
                case 0x11: cpu.registers.de = cpu.ReadUshort(true); cpu.pc += 2; break;
                case 0x21: cpu.registers.hl = cpu.ReadUshort(true); cpu.pc += 2; break;
                case 0x31: cpu.sp = cpu.ReadUshort(true); cpu.pc += 2; break;

                case 0xF9: cpu.sp = cpu.registers.hl; cpu.pc++; break;//LD SP,HL
                case 0xF8: {
                        cpu.registers.f.zero = false;
                        cpu.registers.f.subtract = false;
                        SetFlagC(cpu.sp + (sbyte)cpu.ReadByte());
                        SetFlagH((byte)cpu.sp, (byte)cpu.ReadByte());
                        cpu.registers.hl = cpu.ReadAt((byte)(cpu.sp + (sbyte)cpu.ReadByte(true)));
                    } break;
                case 0x08:
                    {
                        cpu.WriteAt(cpu.ReadUshort(true), (byte)(cpu.sp >> 8));
                        cpu.WriteAt((byte)(cpu.ReadUshort() + 1), (byte)(cpu.sp & 0xff));
                    } break;//LD (nn), SP

                //PUSH nn
                case 0xF5: Push(cpu.registers.af); cpu.pc++; break;
                case 0xC5: Push(cpu.registers.bc); cpu.pc++; break;
                case 0xD5: Push(cpu.registers.de); cpu.pc++; break;
                case 0xE5: Push(cpu.registers.hl); cpu.pc++; break;
                //POP nn
                case 0xF1: cpu.registers.af = Pop(); cpu.pc++; break;
                case 0xC1: cpu.registers.bc = Pop(); cpu.pc++; break;
                case 0xD1: cpu.registers.de = Pop(); cpu.pc++; break;
                case 0xE1: cpu.registers.hl = Pop(); cpu.pc++; break;

                //8-bit ALU
                //Add A,n
                case 0x87: ADD(cpu.registers.a); cpu.pc++; break;
                case 0x80: ADD(cpu.registers.b); cpu.pc++; break;
                case 0x81: ADD(cpu.registers.c); cpu.pc++; break;
                case 0x82: ADD(cpu.registers.d); cpu.pc++; break;
                case 0x83: ADD(cpu.registers.e); cpu.pc++; break;
                case 0x84: ADD(cpu.registers.h); cpu.pc++; break;
                case 0x85: ADD(cpu.registers.l); cpu.pc++; break;
                case 0x86: ADD(cpu.ReadAt(cpu.registers.hl)); cpu.pc++; break;
                case 0xC6: ADD(cpu.ReadByte(true)); cpu.pc++; break;

                //ADC A,n
                case 0x8F: ADC(cpu.registers.a); cpu.pc++; break;
                case 0x88: ADC(cpu.registers.b); cpu.pc++; break;
                case 0x89: ADC(cpu.registers.c); cpu.pc++; break;
                case 0x8A: ADC(cpu.registers.d); cpu.pc++; break;
                case 0x8B: ADC(cpu.registers.e); cpu.pc++; break;
                case 0x8C: ADC(cpu.registers.h); cpu.pc++; break;
                case 0x8D: ADC(cpu.registers.l); cpu.pc++; break;
                case 0x8E: ADC(cpu.ReadAt(cpu.registers.hl)); cpu.pc++; break;
                case 0xCE: ADC(cpu.ReadByte(true)); cpu.pc++; break;

                //SUB n
                case 0x97: SUB(cpu.registers.a); cpu.pc++; break;
                case 0x90: SUB(cpu.registers.b); cpu.pc++; break;
                case 0x91: SUB(cpu.registers.c); cpu.pc++; break;
                case 0x92: SUB(cpu.registers.d); cpu.pc++; break;
                case 0x93: SUB(cpu.registers.e); cpu.pc++; break;
                case 0x94: SUB(cpu.registers.h); cpu.pc++; break;
                case 0x95: SUB(cpu.registers.l); cpu.pc++; break;
                case 0x96: SUB(cpu.ReadAt(cpu.registers.hl)); cpu.pc++; break;
                case 0xD6: SUB(cpu.ReadByte(true)) ; cpu.pc++; break;

                //SBC A,n
                case 0x9F: SBC(cpu.registers.a); cpu.pc++; break;
                case 0x98: SBC(cpu.registers.b); cpu.pc++; break;
                case 0x99: SBC(cpu.registers.c); cpu.pc++; break;
                case 0x9A: SBC(cpu.registers.d); cpu.pc++; break;
                case 0x9B: SBC(cpu.registers.e); cpu.pc++; break;
                case 0x9C: SBC(cpu.registers.h); cpu.pc++; break;
                case 0x9D: SBC(cpu.registers.l); cpu.pc++; break;
                case 0x9E: SBC(cpu.ReadAt(cpu.registers.hl)) ; cpu.pc++; break;
                
                //AND n
                case 0xA7: AND(cpu.registers.a); cpu.pc++; break;
                case 0xA0: AND(cpu.registers.b); cpu.pc++; break;
                case 0xA1: AND(cpu.registers.c); cpu.pc++; break;
                case 0xA2: AND(cpu.registers.d); cpu.pc++; break;
                case 0xA3: AND(cpu.registers.e); cpu.pc++; break;
                case 0xA4: AND(cpu.registers.h); cpu.pc++; break;
                case 0xA5: AND(cpu.registers.l); cpu.pc++; break;
                case 0xA6: AND(cpu.ReadAt(cpu.registers.hl)); cpu.pc++; break;
                case 0xE6: AND(cpu.ReadByte(true)); cpu.pc++; break;

                //OR n
                case 0xB7: OR(cpu.registers.a); cpu.pc++; break;
                case 0xB0: OR(cpu.registers.b); cpu.pc++; break;
                case 0xB1: OR(cpu.registers.c); cpu.pc++; break;
                case 0xB2: OR(cpu.registers.d); cpu.pc++; break;
                case 0xB3: OR(cpu.registers.e); cpu.pc++; break;
                case 0xB4: OR(cpu.registers.h); cpu.pc++; break;
                case 0xB5: OR(cpu.registers.l); cpu.pc++; break;
                case 0xB6: OR(cpu.ReadAt(cpu.registers.hl)); cpu.pc++; break;
                case 0xF6: OR(cpu.ReadByte(true)); cpu.pc++; break;

                //XOR n
                case 0xAF: XOR(cpu.registers.a); cpu.pc++; break;
                case 0xA8: XOR(cpu.registers.b); cpu.pc++; break;
                case 0xA9: XOR(cpu.registers.c); cpu.pc++; break;
                case 0xAA: XOR(cpu.registers.d); cpu.pc++; break;
                case 0xAB: XOR(cpu.registers.e); cpu.pc++; break;
                case 0xAC: XOR(cpu.registers.h); cpu.pc++; break;
                case 0xAD: XOR(cpu.registers.l); cpu.pc++; break;
                case 0xAE: XOR(cpu.ReadAt(cpu.registers.hl)); cpu.pc++; break;
                case 0xEE: XOR(cpu.ReadByte(true)); cpu.pc++; break;

                //CP n
                case 0xBF: CP(cpu.registers.a); cpu.pc++; break;
                case 0xB8: CP(cpu.registers.b); cpu.pc++; break;
                case 0xB9: CP(cpu.registers.c); cpu.pc++; break;
                case 0xBA: CP(cpu.registers.d); cpu.pc++; break;
                case 0xBB: CP(cpu.registers.e); cpu.pc++; break;
                case 0xBC: CP(cpu.registers.h); cpu.pc++; break;
                case 0xBD: CP(cpu.registers.l); cpu.pc++; break;
                case 0xBE: CP(cpu.ReadAt(cpu.registers.hl)); cpu.pc++; break;
                case 0xFE: CP(cpu.ReadByte(true)); cpu.pc++; break;

                //INC n
                case 0x3C: cpu.registers.a = INC(cpu.registers.a ); cpu.pc++; break;
                case 0x04: cpu.registers.b = INC(cpu.registers.b ); cpu.pc++; break;
                case 0x0C: cpu.registers.c = INC(cpu.registers.c ); cpu.pc++; break;
                case 0x14: cpu.registers.d = INC(cpu.registers.d ); cpu.pc++; break;
                case 0x1C: cpu.registers.e = INC(cpu.registers.e ); cpu.pc++; break;
                case 0x24: cpu.registers.h = INC(cpu.registers.h ); cpu.pc++; break;
                case 0x2C: cpu.registers.l = INC(cpu.registers.l ); cpu.pc++; break;
                case 0x34: cpu.WriteAt(cpu.registers.hl, INC(cpu.ReadAt(cpu.registers.hl))); cpu.pc++; break;

                //DEC n
                case 0x3D: cpu.registers.a = DEC(cpu.registers.a); cpu.pc++; break;
                case 0x05: cpu.registers.b = DEC(cpu.registers.b); cpu.pc++; break;
                case 0x0D: cpu.registers.c = DEC(cpu.registers.c); cpu.pc++; break;
                case 0x15: cpu.registers.d = DEC(cpu.registers.d); cpu.pc++; break;
                case 0x1D: cpu.registers.e = DEC(cpu.registers.e); cpu.pc++; break;
                case 0x25: cpu.registers.h = DEC(cpu.registers.h); cpu.pc++; break;
                case 0x2D: cpu.registers.l = DEC(cpu.registers.l); cpu.pc++; break;
                case 0x35: cpu.WriteAt(cpu.registers.hl, DEC(cpu.ReadAt(cpu.registers.hl))); cpu.pc++; break;

                //16-bit ALU
                //ADD HL, n
                case 0x09: cpu.registers.hl = ADDUShort(cpu.registers.hl,cpu.registers.bc); cpu.pc++; break;
                case 0x19: cpu.registers.hl = ADDUShort(cpu.registers.hl,cpu.registers.de); cpu.pc++; break;
                case 0x29: cpu.registers.hl = ADDUShort(cpu.registers.hl,cpu.registers.hl); cpu.pc++; break;
                case 0x39: cpu.registers.hl = ADDUShort(cpu.registers.hl,cpu.sp);           cpu.pc++; break;

                //ADD SP, n
                case 0xE8: cpu.sp = ADDUShort(cpu.sp, cpu.ReadByte(true)); cpu.pc++; break;




                default:
                    Console.WriteLine("undefined or empty opcode"); cpu.pc++;
                    throw new NotImplementedException("the opcode was either not implemented of flawed");
            }
            static ushort Combine(byte a, byte b)
            {
                return (ushort)((a << 8) | b);
            }
            void SetFlagC(int i)
            {
                cpu.registers.f.carry = (i >> 8) != 0;
            }
            void SetFlagH(byte b1, byte b2)
            {
                cpu.registers.f.half_carry = ((b1 & 0xF) + (b2 & 0xF)) > 0xF;

            }
            void SetFlagZ(int value)
            {
                cpu.registers.f.zero = value == 0;
            }
            void SetFlagHSubstract(byte b1, byte b2)
            {
                cpu.registers.f.half_carry = (b1 & 0xF) < (b2 & 0xF);
            }
                
            void Push(ushort register)
            {
                cpu.WriteShortAt(--cpu.sp, register); cpu.sp--;
            }
            ushort Pop()
            {
                ushort result = cpu.ReadShortAt((byte)(cpu.sp + 1));
                cpu.WriteAt(++cpu.sp, 0x00);
                cpu.WriteAt(++cpu.sp, 0x00);
                return result;
            }
            void ADD(byte b)
            {
                int result = cpu.registers.a + b + (cpu.registers.f.carry ? 1 : 0);
                cpu.registers.f.subtract = false;
                SetFlagZ(result);
                SetFlagH(cpu.registers.a, b);
                SetFlagC(result);
                cpu.registers.a = (byte)result;
            }
            ushort ADDUShort(ushort a, ushort b)
            {
                int result = a + b + (cpu.registers.f.carry ? 1 : 0);
                cpu.registers.f.subtract = false;
                SetFlagZ(result);
                cpu.registers.f.half_carry = (a & 0x0FFF) < (b & 0x0FFF);
                cpu.registers.f.carry = (result >> 16) != 0;
                SetFlagC(result);
                return (ushort)result;
            }
            void ADC(byte b)
            {
                int result = cpu.registers.a + b;
                cpu.registers.f.subtract = false;
                SetFlagZ(result);
                SetFlagH(cpu.registers.a, b);
                SetFlagC(result);
                cpu.registers.a = (byte)result;
            }
            void SUB(byte b)
            {
                int result = cpu.registers.a - b;
                cpu.registers.f.subtract = true;
                SetFlagZ(result);
                SetFlagHSubstract(cpu.registers.a, b);
                SetFlagC(result);
                cpu.registers.a = (byte)result;
            }
            void SBC(byte b)
            {
                int result = cpu.registers.a - b - (cpu.registers.f.carry ? 1 : 0);
                cpu.registers.f.subtract = true;
                SetFlagZ(result);
                SetFlagHSubstract(cpu.registers.a, b);
                SetFlagC(b);
                cpu.registers.a = (byte)result;
            }
            void AND(byte b)
            {
                cpu.registers.a = (byte)(cpu.registers.a & b);
                SetFlagZ(cpu.registers.a);
                cpu.registers.f.subtract = false;
                cpu.registers.f.half_carry = true;
                cpu.registers.f.carry = false;
            }
            void OR(byte b)
            {
                cpu.registers.a = (byte)(cpu.registers.a | b);
                SetFlagZ(cpu.registers.a);
                cpu.registers.f.subtract = false;
                cpu.registers.f.half_carry = false;
                cpu.registers.f.carry = false;
            }
            void XOR(byte b)
            {
                cpu.registers.a = (byte)(cpu.registers.a ^ b);
                SetFlagZ(cpu.registers.a);
                cpu.registers.f.subtract = false;
                cpu.registers.f.half_carry = false;
                cpu.registers.f.carry = false;
            }
            void CP(byte b)
            {
                int result = cpu.registers.a - b;
                cpu.registers.f.subtract = true;
                SetFlagZ(result);
                SetFlagHSubstract(cpu.registers.a, b);
                SetFlagC(result);
            }
            byte INC(byte b)
            {
                byte result = (byte)(b + 1);
                SetFlagZ(result);
                SetFlagH(b, 1);
                cpu.registers.f.subtract = false;
                return result;
            }
            byte DEC(byte b)
            {
                byte result = (byte)(b - 1);
                SetFlagZ(result);
                SetFlagHSubstract(b, 1);
                cpu.registers.f.subtract = true;
                return result;
            }
        }
    }
}