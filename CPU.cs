namespace GBEmulator
{
    public class CPU
    {
        public bool Halted {  get; set; }
        public Register registers;

        //program counter
        public ushort pc = 0x100;
        //program memory
        public Memory memory;
        //stack pointer
        public ushort sp = 0xFFFE;

        public CPU() 
        {
            Halted = false;
            registers = new Register();
            memory = new Memory(this);
        }
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

        public static int ExecuteInstruction(this CPU cpu)
        {
            if (cpu.Halted) return 0;
            int cycles = 0;
            cycles += Cycles.tCycles[cpu.memory[cpu.pc]];
            switch (cpu.memory[cpu.pc])
            {
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
                case 0xF8:
                    {
                        cpu.registers.f.zero = false;
                        cpu.registers.f.subtract = false;
                        SetFlagC(cpu.sp + (sbyte)cpu.ReadByte());
                        SetFlagH((byte)cpu.sp, (byte)cpu.ReadByte());
                        cpu.registers.hl = cpu.ReadAt((byte)(cpu.sp + (sbyte)cpu.ReadByte(true)));
                    }
                    break;
                case 0x08:
                    {
                        cpu.WriteAt(cpu.ReadUshort(true), (byte)(cpu.sp >> 8));
                        cpu.WriteAt((byte)(cpu.ReadUshort() + 1), (byte)(cpu.sp & 0xff));
                    }
                    break;//LD (nn), SP

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
                case 0xD6: SUB(cpu.ReadByte(true)); cpu.pc++; break;

                //SBC A,n
                case 0x9F: SBC(cpu.registers.a); cpu.pc++; break;
                case 0x98: SBC(cpu.registers.b); cpu.pc++; break;
                case 0x99: SBC(cpu.registers.c); cpu.pc++; break;
                case 0x9A: SBC(cpu.registers.d); cpu.pc++; break;
                case 0x9B: SBC(cpu.registers.e); cpu.pc++; break;
                case 0x9C: SBC(cpu.registers.h); cpu.pc++; break;
                case 0x9D: SBC(cpu.registers.l); cpu.pc++; break;
                case 0x9E: SBC(cpu.ReadAt(cpu.registers.hl)); cpu.pc++; break;

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
                case 0x3C: cpu.registers.a = INC(cpu.registers.a); cpu.pc++; break;
                case 0x04: cpu.registers.b = INC(cpu.registers.b); cpu.pc++; break;
                case 0x0C: cpu.registers.c = INC(cpu.registers.c); cpu.pc++; break;
                case 0x14: cpu.registers.d = INC(cpu.registers.d); cpu.pc++; break;
                case 0x1C: cpu.registers.e = INC(cpu.registers.e); cpu.pc++; break;
                case 0x24: cpu.registers.h = INC(cpu.registers.h); cpu.pc++; break;
                case 0x2C: cpu.registers.l = INC(cpu.registers.l); cpu.pc++; break;
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
                case 0x09: cpu.registers.hl = ADDUShort(cpu.registers.hl, cpu.registers.bc); cpu.pc++; break;
                case 0x19: cpu.registers.hl = ADDUShort(cpu.registers.hl, cpu.registers.de); cpu.pc++; break;
                case 0x29: cpu.registers.hl = ADDUShort(cpu.registers.hl, cpu.registers.hl); cpu.pc++; break;
                case 0x39: cpu.registers.hl = ADDUShort(cpu.registers.hl, cpu.sp); cpu.pc++; break;

                //ADD SP, n
                case 0xE8: cpu.sp = ADDUShort(cpu.sp, cpu.ReadByte(true)); cpu.pc++; break;

                //INC nn
                case 0x03: cpu.registers.bc = (ushort)(cpu.registers.bc + 1); cpu.pc++; break;
                case 0x13: cpu.registers.de = (ushort)(cpu.registers.de + 1); cpu.pc++; break;
                case 0x23: cpu.registers.hl = (ushort)(cpu.registers.hl + 1); cpu.pc++; break;
                case 0x33: cpu.sp = (ushort)(cpu.sp + 1); cpu.pc++; break;

                //DEC nn
                case 0x0B: cpu.registers.bc = (ushort)(cpu.registers.bc - 1); cpu.pc++; break;
                case 0x1B: cpu.registers.de = (ushort)(cpu.registers.de - 1); cpu.pc++; break;
                case 0x2B: cpu.registers.hl = (ushort)(cpu.registers.hl - 1); cpu.pc++; break;
                case 0x3B: cpu.sp = (ushort)(cpu.sp - 1); cpu.pc++; break;

                //Prefix opcodes
                case 0xCB: PrefixOpcodes(cpu.ReadByte(true)); cpu.pc++; break;

                //DAA
                case 0x27:
                    {
                        if (cpu.registers.f.subtract)
                        { // sub
                            if (cpu.registers.f.carry) { cpu.registers.a -= 0x60; }
                            if (cpu.registers.f.half_carry) { cpu.registers.a -= 0x6; }
                        }
                        else
                        { // add
                            if (cpu.registers.f.carry || (cpu.registers.a > 0x99)) { cpu.registers.a += 0x60; cpu.registers.f.carry = true; }
                            if (cpu.registers.f.half_carry || (cpu.registers.a & 0xF) > 0x9) { cpu.registers.a += 0x6; }
                        }
                        SetFlagZ(cpu.registers.a);
                        cpu.registers.f.half_carry = false;
                    }
                    cpu.pc++; break;

                //CPL
                case 0x2F:
                    {
                        cpu.registers.a = (byte)~cpu.registers.a;
                        cpu.registers.f.subtract = true;
                        cpu.registers.f.half_carry = true;
                    }
                    cpu.pc++; break;

                //CCF
                case 0x3F: cpu.registers.f.subtract = false; cpu.registers.f.half_carry = false; cpu.registers.f.carry = !cpu.registers.f.carry; cpu.pc++; break;

                //SCF
                case 0x37: cpu.registers.f.subtract = false; cpu.registers.f.half_carry = false; cpu.registers.f.carry = true; cpu.pc++; break;

                //NOP
                case 0x00: cpu.pc++; break;

                //HALT
                case 0x76: HALT(); cpu.pc++; break;

                //STOP
                case 0x10: STOP(); cpu.pc++; break;

                //DI
                case 0xF3: DisableInterrupt(); break;

                case 0xFB: EnableInterrupt(); break;

                //Rotates and Shifts
                //RLCA
                case 0x07:
                    {
                        cpu.registers.f.carry = cpu.registers.a >> 7 != 0;
                        byte result = (byte)(cpu.registers.a << 1 | cpu.registers.a >> 7);
                        SetFlagZ(result);
                        cpu.registers.a = result;
                        cpu.registers.f.subtract = false;
                        cpu.registers.f.half_carry = false;
                        cpu.pc++; break;
                    }

                //RLA
                case 0x17:
                    {
                        bool carry = cpu.registers.f.carry;
                        cpu.registers.f.carry = cpu.registers.a >> 7 != 0;
                        cpu.registers.a = (byte)(cpu.registers.a | (carry ? 1 : 0));
                        cpu.registers.f.subtract = false;
                        cpu.registers.f.half_carry = false;
                        cpu.pc++; break;
                    }

                //RRCA
                case 0x0F:
                    {
                        cpu.registers.f.carry = cpu.registers.a << 7 != 0;
                        byte result = (byte)(cpu.registers.a << 7 | cpu.registers.a >> 1);
                        SetFlagZ(result);
                        cpu.registers.a = result;
                        cpu.registers.f.subtract = false;
                        cpu.registers.f.half_carry = false;
                        cpu.pc++; break;
                    }

                //RRA
                case 0x1F:
                    {
                        bool carry = cpu.registers.f.carry;
                        cpu.registers.f.carry = cpu.registers.a << 7 != 0;
                        cpu.registers.a = (byte)(cpu.registers.a | (carry ? 1 : 0));
                        cpu.registers.f.subtract = false;
                        cpu.registers.f.half_carry = false;
                        cpu.pc++; break;
                    }

                //Jumps
                //JP nn
                case 0xC3: JPif(true); break;

                //JP cc,nn
                case 0xC2: JPif(!cpu.registers.f.subtract); break;
                case 0xCA: JPif(cpu.registers.f.zero); break;
                case 0xD2: JPif(!cpu.registers.f.carry); break;
                case 0xDA: JPif(cpu.registers.f.carry); break;

                //JP (HL)
                case 0xE9: cpu.pc = cpu.registers.hl; break;

                //JR n
                case 0x18: JRif(true); break;

                //JR cc,n
                case 0x20: JRif(!cpu.registers.f.subtract); break;
                case 0x28: JRif(cpu.registers.f.zero); break;
                case 0x30: JRif(!cpu.registers.f.carry); break;
                case 0x38: JRif(cpu.registers.f.carry); break;

                //Calls
                //CALL nn
                case 0xCD: CALLif(true); break;

                //CALL cc,nn
                case 0xC4: CALLif(!cpu.registers.f.subtract); break;
                case 0xCC: CALLif(cpu.registers.f.zero); break;
                case 0xD4: CALLif(!cpu.registers.f.carry); break;
                case 0xDC: CALLif(cpu.registers.f.carry); break;

                //RST n
                case 0xC7: RST(0x00); break;
                case 0xCF: RST(0x08); break;
                case 0xD7: RST(0x10); break;
                case 0xDF: RST(0x18); break;
                case 0xE7: RST(0x20); break;
                case 0xEF: RST(0x20); break;
                case 0xF7: RST(0x28); break;
                case 0xFF: RST(0x38); break;

                //Returns
                //RET
                case 0xC9: cpu.pc = Pop(); break;

                //RET cc
                case 0xC0: RETif(!cpu.registers.f.subtract); break;
                case 0xC8: RETif(cpu.registers.f.zero); break;
                case 0xD0: RETif(!cpu.registers.f.carry); break;
                case 0xD8: RETif(cpu.registers.f.carry); break;

                //RETI
                case 0xD9: RETif(true); EnableInterrupt(); break;

                default:
                    Console.WriteLine($"undefined or empty opcode: {cpu.memory[cpu.pc]}"); cpu.pc++; break;
            }
            return cycles;

            void PrefixOpcodes(byte opcode)
            {
                switch (opcode)
                {
                    case 0x37: cpu.registers.a = SWAP(cpu.registers.a); break;
                    case 0x30: cpu.registers.b = SWAP(cpu.registers.b); break;
                    case 0x31: cpu.registers.c = SWAP(cpu.registers.c); break;
                    case 0x32: cpu.registers.d = SWAP(cpu.registers.d); break;
                    case 0x33: cpu.registers.e = SWAP(cpu.registers.e); break;
                    case 0x34: cpu.registers.h = SWAP(cpu.registers.h); break;
                    case 0x35: cpu.registers.l = SWAP(cpu.registers.l); break;
                    case 0x36: cpu.WriteAt(cpu.registers.hl, SWAP(cpu.ReadAt(cpu.registers.hl))); break;

                    //RLC n
                    case 0x07: cpu.registers.a = RLC(cpu.registers.a); break;
                    case 0x00: cpu.registers.b = RLC(cpu.registers.b); break;
                    case 0x01: cpu.registers.c = RLC(cpu.registers.c); break;
                    case 0x02: cpu.registers.d = RLC(cpu.registers.d); break;
                    case 0x03: cpu.registers.e = RLC(cpu.registers.e); break;
                    case 0x04: cpu.registers.h = RLC(cpu.registers.h); break;
                    case 0x05: cpu.registers.l = RLC(cpu.registers.l); break;
                    case 0x06: cpu.WriteAt(cpu.registers.hl, RLC(cpu.ReadAt(cpu.registers.hl))); break;

                    //RL n
                    case 0x17: cpu.registers.a = RL(cpu.registers.a); break;
                    case 0x10: cpu.registers.b = RL(cpu.registers.b); break;
                    case 0x11: cpu.registers.c = RL(cpu.registers.c); break;
                    case 0x12: cpu.registers.d = RL(cpu.registers.d); break;
                    case 0x13: cpu.registers.e = RL(cpu.registers.e); break;
                    case 0x14: cpu.registers.h = RL(cpu.registers.h); break;
                    case 0x15: cpu.registers.l = RL(cpu.registers.l); break;
                    case 0x16: cpu.WriteAt(cpu.registers.hl, RL(cpu.ReadAt(cpu.registers.hl))); break;

                    //RRC n
                    case 0x0F: cpu.registers.a = RRC(cpu.registers.a); break;
                    case 0x08: cpu.registers.b = RRC(cpu.registers.b); break;
                    case 0x09: cpu.registers.c = RRC(cpu.registers.c); break;
                    case 0x0A: cpu.registers.d = RRC(cpu.registers.d); break;
                    case 0x0B: cpu.registers.e = RRC(cpu.registers.e); break;
                    case 0x0C: cpu.registers.h = RRC(cpu.registers.h); break;
                    case 0x0D: cpu.registers.l = RRC(cpu.registers.l); break;
                    case 0x0E: cpu.WriteAt(cpu.registers.hl, RRC(cpu.ReadAt(cpu.registers.hl))); break;

                    //RR n
                    case 0x1F: cpu.registers.a = RR(cpu.registers.a); break;
                    case 0x18: cpu.registers.b = RR(cpu.registers.b); break;
                    case 0x19: cpu.registers.c = RR(cpu.registers.c); break;
                    case 0x1A: cpu.registers.d = RR(cpu.registers.d); break;
                    case 0x1B: cpu.registers.e = RR(cpu.registers.e); break;
                    case 0x1C: cpu.registers.h = RR(cpu.registers.h); break;
                    case 0x1D: cpu.registers.l = RR(cpu.registers.l); break;
                    case 0x1E: cpu.WriteAt(cpu.registers.hl, RR(cpu.ReadAt(cpu.registers.hl))); break;

                    //SLA n
                    case 0x27: cpu.registers.a = SLA(cpu.registers.a); break;
                    case 0x20: cpu.registers.b = SLA(cpu.registers.b); break;
                    case 0x21: cpu.registers.c = SLA(cpu.registers.c); break;
                    case 0x22: cpu.registers.d = SLA(cpu.registers.d); break;
                    case 0x23: cpu.registers.e = SLA(cpu.registers.e); break;
                    case 0x24: cpu.registers.h = SLA(cpu.registers.h); break;
                    case 0x25: cpu.registers.l = SLA(cpu.registers.l); break;
                    case 0x26: cpu.WriteAt(cpu.registers.hl, SLA(cpu.ReadAt(cpu.registers.hl))); break;

                    //SRA n
                    case 0x2F: cpu.registers.a = SRA(cpu.registers.a); break;
                    case 0x28: cpu.registers.b = SRA(cpu.registers.b); break;
                    case 0x29: cpu.registers.c = SRA(cpu.registers.c); break;
                    case 0x2A: cpu.registers.d = SRA(cpu.registers.d); break;
                    case 0x2B: cpu.registers.e = SRA(cpu.registers.e); break;
                    case 0x2C: cpu.registers.h = SRA(cpu.registers.h); break;
                    case 0x2D: cpu.registers.l = SRA(cpu.registers.l); break;
                    case 0x2E: cpu.WriteAt(cpu.registers.hl, SRA(cpu.ReadAt(cpu.registers.hl))); break;

                    //SRL n
                    case 0x3F: cpu.registers.a = SRL(cpu.registers.a); break;
                    case 0x38: cpu.registers.b = SRL(cpu.registers.b); break;
                    case 0x39: cpu.registers.c = SRL(cpu.registers.c); break;
                    case 0x3A: cpu.registers.d = SRL(cpu.registers.d); break;
                    case 0x3B: cpu.registers.e = SRL(cpu.registers.e); break;
                    case 0x3C: cpu.registers.h = SRL(cpu.registers.h); break;
                    case 0x3D: cpu.registers.l = SRL(cpu.registers.l); break;
                    case 0x3E: cpu.WriteAt(cpu.registers.hl, SRL(cpu.ReadAt(cpu.registers.hl))); break;

                    //BIT b,r
                    case 0x40: BIT(0x01, cpu.registers.b); break;
                    case 0x41: BIT(0x01, cpu.registers.c); break;
                    case 0x42: BIT(0x01, cpu.registers.d); break;
                    case 0x43: BIT(0x01, cpu.registers.e); break;
                    case 0x44: BIT(0x01, cpu.registers.h); break;
                    case 0x45: BIT(0x01, cpu.registers.l); break;
                    case 0x46: BIT(0x01, cpu.ReadAt(cpu.registers.hl)); break;
                    case 0x47: BIT(0x01, cpu.registers.a); break;

                    case 0x48: BIT(0x02, cpu.registers.b); break;
                    case 0x49: BIT(0x02, cpu.registers.c); break;
                    case 0x4A: BIT(0x02, cpu.registers.d); break;
                    case 0x4B: BIT(0x02, cpu.registers.e); break;
                    case 0x4C: BIT(0x02, cpu.registers.h); break;
                    case 0x4D: BIT(0x02, cpu.registers.l); break;
                    case 0x4E: BIT(0x02, cpu.ReadAt(cpu.registers.hl)); break;
                    case 0x4F: BIT(0x02, cpu.registers.a); break;

                    case 0x50: BIT(0x04, cpu.registers.b); break;
                    case 0x51: BIT(0x04, cpu.registers.c); break;
                    case 0x52: BIT(0x04, cpu.registers.d); break;
                    case 0x53: BIT(0x04, cpu.registers.e); break;
                    case 0x54: BIT(0x04, cpu.registers.h); break;
                    case 0x55: BIT(0x04, cpu.registers.l); break;
                    case 0x56: BIT(0x04, cpu.ReadAt(cpu.registers.hl)); break;
                    case 0x57: BIT(0x04, cpu.registers.a); break;

                    case 0x58: BIT(0x08, cpu.registers.b); break;
                    case 0x59: BIT(0x08, cpu.registers.c); break;
                    case 0x5A: BIT(0x08, cpu.registers.d); break;
                    case 0x5B: BIT(0x08, cpu.registers.e); break;
                    case 0x5C: BIT(0x08, cpu.registers.h); break;
                    case 0x5D: BIT(0x08, cpu.registers.l); break;
                    case 0x5E: BIT(0x08, cpu.ReadAt(cpu.registers.hl)); break;
                    case 0x5F: BIT(0x08, cpu.registers.a); break;

                    case 0x60: BIT(0x10, cpu.registers.b); break;
                    case 0x61: BIT(0x10, cpu.registers.c); break;
                    case 0x62: BIT(0x10, cpu.registers.d); break;
                    case 0x63: BIT(0x10, cpu.registers.e); break;
                    case 0x64: BIT(0x10, cpu.registers.h); break;
                    case 0x65: BIT(0x10, cpu.registers.l); break;
                    case 0x66: BIT(0x10, cpu.ReadAt(cpu.registers.hl)); break;
                    case 0x67: BIT(0x10, cpu.registers.a); break;

                    case 0x68: BIT(0x20, cpu.registers.b); break;
                    case 0x69: BIT(0x20, cpu.registers.c); break;
                    case 0x6A: BIT(0x20, cpu.registers.d); break;
                    case 0x6B: BIT(0x20, cpu.registers.e); break;
                    case 0x6C: BIT(0x20, cpu.registers.h); break;
                    case 0x6D: BIT(0x20, cpu.registers.l); break;
                    case 0x6E: BIT(0x20, cpu.ReadAt(cpu.registers.hl)); break;
                    case 0x6F: BIT(0x20, cpu.registers.a); break;

                    case 0x70: BIT(0x40, cpu.registers.b); break;
                    case 0x71: BIT(0x40, cpu.registers.c); break;
                    case 0x72: BIT(0x40, cpu.registers.d); break;
                    case 0x73: BIT(0x40, cpu.registers.e); break;
                    case 0x74: BIT(0x40, cpu.registers.h); break;
                    case 0x75: BIT(0x40, cpu.registers.l); break;
                    case 0x76: BIT(0x40, cpu.ReadAt(cpu.registers.hl)); break;
                    case 0x77: BIT(0x40, cpu.registers.a); break;

                    case 0x78: BIT(0x80, cpu.registers.b); break;
                    case 0x79: BIT(0x80, cpu.registers.c); break;
                    case 0x7A: BIT(0x80, cpu.registers.d); break;
                    case 0x7B: BIT(0x80, cpu.registers.e); break;
                    case 0x7C: BIT(0x80, cpu.registers.h); break;
                    case 0x7D: BIT(0x80, cpu.registers.l); break;
                    case 0x7E: BIT(0x80, cpu.ReadAt(cpu.registers.hl)); break;
                    case 0x7F: BIT(0x80, cpu.registers.a); break;

                    //SET b,r
                    case 0xC0: cpu.registers.b = SET(0x01, cpu.registers.b); break;
                    case 0xC1: cpu.registers.c = SET(0x01, cpu.registers.c); break;
                    case 0xC2: cpu.registers.d = SET(0x01, cpu.registers.d); break;
                    case 0xC3: cpu.registers.e = SET(0x01, cpu.registers.e); break;
                    case 0xC4: cpu.registers.h = SET(0x01, cpu.registers.h); break;
                    case 0xC5: cpu.registers.l = SET(0x01, cpu.registers.l); break;
                    case 0xC6: cpu.WriteAt(cpu.registers.hl, SET(0x01, cpu.ReadAt(cpu.registers.hl))); break;
                    case 0xC7: cpu.registers.a = SET(0x01, cpu.registers.a); break;

                    case 0xC8: cpu.registers.b = SET(0x02, cpu.registers.b); break;
                    case 0xC9: cpu.registers.c = SET(0x02, cpu.registers.c); break;
                    case 0xCA: cpu.registers.d = SET(0x02, cpu.registers.d); break;
                    case 0xCB: cpu.registers.e = SET(0x02, cpu.registers.e); break;
                    case 0xCC: cpu.registers.h = SET(0x02, cpu.registers.h); break;
                    case 0xCD: cpu.registers.l = SET(0x02, cpu.registers.l); break;
                    case 0xCE: cpu.WriteAt(cpu.registers.hl, SET(0x02, cpu.ReadAt(cpu.registers.hl))); break;
                    case 0xCF: cpu.registers.a = SET(0x02, cpu.registers.a); break;

                    case 0xD0: cpu.registers.b = SET(0x04, cpu.registers.b); break;
                    case 0xD1: cpu.registers.c = SET(0x04, cpu.registers.c); break;
                    case 0xD2: cpu.registers.d = SET(0x04, cpu.registers.d); break;
                    case 0xD3: cpu.registers.e = SET(0x04, cpu.registers.e); break;
                    case 0xD4: cpu.registers.h = SET(0x04, cpu.registers.h); break;
                    case 0xD5: cpu.registers.l = SET(0x04, cpu.registers.l); break;
                    case 0xD6: cpu.WriteAt(cpu.registers.hl, SET(0x04, cpu.ReadAt(cpu.registers.hl))); break;
                    case 0xD7: cpu.registers.a = SET(0x04, cpu.registers.a); break;

                    case 0xD8: cpu.registers.b = SET(0x08, cpu.registers.b); break;
                    case 0xD9: cpu.registers.c = SET(0x08, cpu.registers.c); break;
                    case 0xDA: cpu.registers.d = SET(0x08, cpu.registers.d); break;
                    case 0xDB: cpu.registers.e = SET(0x08, cpu.registers.e); break;
                    case 0xDC: cpu.registers.h = SET(0x08, cpu.registers.h); break;
                    case 0xDD: cpu.registers.l = SET(0x08, cpu.registers.l); break;
                    case 0xDE: cpu.WriteAt(cpu.registers.hl, SET(0x08, cpu.ReadAt(cpu.registers.hl))); break;
                    case 0xDF: cpu.registers.a = SET(0x08, cpu.registers.a); break;

                    case 0xE0: cpu.registers.b = SET(0x10, cpu.registers.b); break;
                    case 0xE1: cpu.registers.c = SET(0x10, cpu.registers.c); break;
                    case 0xE2: cpu.registers.d = SET(0x10, cpu.registers.d); break;
                    case 0xE3: cpu.registers.e = SET(0x10, cpu.registers.e); break;
                    case 0xE4: cpu.registers.h = SET(0x10, cpu.registers.h); break;
                    case 0xE5: cpu.registers.l = SET(0x10, cpu.registers.l); break;
                    case 0xE6: cpu.WriteAt(cpu.registers.hl, SET(0x010, cpu.ReadAt(cpu.registers.hl))); break;
                    case 0xE7: cpu.registers.a = SET(0x10, cpu.registers.a); break;

                    case 0xE8: cpu.registers.b = SET(0x20, cpu.registers.b); break;
                    case 0xE9: cpu.registers.c = SET(0x20, cpu.registers.c); break;
                    case 0xEA: cpu.registers.d = SET(0x20, cpu.registers.d); break;
                    case 0xEB: cpu.registers.e = SET(0x20, cpu.registers.e); break;
                    case 0xEC: cpu.registers.h = SET(0x20, cpu.registers.h); break;
                    case 0xED: cpu.registers.l = SET(0x20, cpu.registers.l); break;
                    case 0xEE: cpu.WriteAt(cpu.registers.hl, SET(0x20, cpu.ReadAt(cpu.registers.hl))); break;
                    case 0xEF: cpu.registers.a = SET(0x20, cpu.registers.a); break;

                    case 0xF0: cpu.registers.b = SET(0x40, cpu.registers.b); break;
                    case 0xF1: cpu.registers.c = SET(0x40, cpu.registers.c); break;
                    case 0xF2: cpu.registers.d = SET(0x40, cpu.registers.d); break;
                    case 0xF3: cpu.registers.e = SET(0x40, cpu.registers.e); break;
                    case 0xF4: cpu.registers.h = SET(0x40, cpu.registers.h); break;
                    case 0xF5: cpu.registers.l = SET(0x40, cpu.registers.l); break;
                    case 0xF6: cpu.WriteAt(cpu.registers.hl, SET(0x40, cpu.ReadAt(cpu.registers.hl))); break;
                    case 0xF7: cpu.registers.a = SET(0x40, cpu.registers.a); break;

                    case 0xF8: cpu.registers.b = SET(0x80, cpu.registers.b); break;
                    case 0xF9: cpu.registers.c = SET(0x80, cpu.registers.c); break;
                    case 0xFA: cpu.registers.d = SET(0x80, cpu.registers.d); break;
                    case 0xFB: cpu.registers.e = SET(0x80, cpu.registers.e); break;
                    case 0xFC: cpu.registers.h = SET(0x80, cpu.registers.h); break;
                    case 0xFD: cpu.registers.l = SET(0x80, cpu.registers.l); break;
                    case 0xFE: cpu.WriteAt(cpu.registers.hl, SET(0x80, cpu.ReadAt(cpu.registers.hl))); break;
                    case 0xFF: cpu.registers.a = SET(0x80, cpu.registers.a); break;

                    //RES b,r
                    case 0x80: cpu.registers.b = RES(0x01, cpu.registers.b); break;
                    case 0x81: cpu.registers.c = RES(0x01, cpu.registers.c); break;
                    case 0x82: cpu.registers.d = RES(0x01, cpu.registers.d); break;
                    case 0x83: cpu.registers.e = RES(0x01, cpu.registers.e); break;
                    case 0x84: cpu.registers.h = RES(0x01, cpu.registers.h); break;
                    case 0x85: cpu.registers.l = RES(0x01, cpu.registers.l); break;
                    case 0x86: cpu.WriteAt(cpu.registers.hl, RES(0x01, cpu.ReadAt(cpu.registers.hl))); break;
                    case 0x87: cpu.registers.a = RES(0x01, cpu.registers.a); break;

                    case 0x88: cpu.registers.b = RES(0x02, cpu.registers.b); break;
                    case 0x89: cpu.registers.c = RES(0x02, cpu.registers.c); break;
                    case 0x8A: cpu.registers.d = RES(0x02, cpu.registers.d); break;
                    case 0x8B: cpu.registers.e = RES(0x02, cpu.registers.e); break;
                    case 0x8C: cpu.registers.h = RES(0x02, cpu.registers.h); break;
                    case 0x8D: cpu.registers.l = RES(0x02, cpu.registers.l); break;
                    case 0x8E: cpu.WriteAt(cpu.registers.hl, RES(0x02, cpu.ReadAt(cpu.registers.hl))); break;
                    case 0x8F: cpu.registers.a = RES(0x02, cpu.registers.a); break;

                    case 0x90: cpu.registers.b = RES(0x04, cpu.registers.b); break;
                    case 0x91: cpu.registers.c = RES(0x04, cpu.registers.c); break;
                    case 0x92: cpu.registers.d = RES(0x04, cpu.registers.d); break;
                    case 0x93: cpu.registers.e = RES(0x04, cpu.registers.e); break;
                    case 0x94: cpu.registers.h = RES(0x04, cpu.registers.h); break;
                    case 0x95: cpu.registers.l = RES(0x04, cpu.registers.l); break;
                    case 0x96: cpu.WriteAt(cpu.registers.hl, RES(0x04, cpu.ReadAt(cpu.registers.hl))); break;
                    case 0x97: cpu.registers.a = SET(0x04, cpu.registers.a); break;

                    case 0x98: cpu.registers.b = SET(0x08, cpu.registers.b); break;
                    case 0x99: cpu.registers.c = SET(0x08, cpu.registers.c); break;
                    case 0x9A: cpu.registers.d = SET(0x08, cpu.registers.d); break;
                    case 0x9B: cpu.registers.e = SET(0x08, cpu.registers.e); break;
                    case 0x9C: cpu.registers.h = SET(0x08, cpu.registers.h); break;
                    case 0x9D: cpu.registers.l = SET(0x08, cpu.registers.l); break;
                    case 0x9E: cpu.WriteAt(cpu.registers.hl, SET(0x08, cpu.ReadAt(cpu.registers.hl))); break;
                    case 0x9F: cpu.registers.a = SET(0x08, cpu.registers.a); break;

                    case 0xA0: cpu.registers.b = SET(0x10, cpu.registers.b); break;
                    case 0xA1: cpu.registers.c = SET(0x10, cpu.registers.c); break;
                    case 0xA2: cpu.registers.d = SET(0x10, cpu.registers.d); break;
                    case 0xA3: cpu.registers.e = SET(0x10, cpu.registers.e); break;
                    case 0xA4: cpu.registers.h = SET(0x10, cpu.registers.h); break;
                    case 0xA5: cpu.registers.l = SET(0x10, cpu.registers.l); break;
                    case 0xA6: cpu.WriteAt(cpu.registers.hl, SET(0x010, cpu.ReadAt(cpu.registers.hl))); break;
                    case 0xA7: cpu.registers.a = SET(0x10, cpu.registers.a); break;

                    case 0xA8: cpu.registers.b = SET(0x20, cpu.registers.b); break;
                    case 0xA9: cpu.registers.c = SET(0x20, cpu.registers.c); break;
                    case 0xAA: cpu.registers.d = SET(0x20, cpu.registers.d); break;
                    case 0xAB: cpu.registers.e = SET(0x20, cpu.registers.e); break;
                    case 0xAC: cpu.registers.h = SET(0x20, cpu.registers.h); break;
                    case 0xAD: cpu.registers.l = SET(0x20, cpu.registers.l); break;
                    case 0xAE: cpu.WriteAt(cpu.registers.hl, SET(0x20, cpu.ReadAt(cpu.registers.hl))); break;
                    case 0xAF: cpu.registers.a = SET(0x20, cpu.registers.a); break;

                    case 0xB0: cpu.registers.b = SET(0x40, cpu.registers.b); break;
                    case 0xB1: cpu.registers.c = SET(0x40, cpu.registers.c); break;
                    case 0xB2: cpu.registers.d = SET(0x40, cpu.registers.d); break;
                    case 0xB3: cpu.registers.e = SET(0x40, cpu.registers.e); break;
                    case 0xB4: cpu.registers.h = SET(0x40, cpu.registers.h); break;
                    case 0xB5: cpu.registers.l = SET(0x40, cpu.registers.l); break;
                    case 0xB6: cpu.WriteAt(cpu.registers.hl, SET(0x40, cpu.ReadAt(cpu.registers.hl))); break;
                    case 0xB7: cpu.registers.a = SET(0x40, cpu.registers.a); break;

                    case 0xB8: cpu.registers.b = SET(0x80, cpu.registers.b); break;
                    case 0xB9: cpu.registers.c = SET(0x80, cpu.registers.c); break;
                    case 0xBA: cpu.registers.d = SET(0x80, cpu.registers.d); break;
                    case 0xBB: cpu.registers.e = SET(0x80, cpu.registers.e); break;
                    case 0xBC: cpu.registers.h = SET(0x80, cpu.registers.h); break;
                    case 0xBD: cpu.registers.l = SET(0x80, cpu.registers.l); break;
                    case 0xBE: cpu.WriteAt(cpu.registers.hl, SET(0x80, cpu.ReadAt(cpu.registers.hl))); break;
                    case 0xBF: cpu.registers.a = SET(0x80, cpu.registers.a); break;
                }
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
            byte SWAP(byte b)
            {
                int result = (b >> 4 | b << 4);
                SetFlagZ(result);
                cpu.registers.f.subtract = false;
                cpu.registers.f.carry = false;
                cpu.registers.f.half_carry = false;
                return (byte)result;
            }
            byte RLC(byte b)
            {
                int result = b << 1 | b >> 7;
                SetFlagZ(result);
                cpu.registers.f.subtract = false;
                cpu.registers.f.half_carry = false;
                cpu.registers.f.carry = b >> 7 != 0;
                return (byte)result;
            }
            byte RL(byte b)
            {
                bool carry = cpu.registers.f.carry;
                cpu.registers.f.carry = cpu.registers.a >> 7 != 0;
                byte result = (byte)(cpu.registers.a | (carry ? 1 : 0));
                SetFlagZ(result);
                cpu.registers.f.subtract = false;
                cpu.registers.f.half_carry = false;
                return result;
            }
            byte RRC(byte b)
            {
                int result = b >> 1 | b << 7;
                SetFlagZ(result);
                cpu.registers.f.subtract = false;
                cpu.registers.f.half_carry = false;
                cpu.registers.f.carry = (b & 1) != 0;
                return (byte)result;
            }
            byte RR(byte b)
            {
                bool carry = cpu.registers.f.carry;
                cpu.registers.f.carry = (b & 1) != 0;
                byte result = (byte)(cpu.registers.a | (carry ? 1 : 0) << 7);
                SetFlagZ(result);
                cpu.registers.f.subtract = false;
                cpu.registers.f.half_carry = false;
                return result;
            }
            byte SLA(byte b)
            {
                int result = b << 1;
                SetFlagZ(result);
                cpu.registers.f.subtract = false;
                cpu.registers.f.half_carry = false;
                cpu.registers.f.carry = b >> 7 != 0;
                return (byte)result;
            }
            byte SRA(byte b)
            {
                byte result = (byte)((b >> 1) | (b & 0x80));
                SetFlagZ(result);
                cpu.registers.f.subtract = false;
                cpu.registers.f.half_carry = false;
                cpu.registers.f.carry = b >> 7 != 0;
                return (byte)result;
            }
            byte SRL(byte b)
            {
                byte result = (byte)(b >> 1);
                SetFlagZ(result);
                cpu.registers.f.subtract = false;
                cpu.registers.f.half_carry = false;
                cpu.registers.f.carry = b >> 7 != 0;
                return (byte)result;
            }
            void BIT(byte bitmask, byte b)
            {
                cpu.registers.f.zero = (bitmask & b) == 0;
                cpu.registers.f.half_carry = true;
                cpu.registers.f.subtract = false;
            }
            byte SET(byte bitmask, byte b)
            {
                return (byte)(b | bitmask);
            }
            byte RES(byte bitmask, byte b)
            {
                return (byte)(b & ~bitmask);
            }
            void JPif(bool jump)
            {
                if (jump) { cpu.pc = (ushort)(cpu.ReadByte(true) & cpu.ReadByte(true) << 8); cycles += Cycles.Jump_To_True; }
                else { cpu.pc++; cycles += Cycles.Jump_To_False; }
            }
            void JRif(bool jump)
            {
                if (jump) { cpu.pc += cpu.ReadAt((ushort)(cpu.pc + 1)); cycles += Cycles.Jump_True; }
                else cycles += Cycles.Jump_False;
            }
            void CALLif(bool call)
            {
                if (call) { Push((byte)(cpu.pc + 1)); cpu.pc = cpu.ReadUshort(true); cycles += Cycles.Call_True; }
                else cycles += Cycles.Call_False;
            }
            void RST(byte n)
            {
                Push(cpu.pc);
                cpu.pc = n;
            }
            void RETif(bool ret)
            {
                if (ret) { cpu.pc = Pop(); cycles += Cycles.Return_True; }
                else
                {
                    cycles += Cycles.Return_False;
                }
            }


            void HALT()
            {
                cpu.Halted = true;
                return;
            }
            void STOP()
            {
                throw new NotImplementedException();
            }
            void DisableInterrupt()
            {
                cpu.memory.IME = false;
            }
            void EnableInterrupt()
            {
                cpu.memory.IME = true;
            }
        }
    }

    static class Cycles
    {
        public static readonly int Jump_True = 12;
        public static readonly int Jump_False = 8;
        public static readonly int Return_True = 20;
        public static readonly int Return_False = 8;
        public static readonly int Call_True = 24;
        public static readonly int Call_False = 12;
        public static readonly int Jump_To_True = 16;
        public static readonly int Jump_To_False = 12;

        public static readonly int[] tCycles =
        {
            //0 values are conditional and listed above,
            //00 values are unused codes
            //0 , 1 , 2 , 3 , 4 , 5 , 6 , 7 , 8 , 9 , A , B , C , D , E , F
              4 , 12, 8 , 8 , 4 , 4 , 8 , 4 , 20, 8 , 8 , 8 , 4 , 4 , 8 , 4 , //0
	          4 , 12, 8 , 8 , 4 , 4 , 8 , 4 , 0 , 8 , 8 , 8 , 4 , 4 , 8 , 4 , //1
              0 , 12, 8 , 8 , 4 , 4 , 8 , 4 , 0 , 8 , 8 , 8 , 4 , 4 , 8 , 4 , //2
              0 , 12, 8 , 8 , 12, 12, 12, 4 , 0 , 8 , 8 , 8 , 4 , 4 , 8 , 4 , //3 

              4 , 4 , 4 , 4 , 4 , 4 , 8 , 4 , 4 , 4 , 4 , 4 , 4 , 4 , 8 , 4 , //4
	          4 , 4 , 4 , 4 , 4 , 4 , 8 , 4 , 4 , 4 , 4 , 4 , 4 , 4 , 8 , 4 , //5
              4 , 4 , 4 , 4 , 4 , 4 , 8 , 4 , 4 , 4 , 4 , 4 , 4 , 4 , 8 , 4 , //6
              8 , 8 , 8 , 8 , 8 , 8 , 4 , 8 , 4 , 4 , 4 , 4 , 4 , 4 , 8 , 4 , //7
 
              4 , 4 , 4 , 4 , 4 , 4 , 8 , 4 , 4 , 4 , 4 , 4 , 4 , 4 , 8 , 4 , //8
	          4 , 4 , 4 , 4 , 4 , 4 , 8 , 4 , 4 , 4 , 4 , 4 , 4 , 4 , 8 , 4 , //9
              4 , 4 , 4 , 4 , 4 , 4 , 8 , 4 , 4 , 4 , 4 , 4 , 4 , 4 , 8 , 4 , //A
              4 , 4 , 4 , 4 , 4 , 4 , 8 , 4 , 4 , 4 , 4 , 4 , 4 , 4 , 8 , 4 , //B

              0 , 12, 0 , 0 , 0 , 16, 8 , 16, 0 , 4 , 0 , 0 , 0 , 0 , 8 , 16, //C
	          0 , 12, 0 , 00, 0 , 16, 8 , 16, 0 , 4 , 0 , 00, 0 , 00, 8 , 16, //D
              12, 12, 8 , 00, 00, 16, 8 , 16, 16, 4 , 16, 00, 00, 00, 8 , 16, //E
              12, 12, 8 , 4 , 00, 16, 8 , 16, 12, 8 , 16, 4 , 00, 00, 8 , 16, //F
        };

        public static readonly int[] prefixCycles =
        {
            //0   1   2   3   4   5   6   7   8   9   A   B   C   D   E   F
	          8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , 8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , //0
              8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , 8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , //1
              8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , 8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , //2
              8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , 8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , //3
                                                   
              8 , 8 , 8 , 8 , 8 , 8 , 12, 8 , 8 , 8 , 8 , 8 , 8 , 8 , 12, 8 , //4
              8 , 8 , 8 , 8 , 8 , 8 , 12, 8 , 8 , 8 , 8 , 8 , 8 , 8 , 12, 8 , //5
              8 , 8 , 8 , 8 , 8 , 8 , 12, 8 , 8 , 8 , 8 , 8 , 8 , 8 , 12, 8 , //6
              8 , 8 , 8 , 8 , 8 , 8 , 12, 8 , 8 , 8 , 8 , 8 , 8 , 8 , 12, 8 , //7
                                                   
              8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , 8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , //8
              8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , 8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , //9
              8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , 8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , //A
              8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , 8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , //B
                                                  
              8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , 8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , //C
              8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , 8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , //D
              8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , 8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , //E
              8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , 8 , 8 , 8 , 8 , 8 , 8 , 16, 8 , //F
        };
    }
}