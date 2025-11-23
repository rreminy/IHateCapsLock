using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using IHateCapsLock.Gui;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace IHateCapsLock
{
    public sealed class Plugin : IDalamudPlugin
    {
        private const long ExceptionDelay = 2000;

        private readonly Config _config;
        private readonly WindowSystem _windows;
        private readonly ConfigWindow _configWindow;
        private Thread? _thread;
        private bool _disposed;
        private long _lastExceptionTicks;

        private IDalamudPluginInterface PluginInterface { get; }
        private IPluginLog Logger { get; }

        public Plugin(IDalamudPluginInterface pluginInterface, IPluginLog logger)
        {
            this.PluginInterface = pluginInterface;
            this.Logger = logger;

            this._config = pluginInterface.GetPluginConfig() as Config ?? new();
            this._config.UpdateConfiguration();

            this._windows = new WindowSystem("ihatecapslock");
            this._configWindow = new ConfigWindow(pluginInterface, this._config);
            this._windows.AddWindow(this._configWindow);

            this.PluginInterface.UiBuilder.Draw += this._windows.Draw;
            this.PluginInterface.UiBuilder.Draw += this.CheckKeysAndReleaseCaps;
            this.PluginInterface.UiBuilder.OpenConfigUi += this._configWindow.Toggle;
        }

        private void CheckKeysAndReleaseCaps()
        {
            if (!this._config.DedicatedThread) this.CheckKeysAndReleaseCapsCore();
            else if (this._thread is null) this.StartDedicatedThread();
        }

        private void CheckKeysAndReleaseCapsCore()
        {
            var ticks = Environment.TickCount64;

            // If an exception happened recently, skip (avoid log spam)
            if (ticks - this._lastExceptionTicks < ExceptionDelay) return;

            // Make sure that the game's window is active or always capture is enabled
            if (!this.ShouldCapture()) return;

            // If Return key is pressed down...
            if (this._config.CompletelyDisable || this.IsAnyConfiguredKeyPressed())
            {
                try
                {
                    // Check for caps lock state.
                    var caps = Native.GetKeyState((int)VirtualKey.CAPITAL) != 0;

                    // If caps is on, release caps lock.
                    if (caps) this.ReleaseCapsLock();
                }
                catch (Exception ex)
                {
                    this._lastExceptionTicks = ticks;
                    this.Logger.Error(ex, string.Empty);
                }
            }
        }

        public bool ShouldCapture() => this._config.AlwaysPoll || Helper.GetActiveProcessId() == Environment.ProcessId;

        public bool IsAnyConfiguredKeyPressed()
        {
            var keys = CollectionsMarshal.AsSpan(this._config.Keys);
            foreach (var key in keys)
            {
                if ((Native.GetKeyState((int)key) & 0xfffe) != 0) return true;
            }
            return false;
        }

        private void ReleaseCapsLock()
        {
            this.Logger.Debug("Removing CAPS LOCK!");
            Native.keybd_event((byte)VirtualKey.CAPITAL, 0x45, Constants.KEYEVENTF_EXTENDEDKEY, 0);
            Native.keybd_event((byte)VirtualKey.CAPITAL, 0x45, Constants.KEYEVENTF_EXTENDEDKEY | Constants.KEYEVENTF_KEYUP, 0);
        }

        private void StartDedicatedThread()
        {
            var thread = Volatile.Read(ref this._thread);
            if (thread is not null) return;

            var newThread = new Thread(this.DedicatedThread);
            if (Interlocked.CompareExchange(ref this._thread, newThread, thread) == thread) newThread.Start();
        }

        private void DedicatedThread()
        {
            try
            {
                while (!this._disposed && this._config.DedicatedThread)
                {
                    this.CheckKeysAndReleaseCapsCore();
                    Thread.Sleep(1);
                }
            }
            finally
            {
                Volatile.Write(ref this._thread, null);
            }
        }

        public void Dispose()
        {
            this._disposed = true;
            this.PluginInterface.UiBuilder.Draw -= this._windows.Draw;
            this.PluginInterface.UiBuilder.Draw -= this.CheckKeysAndReleaseCaps;
            this.PluginInterface.UiBuilder.OpenConfigUi -= this._configWindow.Toggle;
        }
    }
}
