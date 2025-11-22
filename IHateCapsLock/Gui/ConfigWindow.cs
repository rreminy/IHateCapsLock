using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;

namespace IHateCapsLock.Gui
{
    public sealed class ConfigWindow : Window
    {
        private IDalamudPluginInterface PluginInterface { get; }
        private Config Config { get; }

        public ConfigWindow(IDalamudPluginInterface pluginInterface, Config config) : base("I Hate Caps Lock###ihatecapslock")
        {
            PluginInterface = pluginInterface;
            Config = config;

            this.Size = new(320, 340);
            this.SizeCondition = ImGuiCond.FirstUseEver;
        }

        public override void Draw()
        {
            ImGui.Checkbox("Dedicated Thread###thread", ref Config.DedicatedThread);
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("Use a dedicated thread to poll for the configured keys.\nThe dedicated thread polls every millisecond.\nWithout this, keys are polled every frame instead.");

            using (var node = ImRaii.TreeNode($"Keys to poll", ImGuiTreeNodeFlags.DefaultOpen))
            {
                if (node.Success)
                {
                    VirtualKeyWidgets.Selector("keys", "Keys", this.Config.Keys);
                }
            }

            using (var node = ImRaii.TreeNode($"Advanced"))
            {
                if (node.Success)
                {
                    ImGui.Checkbox("Always Poll###poll", ref Config.AlwaysPoll);
                    if (ImGui.IsItemHovered()) ImGui.SetTooltip("Allow polling for the configured keys without the game's window being active.\nWARNING: Only select this if you really want this behavior!");
                }
            }
        }

        public override void OnClose()
        {
            this.PluginInterface.SavePluginConfig(this.Config);
            base.OnClose();
        }
    }
}
