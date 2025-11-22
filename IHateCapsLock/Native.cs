using System.Runtime.InteropServices;

namespace IHateCapsLock
{
    public static partial class Native
    {
        [LibraryImport("user32.dll")]
        public static partial ushort GetKeyState(int nVirtKey);

        [LibraryImport("user32.dll")]
        public static partial void keybd_event(byte bVk, byte bScan, uint dwFlags, nuint dwExtraInfo);

        [LibraryImport("user32.dll")]
        public static partial nint GetForegroundWindow();

        [LibraryImport("user32.dll", SetLastError = true)]
        public static partial uint GetWindowThreadProcessId(nint hWnd, out int lpdwProcessId);
    }
}
