using Dalamud.Game.ClientState.Keys;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using System;

namespace IHateCapsLock
{
    public sealed class Plugin : IDalamudPlugin
    {
        private IDalamudPluginInterface PluginInterface { get; }
        private IPluginLog Logger { get; }

        public Plugin(IDalamudPluginInterface pluginInterface, IKeyState keyState, IPluginLog logger)
        {
            this.PluginInterface = pluginInterface;
            this.Logger = logger;

            pluginInterface.UiBuilder.Draw += this.CheckCaps;
        }

        private void CheckCaps()
        {
            // Make sure that the game's window is active
            if (Helper.GetActiveProcessId() != Environment.ProcessId) return;

            // If Return key is pressed down...
            if ((Native.GetKeyState((int)VirtualKey.RETURN) & 0xfffe) != 0)
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
