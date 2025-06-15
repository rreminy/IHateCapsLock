using Dalamud.Game.ClientState.Keys;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace IHateCapsLock
{
    public sealed class Plugin : IDalamudPlugin
    {
        private IDalamudPluginInterface PluginInterface { get; }
        private IKeyState KeyState { get; }
        private IPluginLog Logger { get; }

        public Plugin(IDalamudPluginInterface pluginInterface, IKeyState keyState, IPluginLog logger)
        {
            this.PluginInterface = pluginInterface;
            this.KeyState = keyState;
            this.Logger = logger;

            pluginInterface.UiBuilder.Draw += this.CheckCaps;
        }

        private void CheckCaps()
        {
            // If Return key is pressed down...
            if (this.KeyState[VirtualKey.RETURN])
            {
                // Check for caps lock state.
                var caps = Native.GetKeyState((int)VirtualKey.CAPITAL) != 0;

                // If caps is on, release caps lock.
                if (caps) this.ReleaseCapsLock();
            }
        }

        private void ReleaseCapsLock()
        {
            this.Logger.Debug("Removing CAPS LOCK!");
            Native.keybd_event((byte)VirtualKey.CAPITAL, 0x45, Constants.KEYEVENTF_EXTENDEDKEY, 0);
            Native.keybd_event((byte)VirtualKey.CAPITAL, 0x45, Constants.KEYEVENTF_EXTENDEDKEY | Constants.KEYEVENTF_KEYUP, 0);
        }

        public void Dispose()
        {
            this.PluginInterface.UiBuilder.Draw -= this.CheckCaps;
        }
    }
}
