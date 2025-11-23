using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace IHateCapsLock.Gui
{
    public static class VirtualKeyWidgets
    {
        private static readonly VirtualKey[] s_keys = [VirtualKey.NO_KEY, VirtualKey.BACK, VirtualKey.RETURN, VirtualKey.SHIFT, VirtualKey.SPACE, VirtualKey.DELETE, VirtualKey.OEM_2];
        private static readonly Dictionary<string, VirtualKey> s_popupSelection = [];
        private static readonly Dictionary<string, int> s_index = [];

        public static bool Selector(string id, string title, IList<VirtualKey> keys)
        {
            var result = false;
            ref var selectedIndex = ref CollectionsMarshal.GetValueRefOrAddDefault(s_index, id, out _);

            ImGui.SetNextWindowSize(new(320, 160), ImGuiCond.FirstUseEver);
            using (var popup = ImRaii.PopupModal($"Add Key###{id}_addPopup"))
            {
                if (popup.Success)
                {
                    ref var selectedKey = ref CollectionsMarshal.GetValueRefOrAddDefault(s_popupSelection, id, out _);
                    Combo($"Key###{id}_addPopupCombo", ref selectedKey);

                    if (ImGui.Button("Add"))
                    {
                        if (selectedKey is not VirtualKey.NO_KEY)
                        {
                            var index = keys.IndexOf(selectedKey);
                            if (index is -1)
                            {
                                selectedIndex = keys.Count;
                                keys.Add(selectedKey);
                                result = true;
                            }
                            else
                            {
                                selectedIndex = index;
                            }
                        }
                        ImGui.CloseCurrentPopup();
                    }

                    if (ImGui.Button("Cancel"))
                    {
                        ImGui.CloseCurrentPopup();
                    }
                }
            }

            using (var list = ImRaii.ListBox($"{title}###{id}_list"))
            {
                if (list.Success)
                {
                    for (var index = 0; index < keys.Count; index++)
                    {
                        if (ImGui.Selectable($"{GetVirtualKeyText(keys[index])}###keys_{index}", selectedIndex == index))
                        {
                            selectedIndex = index;
                            result = true;
                        }
                    }
                }
            }

            if (ImGui.Button("Add###{id_add}")) ImGui.OpenPopup($"###{id}_addPopup");
            if (ImGui.Button("Remove Selected###{id_remove}") && selectedIndex < keys.Count)
            {
                keys.RemoveAt(selectedIndex);
                result = true;
            }
            return result;
        }

        public static bool Combo(string title, ref VirtualKey selectedKey)
        {
            var result = false;
            using (var combo = ImRaii.Combo(title, selectedKey.GetFancyName(), ImGuiComboFlags.HeightLarge))
            {
                if (combo.Success)
                {
                    foreach (var key in s_keys)
                    {
                        if (ImGui.Selectable($"{GetVirtualKeyText(key)}###key_{(int)key}", selectedKey == key))
                        {
                            selectedKey = key;
                            result = true;
                        }
                    }

                    if (Array.IndexOf(s_keys, selectedKey) is -1)
                    {
                        ImGui.Selectable($"{GetVirtualKeyText(selectedKey)}###key_{(int)selectedKey}", true);
                    }
                }
            }
            return result;
        }

        private static string GetVirtualKeyText(VirtualKey key)
        {
            try
            {
                return key.GetFancyName();
            }
            catch
            {
                return $"Unknown Key {key}";
            }
        }
    }
}
