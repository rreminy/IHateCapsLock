using System;
using System.Runtime.InteropServices;

namespace IHateCapsLock
{
    public static partial class Native
    {
        [LibraryImport("user32.dll")]
        public static partial ushort GetKeyState(int nVirtKey);

        [LibraryImport("user32.dll")]
        public static partial void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
    }
}
