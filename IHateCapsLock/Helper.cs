namespace IHateCapsLock
{
    public static class Helper
    {
        public static int GetActiveProcessId()
        {
            var hWnd = Native.GetForegroundWindow();
            if (hWnd is 0) return 0;

            _ = Native.GetWindowThreadProcessId(hWnd, out var pid);
            return pid;
        }
    }
}
