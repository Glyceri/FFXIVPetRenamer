using Dalamud.Game.Gui.FlyText;
using Dalamud.Game.Text.SeStringHandling;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;

namespace PetRenamer.Core.Hooking.Hooks;

// Signatures from: https://github.com/Tischel/ActionTimeline/blob/master/ActionTimeline/Helpers/TimelineManager.cs
// Signatures from: https://github.com/cairthenn/Redirect/blob/main/Redirect/GameHooks.cs
// and from https://github.com/Kouzukii/ffxiv-deathrecap/blob/master/Events/CombatEventCapture.cs
// I store these so when they inevitably change, I can just yoink them again from there.

[Hook]
internal class FlyTextHook : HookableElement
{
    unsafe internal override void OnInit()
    {
        PluginHandlers.FlyTextGui.FlyTextCreated += OnFlyTextCreated;
    }

    void OnFlyTextCreated(ref FlyTextKind kind, ref int val1, ref int val2, ref SeString text1, ref SeString text2, ref uint color, ref uint icon, ref uint damageTypeIcon, ref float yOffset, ref bool handled)
    {

    }
  
    internal override void OnDispose()
    {
        PluginHandlers.FlyTextGui.FlyTextCreated -= OnFlyTextCreated;
    }
}