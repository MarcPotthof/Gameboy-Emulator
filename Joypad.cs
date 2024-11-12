using System.Runtime.CompilerServices;

namespace GBEmulator
{
    public class Joypad
    {
        bool left;
        bool up;
        bool right;
        bool down;
        bool a;
        bool b;
        bool select;
        bool start;

        public byte JoypadToRegister(byte data)
        {
            if (!Utils.GetBit(data, 5))
            {
                Utils.SetBitState(data, 0, a);
                Utils.SetBitState(data, 1, b);
                Utils.SetBitState(data, 2, select);
                Utils.SetBitState(data, 3, start);
            }
            else if (!Utils.GetBit(data, 4))
            {
                Utils.SetBitState(data, 0, right);
                Utils.SetBitState(data, 1, left);
                Utils.SetBitState(data, 2, up);
                Utils.SetBitState(data, 3, down);
            }
            else data = 0x3f;
            return data;
        }
        public void SetControl(int index, bool state)
        {
            switch (index)
            {
                case 0: left = state; break;
                case 1: up = state; break;
                case 2: right = state; break;
                case 3: down = state; break;
                case 4: a = state; break;
                case 5: b = state; break;
                case 6: select = state; break;
                case 7: start = state; break;
            }
        }
    }
}