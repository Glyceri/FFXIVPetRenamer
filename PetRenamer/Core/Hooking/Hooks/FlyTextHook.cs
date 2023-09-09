using Dalamud.Game.Gui.FlyText;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.PettableUserSystem;
using System;
using PetRenamer.Utilization.UtilsModule;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal class FlyTextHook : HookableElement
{
    [Signature("E8 ?? ?? ?? ?? 8B 8C 24 ?? ?? ?? ?? 85 C9", DetourName = nameof(AddToScreenLogWithLogMessageIdDetour))]
    Hook<Delegates.AddToScreenLogWithLogMessageId>? addToScreenLogWithLogMessageId = null;

    unsafe internal override void OnInit()
    {
        PluginHandlers.FlyTextGui.FlyTextCreated += OnFlyTextCreated;

        addToScreenLogWithLogMessageId?.Enable();
    }

    void OnFlyTextCreated(ref FlyTextKind kind, ref int val1, ref int val2, ref SeString text1, ref SeString text2, ref uint color, ref uint icon, ref uint damageTypeIcon, ref float yOffset, ref bool handled)
    {
        if (!PluginLink.Configuration.useCustomFlyoutInChat) return;
        PettableUser user = PluginLink.PettableUserHandler.LastCastedUser()!;
        (string, string)[] validNames = PluginLink.PettableUserHandler.GetValidNames(user, text1.ToString() + text2.ToString());
        StringUtils.instance.ReplaceSeString(ref text1, validNames);
        StringUtils.instance.ReplaceSeString(ref text2, validNames);
    }

    unsafe void AddToScreenLogWithLogMessageIdDetour(IntPtr castBoss, IntPtr castDealer, int a3, char a4, int a5, int a6, int a7, int a8)
    {
        addToScreenLogWithLogMessageId?.Original(castBoss, castDealer, a3, a4, a5, a6, a7, a8);
        PluginLink.PettableUserHandler.SetLastCast(castBoss, castDealer);
    }

    internal override void OnDispose()
    {
        PluginHandlers.FlyTextGui.FlyTextCreated -= OnFlyTextCreated;

        addToScreenLogWithLogMessageId?.Dispose();
    }
}