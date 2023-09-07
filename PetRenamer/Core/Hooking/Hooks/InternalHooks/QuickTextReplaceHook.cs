using Dalamud.Game;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PetRenamer.Core.Hooking.Hooks.InternalHooks;

public unsafe class QuickTextReplaceHook : IDisposable
{
    Hook<Delegates.AddonUpdate>? addonupdatehook = null;
    AtkUnitBase* baseElement;

    string AddonName;
    uint TextPos;
    int AtkPos;

    public QuickTextReplaceHook(string addonName, uint textPos, int atkPos)
    {
        AddonName = addonName;
        TextPos = textPos;
        AtkPos = atkPos;
    }

    public void Dispose()
    {
        baseElement = null;
        addonupdatehook?.Dispose();
    }
    bool allow = true;

    public void OnUpdate(Framework framework, bool allow)
    {
        this.allow = allow;
        baseElement = (AtkUnitBase*)PluginHandlers.GameGui.GetAddonByName(AddonName);
        addonupdatehook ??= Hook<Delegates.AddonUpdate>.FromAddress(new nint(baseElement->AtkEventListener.vfunc[PluginConstants.AtkUnitBaseUpdateIndex]), Handle);
        addonupdatehook?.Enable();
    }

    string lastAnswer = string.Empty;
    byte Handle(AtkUnitBase* baseElement)
    {
        if(!allow) return addonupdatehook!.Original(baseElement);
        string? name = Marshal.PtrToStringUTF8((IntPtr)baseElement->Name);
        if (!baseElement->IsVisible || name != AddonName)
            return addonupdatehook!.Original(baseElement);

        BaseNode bNode = new BaseNode(name);
        if (bNode == null) return addonupdatehook!.Original(baseElement);
        AtkTextNode* tNode = bNode.GetNode<AtkTextNode>(TextPos);
        if (tNode == null) return addonupdatehook!.Original(baseElement);
        string tNodeText = tNode->NodeText.ToString();
        if (tNodeText == lastAnswer) return addonupdatehook!.Original(baseElement);

        AtkNineGridNode* nineGridNode = null;
        if (AtkPos != -1) nineGridNode = bNode.GetNode<AtkNineGridNode>((uint)AtkPos);
        if (AtkPos != -1 && nineGridNode == null) return addonupdatehook!.Original(baseElement);

        PettableUser user = PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return addonupdatehook!.Original(baseElement);

        int id = SheetUtils.instance.GetIDFromName(tNodeText);
        string replaceName = tNodeText;

        if (id == -1)
        {
            List<(string, int)> correctNames = new List<(string, int)>();
            foreach (int nameID in RemapUtils.instance.battlePetRemap.Keys)
            {
                string bPetName = SheetUtils.instance.GetBattlePetName(RemapUtils.instance.battlePetRemap[nameID]);
                if (tNodeText.Contains(bPetName))
                    correctNames.Add((bPetName, nameID));
            }
            if (correctNames.Count != 0)
            {
                int shortestEl = 0;
                for (int i = 1; i < correctNames.Count; i++)
                {
                    if (correctNames[i].Item1.Length < correctNames[shortestEl].Item1.Length)
                        shortestEl = i;
                }
                id = RemapUtils.instance.skeletonToClass[correctNames[shortestEl].Item2];
                replaceName = correctNames[shortestEl].Item1;
            }
        }

        if (replaceName == string.Empty) return addonupdatehook!.Original(baseElement);

        if (id == -1)
        {
            lastAnswer = tNodeText;
            return addonupdatehook!.Original(baseElement);
        }
        user.SerializableUser.LoopThroughBreakable(nickname =>
        {
            if (nickname.Item1 == id)
            {
                if (nickname.Item2 == string.Empty) return false;
                tNode->NodeText.SetString(tNode->NodeText.ToString().Replace(replaceName, nickname.Item2));

                if (nineGridNode != null)
                {
                    tNode->ResizeNodeForCurrentText();
                    nineGridNode->AtkResNode.SetWidth((ushort)(tNode->AtkResNode.Width + 18));
                }

                lastAnswer = nickname.Item2;
                return true;
            }
            return false;
        });

        return addonupdatehook!.Original(baseElement);
    }
}
