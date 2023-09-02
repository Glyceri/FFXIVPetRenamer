using Dalamud.Game;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using System.Runtime.InteropServices;
using System;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.PettableUserSystem;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.UI;
using ImGuiNET;
using Dalamud.Game.Text.SeStringHandling;

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

    byte Update(AtkUnitBase* baseD) => HandleFor(baseD, ref addonupdatehook!, "ActionDetail", 5, -1);
    byte Update2(AtkUnitBase* baseD) => HandleFor(baseD, ref addonupdatehook2!, "Tooltip", 2, 3);

    string lastAnswer = string.Empty;

    byte HandleFor(AtkUnitBase* baseD, ref Hook<Delegates.AddonUpdate> addonUpdateHook, string addonName, uint pos, int pos2)
    {
        if (!PluginLink.Configuration.displayCustomNames || !PluginLink.Configuration.allowTooltips) return addonUpdateHook!.Original(baseD);

        string? name = Marshal.PtrToStringUTF8((IntPtr)baseD->Name);
        if (!baseD->IsVisible || name != addonName)
            return addonUpdateHook!.Original(baseD);

        BaseNode bNode = new BaseNode(name);
        if (bNode == null) return addonUpdateHook!.Original(baseD);
        AtkTextNode* tNode = bNode.GetNode<AtkTextNode>(pos);
        if (tNode == null) return addonUpdateHook!.Original(baseD);
        AtkNineGridNode* nineGridNode = null;
        if (pos2 != -1)
        {
            nineGridNode = bNode.GetNode<AtkNineGridNode>((uint)pos2);
        }
        if(pos2 != -1 && nineGridNode == null) return addonUpdateHook!.Original(baseD);

        string tNodeText = tNode->NodeText.ToString();
        if (tNodeText == lastAnswer) return addonUpdateHook!.Original(baseD);

        PettableUser user = PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return addonUpdateHook!.Original(baseD);

        int id = SheetUtils.instance.GetIDFromName(tNodeText);
        string replaceName = tNodeText;

        if (id == -1)
        {
            List<(string, int)> correctNames = new List<(string, int)>();
            foreach(int nameID in RemapUtils.instance.battlePetRemap.Keys)
            {
                string bPetName = SheetUtils.instance.GetBattlePetName(RemapUtils.instance.battlePetRemap[nameID]);
                if (tNodeText.Contains(bPetName))
                    correctNames.Add((bPetName, nameID));
            }
            if(correctNames.Count != 0)
            {
                int shortestEl = 0;
                for(int i = 1; i < correctNames.Count; i++)
                {
                    if (correctNames[i].Item1.Length < correctNames[shortestEl].Item1.Length)
                        shortestEl = i;
                }
                id = RemapUtils.instance.skeletonToClass[correctNames[shortestEl].Item2];
                replaceName = correctNames[shortestEl].Item1;
            }
        }

        if (id == -1) 
        {
            lastAnswer = tNodeText;
            return addonUpdateHook!.Original(baseD); 
        }
        user.SerializableUser.LoopThroughBreakable(nickname =>
        {
            if(nickname.Item1 == id)
            {
                tNode->NodeText.SetString(tNode->NodeText.ToString().Replace(replaceName, nickname.Item2));
                if(nineGridNode != null)
                {
                    tNode->ResizeNodeForCurrentText();
                    nineGridNode->AtkResNode.SetWidth((ushort)(tNode->AtkResNode.Width + 18));
                }

                lastAnswer = nickname.Item2;
                return true;
            }
            return false;
        });

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
