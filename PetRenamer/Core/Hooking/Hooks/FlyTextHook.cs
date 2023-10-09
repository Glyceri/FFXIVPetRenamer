using Dalamud.Game.Gui.FlyText;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using System;
using System.Linq;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal class FlyTextHook : HookableElement
{
    [Signature("E8 ?? ?? ?? ?? 8B 8C 24 ?? ?? ?? ?? 85 C9", DetourName = nameof(AddToScreenLogWithLogMessageIdDetour))]
    readonly Hook<Delegates.AddToScreenLogWithLogMessageId>? addToScreenLogWithLogMessageId = null;

    unsafe internal override void OnInit()
    {
        PluginHandlers.FlyTextGui.FlyTextCreated += OnFlyTextCreated;
        addToScreenLogWithLogMessageId?.Enable();
    }

    // TODO: FIX
    void OnFlyTextCreated(ref FlyTextKind kind, ref int val1, ref int val2, ref SeString text1, ref SeString text2, ref uint color, ref uint icon, ref uint damageTypeIcon, ref float yOffset, ref bool handled)
    {
        if (!PluginLink.Configuration.displayCustomNames) return;
        if (!PluginLink.Configuration.useCustomFlyoutPet) return;
        PettableUser user = PluginLink.PettableUserHandler.LastCastedUser()!;
        (string, string)[] validNames = PluginLink.PettableUserHandler.GetValidNames(user, text1.ToString() + text2.ToString(), false);
        StringUtils.instance.ReplaceSeString(ref text1, ref validNames);
        StringUtils.instance.ReplaceSeString(ref text2, ref validNames);
    }

    unsafe void AddToScreenLogWithLogMessageIdDetour(IntPtr target, IntPtr castDealer, int a3, char a4, int castID, int a6, int a7, int a8)
    {
        addToScreenLogWithLogMessageId?.Original(target, castDealer, a3, a4, castID, a6, a7, a8);
        PluginLink.PettableUserHandler.SetLastCast(target, castDealer, castID);
        if(RemapUtils.instance.petIDToAction.Values.Contains((uint)castID)) PluginLink.PettableUserHandler.SetLastCastSoft(target, castDealer, castID);
    }

    internal override void OnDispose()
    {
        PluginHandlers.FlyTextGui.FlyTextCreated -= OnFlyTextCreated;
        addToScreenLogWithLogMessageId?.Dispose();
    }
}