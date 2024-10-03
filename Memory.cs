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
    public class Memory
    {
        byte[] memory = new byte[0xFFFF];

        public byte[] ROMBank00 { get { return memory.Take(0x4000).ToArray();  } set { memory = value.Concat(memory.Skip(0x4000)).ToArray(); } } //From cartridge, fixed bank
        public byte[] ROMBank01 { get { return memory.Skip(0x4000).Take(0x4000).ToArray(); } set { memory = memory.Take(0x4000).Concat(value).Concat(memory.Skip(0x8000)).ToArray(); } } //from cartridge, switchable bank

        public byte this[int i]
        {
            get { return memory[i]; }
            set { memory[i] = value; }
        }
    }
}