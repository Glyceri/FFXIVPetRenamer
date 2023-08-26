using Dalamud.Game;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using System.Runtime.InteropServices;
using System;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Core.Serialization;
using Dalamud.Logging;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal unsafe class TooltipHook : HookableElement
{
    private Hook<Delegates.AddonUpdate>? addonupdatehook = null;
    private Hook<Delegates.AddonUpdate>? addonupdatehook2 = null;

    AtkUnitBase* baseElement;
    AtkUnitBase* baseElement2;

    internal override void OnUpdate(Framework framework)
    {
        if (PluginHandlers.ClientState.LocalPlayer! == null) return;
        baseElement = (AtkUnitBase*)PluginHandlers.GameGui.GetAddonByName("ActionDetail");
        baseElement2 = (AtkUnitBase*)PluginHandlers.GameGui.GetAddonByName("Tooltip");
        addonupdatehook ??= Hook<Delegates.AddonUpdate>.FromAddress(new nint(baseElement->AtkEventListener.vfunc[42]), Update);
        addonupdatehook?.Enable();

        addonupdatehook2 ??= Hook<Delegates.AddonUpdate>.FromAddress(new nint(baseElement2->AtkEventListener.vfunc[42]), Update2);
        addonupdatehook2?.Enable();
    }

    byte Update(AtkUnitBase* baseD) => HandleFor(baseD, ref addonupdatehook!, "ActionDetail", 5);
    byte Update2(AtkUnitBase* baseD) => HandleFor(baseD, ref addonupdatehook2!, "Tooltip", 2);

    string lastAnswer = string.Empty;

    byte HandleFor(AtkUnitBase* baseD, ref Hook<Delegates.AddonUpdate> addonUpdateHook, string addonName, uint pos)
    {
        if (!PluginLink.Configuration.displayCustomNames || !PluginLink.Configuration.allowTooltips) return addonUpdateHook!.Original(baseD);

        string? name = Marshal.PtrToStringUTF8((IntPtr)baseD->Name);
        if (!baseD->IsVisible || name != addonName)
            return addonUpdateHook!.Original(baseD);

        BaseNode bNode = new BaseNode(name);
        if (bNode == null) return addonUpdateHook!.Original(baseD);
        AtkTextNode* tNode = bNode.GetNode<AtkTextNode>(pos);
        if (tNode == null) return addonUpdateHook!.Original(baseD);

        string tNodeText = tNode->NodeText.ToString();
        if (tNodeText == lastAnswer) return addonUpdateHook!.Original(baseD);

        SerializableNickname[] nicknames = NicknameUtils.instance.GetLocalNicknamesV2();
        foreach (SerializableNickname nickname in nicknames)
        {
            if (nickname == null) continue;
            if (!nickname.Valid()) continue;
            nickname.Setup();
            if (!nickname.BaseNameEquals(tNodeText)) continue;
            tNode->NodeText.SetString(nickname.Name);
            lastAnswer = nickname.Name;
            break;
        }

        return addonUpdateHook!.Original(baseD);
    }

    internal override void OnDispose()
    {
        addonupdatehook?.Disable();
        addonupdatehook?.Dispose();

        addonupdatehook2?.Disable();
        addonupdatehook2?.Dispose();
    }
}
