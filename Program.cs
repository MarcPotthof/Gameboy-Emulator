

using GBEmulator;
using Microsoft.Win32;
using System.ComponentModel.Design;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

internal class Program
{
    private static void Main(string[] args)
    {
        byte number = 0b00000001;
        Console.WriteLine(Utils.GetBit(number, 1));
    }
}