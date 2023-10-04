using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PetRenamer.Core.Hooking.Hooks.InternalHooks;

public unsafe class QuickTextReplaceHook
{
    readonly string AddonName;
    readonly uint[] TextPos;
    readonly int AtkPos;
    readonly Func<PettableUser> recallAction;
    readonly Func<int, bool> allowedToFunction;
    readonly Action<string> latestOutcome;

    bool allow = true;
    string lastAnswer = string.Empty;

    public QuickTextReplaceHook(string addonName, uint textPos, Func<int, bool> allowedToFunction, int atkPos = -1, Func<PettableUser> recallAction = null!, Action<string> latestOutcome = null!) : this(addonName, new uint[1] { textPos }, allowedToFunction, atkPos, recallAction, latestOutcome) { }
    public QuickTextReplaceHook(string addonName, uint[] textPos, Func<int, bool> allowedToFunction, int atkPos = -1, Func<PettableUser> recallAction = null!, Action<string> latestOutcome = null!)
    {
        TextPos = textPos;
        AddonName = addonName;
        AtkPos = atkPos;
        this.recallAction = recallAction;
        this.allowedToFunction = allowedToFunction;
        this.latestOutcome = latestOutcome;
        PluginHandlers.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, addonName, HandleUpdate);
    }

    void HandleUpdate(AddonEvent addonEvent, AddonArgs addonArgs) => Handle((AtkUnitBase*)addonArgs.Addon);

    void Handle(AtkUnitBase* baseElement)
    {
        if (!allow || TextPos.Length == 0) return;

        string? name = Marshal.PtrToStringUTF8((IntPtr)baseElement->Name);
        if (!baseElement->IsVisible || name != AddonName) return;

        if (TooltipHelper.handleAsItem) return;

        BaseNode bNode = new BaseNode(name);
        if (bNode == null) return;
        AtkTextNode* tNode = GetTextNode(ref bNode);
        if (tNode == null) return;

        string tNodeText = tNode->NodeText.ToString() ?? string.Empty;
        if (tNodeText == string.Empty) return;
        if (tNodeText != lastAnswer) latestOutcome?.Invoke(tNodeText);
        if (tNodeText == lastAnswer && AddonName != "Tooltip") return;

        AtkNineGridNode* nineGridNode = GetBackgroundNode(ref bNode);
        if (AtkPos != -1 && nineGridNode == null) return;

        PettableUser user = GetUser();
        if (user == null) return;

        (int, string) data = GetName(SheetUtils.instance.GetIDFromName(tNodeText), tNodeText);
        int id = data.Item1;
        string replaceName = data.Item2;
        if (replaceName == string.Empty) return;

        if (id == -1)
        {
            lastAnswer = tNodeText;
            return;
        }

        if (!allowedToFunction?.Invoke(id) ?? false) return;

        string curNickname = user.SerializableUser.GetNameFor(id);
        if (curNickname == string.Empty) return;
        StringUtils.instance.ReplaceAtkString(tNode, replaceName, curNickname, nineGridNode);
        lastAnswer = curNickname;

        return;
    }

    AtkTextNode* GetTextNode(ref BaseNode bNode)
    {
        if (TextPos.Length > 1)
        {
            ComponentNode cNode = bNode.GetComponentNode(TextPos[0]);
            for (int i = 1; i < TextPos.Length - 1; i++)
            {
                if (cNode == null) return null!;
                cNode = cNode.GetComponentNode(TextPos[i]);
            }
            if (cNode == null) return null!;
            return cNode.GetNode<AtkTextNode>(TextPos[^1]);
        }
        return bNode.GetNode<AtkTextNode>(TextPos[0]);
    }

    PettableUser GetUser() 
    { 
        if (recallAction != null) return recallAction.Invoke()!;
        return PluginLink.PettableUserHandler.LocalUser()!;
    }
    AtkNineGridNode* GetBackgroundNode(ref BaseNode bNode) => AtkPos != -1 ? bNode.GetNode<AtkNineGridNode>((uint)AtkPos) : null!;

    (int, string) GetName(int id, string replaceName)
    {
        string textNodeText = replaceName;
        if (id != -1) return (id, replaceName);

        List<(string, int)> correctNames = new List<(string, int)>();
        foreach (int nameID in RemapUtils.instance.battlePetRemap.Keys)
        {
            if (!RemapUtils.instance.bakedBattlePetSkeletonToName.ContainsKey(nameID)) continue;
            string bPetName = RemapUtils.instance.bakedBattlePetSkeletonToName[nameID];
            if (textNodeText.Contains(bPetName))
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

        return (id, replaceName);
    }
}
