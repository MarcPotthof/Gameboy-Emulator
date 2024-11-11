namespace GBEmulator
{
    class Utils
    {
        public static bool GetBit(byte number, int bit)
        {
            return (number << 7 - bit) >> 7 != 0;
        }
        public static byte SetBit(byte number, int bit)
        {
            return (byte)(number & (1 << bit));
        }
        public static byte ResetBit(byte number, int bit)
        {
            return (byte)(number !& (1 << bit));
        }
        public static byte SetBitState(byte number, int bit, bool state)
        {
            return state ? SetBit(number, bit) : SetBit(number, bit);
        }
    }
}