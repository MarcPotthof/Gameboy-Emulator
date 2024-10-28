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
    public class Memory
    {
        public CPU cpu;
        byte[] memory = new byte[0xFFFF];

        //Timer registers
        public byte DIV  { get { return memory[0xFF04]; } set { memory[0xFF04] = value; } } //DIV: Divider register
        public byte TIMA { get { return memory[0xFF05]; } set { memory[0xFF05] = value; } } //TIMA: Timer counter
        public byte TMA  { get { return memory[0xFF06]; } set { memory[0xFF06] = value; } } //TMA: Timer modulo
        public byte TAC  { get { return memory[0xFF07]; } set { memory[0xFF07] = value; } } //TAC Timer control

        //Interrupt registers
        public bool IME  { get;                           set;                            } //IME: Interrupt Master Enable
        public byte IE   { get { return memory[0xFFFF]; } set { memory[0xFF0F] = value; } } //IF: Interrupt Enable
        public byte IF   { get { return memory[0xFF0F]; } set { memory[0xFF0F] = value; } } //IF: Interrupt Flag
        public byte JOYP { get { return memory[0xFF00]; } set { memory[0xFF00] = value; } } //JOYP: Joypad


        public byte[] ROMBank00 { get { return memory.Take(0x4000).ToArray();  } set { memory = value.Concat(memory.Skip(0x4000)).ToArray(); } } //From cartridge, fixed bank
        public byte[] ROMBank01 { get { return memory.Skip(0x4000).Take(0x4000).ToArray(); } set { memory = memory.Take(0x4000).Concat(value).Concat(memory.Skip(0x8000)).ToArray(); } } //from cartridge, switchable bank

        public enum Interrupt
        {
            VBlank,
            STAT,
            Timer,
            Serial,
            Joypad,
        }
        public void CallInterrupt(Interrupt interrupt)
        {
            if (!IME) return;
            if (!Utils.GetBit(IE, (byte)interrupt)) return;

            IF = Utils.ResetBit(IF, (byte)interrupt);
            switch (interrupt)
            {
                case Interrupt.VBlank: cpu.WriteShortAt(--cpu.sp, cpu.pc); cpu.pc = 0x0040; break;
                case Interrupt.STAT: cpu.WriteShortAt(--cpu.sp, cpu.pc); cpu.pc = 0x0048; break;
                case Interrupt.Timer: cpu.WriteShortAt(--cpu.sp, cpu.pc); cpu.pc = 0x0050; break;
                case Interrupt.Serial: cpu.WriteShortAt(--cpu.sp, cpu.pc); cpu.pc = 0x0058; break;
                case Interrupt.Joypad: cpu.WriteShortAt(--cpu.sp, cpu.pc); cpu.pc = 0x0060; break;
            }

        }
        public void RequestInterrupt(Interrupt interrupt)
        {
            IF = Utils.SetBit(IF, (int)interrupt);
        }

        public byte this[int i]
        {
            get { return memory[i]; }
            set { memory[i] = value; }
        }
        public Memory(CPU _cpu)
        {
            cpu = _cpu;
        }
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
}