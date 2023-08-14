using Dalamud.Game;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using System.Runtime.InteropServices;
using System;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Core.Serialization;

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

    byte Update(AtkUnitBase* baseD)
    {
        if (!PluginLink.Configuration.displayCustomNames || !PluginLink.Configuration.allowTooltips) return addonupdatehook!.Original(baseD);
        string? name = Marshal.PtrToStringUTF8((IntPtr)baseD->Name);
        if (!baseD->IsVisible || name is not "ActionDetail")
            return addonupdatehook!.Original(baseD);
        BaseNode bNode = new BaseNode(name);
        if(bNode == null) return addonupdatehook!.Original(baseD);
        AtkTextNode* tNode = bNode.GetNode<AtkTextNode>(5);
        if(tNode == null) return addonupdatehook!.Original(baseD);
        int id = SheetUtils.instance.GetIDFromName(tNode->NodeText.ToString());
        if(id == -1) return addonupdatehook!.Original(baseD);
        SerializableNickname nName = NicknameUtils.instance.GetLocalNicknameV2(id);
        if (nName == null) return addonupdatehook!.Original(baseD);
        if (!nName.Valid()) return addonupdatehook!.Original(baseD);
        tNode->NodeText.SetString(nName.Name);
        return addonupdatehook!.Original(baseD);
    }

    byte Update2(AtkUnitBase* baseD)
    {
        if (!PluginLink.Configuration.displayCustomNames || !PluginLink.Configuration.allowTooltips) return addonupdatehook2!.Original(baseD);
        string? name = Marshal.PtrToStringUTF8((IntPtr)baseD->Name);
        if (!baseD->IsVisible || name is not "Tooltip")
            return addonupdatehook2!.Original(baseD);
        BaseNode bNode = new BaseNode(name);
        if (bNode == null) return addonupdatehook2!.Original(baseD);
        AtkTextNode* tNode = bNode.GetNode<AtkTextNode>(2);
        if (tNode == null) return addonupdatehook2!.Original(baseD);
        int id = SheetUtils.instance.GetIDFromName(tNode->NodeText.ToString());
        if (id == -1) return addonupdatehook2!.Original(baseD);
        SerializableNickname nName = NicknameUtils.instance.GetLocalNicknameV2(id);
        if (nName == null) return addonupdatehook2!.Original(baseD);
        if (!nName.Valid()) return addonupdatehook2!.Original(baseD);
        tNode->NodeText.SetString(nName.Name);
        return addonupdatehook2!.Original(baseD);
    }

    internal override void OnDispose()
    {
        addonupdatehook?.Disable();
        addonupdatehook?.Dispose();

        addonupdatehook2?.Disable();
        addonupdatehook2?.Dispose();
    }
}
