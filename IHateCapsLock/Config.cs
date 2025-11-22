using Dalamud.Configuration;
using Dalamud.Game.ClientState.Keys;
using System.Collections.Generic;

namespace IHateCapsLock
{
    public sealed class Config : IPluginConfiguration
    {
        private List<VirtualKey> _keys = [];
        
        public int Version { get; set; } = 0;

        public bool AlwaysPoll;

        public bool DedicatedThread;

        public bool DefaultKeysAdded;

        public List<VirtualKey> Keys
        {
            get => this._keys;
            set => this._keys = value ?? [];
        }

        public void UpdateConfiguration()
        {
            if (!DefaultKeysAdded)
            {
                this.DefaultKeysAdded = true;
                this.Keys.Add(VirtualKey.RETURN);
            }
        }
    }
}
