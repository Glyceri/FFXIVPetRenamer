using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Logging;
using PetRenamer.Utilization.UtilsModule;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace PetRenamer.Core.Hooking.Hooks.InternalHooks;

public unsafe class QuickTextReplaceHook : IDisposable
{
    Hook<Delegates.AddonUpdate>? addonupdatehook = null;
    AtkUnitBase* baseElement;

    readonly string AddonName;
    readonly uint[] TextPos;
    readonly int AtkPos;
    readonly Func<PettableUser> recallAction;
    readonly Func<int, bool> allowedToFunction;
    readonly Action<string> latestOutcome;

    public QuickTextReplaceHook(string addonName, uint textPos, Func<int, bool> allowedToFunction, int atkPos = -1, Func<PettableUser> recallAction = null!, Action<string> latestOutcome = null!)
    {
        AddonName = addonName;
        TextPos = new uint[1] { textPos };
        AtkPos = atkPos;
        this.recallAction = recallAction;
        this.allowedToFunction = allowedToFunction;
        this.latestOutcome = latestOutcome;
    }

    public QuickTextReplaceHook(string addonName, uint[] textPos, Func<int, bool> allowedToFunction, int atkPos = -1, Func<PettableUser> recallAction = null!, Action<string> latestOutcome = null!)
    {
        AddonName = addonName;
        TextPos = textPos;
        AtkPos = atkPos;
        this.recallAction = recallAction;
        this.allowedToFunction = allowedToFunction;
        this.latestOutcome = latestOutcome;
    }

    public void Dispose()
    {
        baseElement = null;
        addonupdatehook?.Dispose();
        GC.SuppressFinalize(this);
    }

    bool allow = true;

    public void OnUpdate(IFramework framework, bool allow)
    {
        this.allow = allow;
        baseElement = (AtkUnitBase*)PluginHandlers.GameGui.GetAddonByName(AddonName);
        if (baseElement == null) return;
        addonupdatehook ??= PluginHandlers.Hooking.HookFromAddress<Delegates.AddonUpdate>(new nint(baseElement->AtkEventListener.vfunc[PluginConstants.AtkUnitBaseUpdateIndex]), Handle);
        addonupdatehook?.Enable();
    }

    string lastAnswer = string.Empty;

    byte Handle(AtkUnitBase* baseElement)
    {
        if (TextPos.Length == 0) return addonupdatehook!.Original(baseElement);
        if (!allow) return addonupdatehook!.Original(baseElement);

        string? name = Marshal.PtrToStringUTF8((IntPtr)baseElement->Name);
        if (!baseElement->IsVisible || name != AddonName)
            return addonupdatehook!.Original(baseElement);

        BaseNode bNode = new BaseNode(name);
        if (bNode == null) return addonupdatehook!.Original(baseElement);
        AtkTextNode* tNode = null!;
        if (TextPos.Length > 1)
        {
            ComponentNode cNode = bNode.GetComponentNode(TextPos[0]);
            for (int i = 1; i < TextPos.Length - 1; i++)
            {
                if (cNode == null) return addonupdatehook!.Original(baseElement);
                cNode = cNode.GetComponentNode(TextPos[i]);
            }
            if (cNode == null) return addonupdatehook!.Original(baseElement);
            tNode = cNode.GetNode<AtkTextNode>(TextPos[^1]);
        }
        else
        {
            tNode = bNode.GetNode<AtkTextNode>(TextPos[0]);
        }

        if (tNode == null) return addonupdatehook!.Original(baseElement);
        if (TooltipHelper.handleAsItem) return addonupdatehook!.Original(baseElement);
        string tNodeText = tNode->NodeText.ToString();
        if (tNodeText == null) return addonupdatehook!.Original(baseElement);
        if (tNodeText != lastAnswer) latestOutcome?.Invoke(tNodeText);
        if (tNodeText == lastAnswer && AddonName != "Tooltip") return addonupdatehook!.Original(baseElement);


        AtkNineGridNode* nineGridNode = null;
        if (AtkPos != -1) nineGridNode = bNode.GetNode<AtkNineGridNode>((uint)AtkPos);
        if (AtkPos != -1 && nineGridNode == null) return addonupdatehook!.Original(baseElement);

        PettableUser user = PluginLink.PettableUserHandler.LocalUser()!;

        if (recallAction != null)
        {
            PettableUser tempUser = recallAction.Invoke();
            if (tempUser != null) user = tempUser;
        }

        if (user == null) return addonupdatehook!.Original(baseElement);

        int id = SheetUtils.instance.GetIDFromName(tNodeText);
        string replaceName = tNodeText;

        if (id == -1)
        {
            List<(string, int)> correctNames = new List<(string, int)>();
            foreach (int nameID in RemapUtils.instance.battlePetRemap.Keys)
            {
                if (!RemapUtils.instance.bakedBattlePetSkeletonToName.ContainsKey(nameID)) continue;
                string bPetName = RemapUtils.instance.bakedBattlePetSkeletonToName[nameID];
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
                int item2 = correctNames[shortestEl].Item2;
                if (RemapUtils.instance.skeletonToClass.ContainsKey(item2))
                    id = RemapUtils.instance.skeletonToClass[item2];
                replaceName = correctNames[shortestEl].Item1;
            }
        }

        if (replaceName == string.Empty) return addonupdatehook!.Original(baseElement);

        if (id == -1)
        {
            lastAnswer = tNodeText;
            return addonupdatehook!.Original(baseElement);
        }
        if (!allowedToFunction?.Invoke(id) ?? false) return addonupdatehook!.Original(baseElement);

        for (int i = 0; i < user.SerializableUser.length; i++)
        {
            int curID = user.SerializableUser.ids[i];
            string curNickname = user.SerializableUser.names[i];
            if (curID != id) continue;
            if (curNickname == string.Empty) continue;

            StringUtils.instance.ReplaceAtkString(tNode, replaceName, curNickname);

            if (nineGridNode != null)
            {
                tNode->ResizeNodeForCurrentText();
                nineGridNode->AtkResNode.SetWidth((ushort)(tNode->AtkResNode.Width + 18));
            }

            lastAnswer = curNickname;
            break;
        }

        return addonupdatehook!.Original(baseElement);
    }
}
