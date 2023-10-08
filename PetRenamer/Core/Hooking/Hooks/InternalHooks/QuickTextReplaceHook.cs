using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.Core.Hooking.Hooks.InternalHooks;

public unsafe class QuickTextReplaceHook
{
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
        AtkPos = atkPos;
        this.recallAction = recallAction;
        this.allowedToFunction = allowedToFunction;
        this.latestOutcome = latestOutcome;
        PluginHandlers.AddonLifecycle.RegisterListener(AddonEvent.PreDraw, addonName, HandleUpdate);
    }

    void HandleUpdate(AddonEvent addonEvent, AddonArgs addonArgs) => HandleRework((AtkUnitBase*)addonArgs.Addon);

    void HandleRework(AtkUnitBase* baseElement)
    {
        if (!allow || TextPos.Length == 0) return;
        if (!baseElement->IsVisible) return;
        if (TooltipHelper.handleAsItem) return;

        BaseNode bNode = new BaseNode(baseElement);
        AtkTextNode* tNode = GetTextNode(ref bNode);
        if (tNode == null) return;

        string tNodeText = tNode->NodeText.ToString() ?? string.Empty;
        if (tNodeText == string.Empty || tNodeText == lastAnswer) return;
        latestOutcome?.Invoke(tNodeText);

        AtkNineGridNode* nineGridNode = GetBackgroundNode(ref bNode);

        PettableUser user = GetUser();
        if (user == null) return;

        (int, string) currentName = GetNameRework(tNodeText, ref user);

        int id = currentName.Item1;
        string replaceName = currentName.Item2;
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
        if (recallAction != null) return recallAction.Invoke();
        else return PluginLink.PettableUserHandler.LocalUser()!;
    }
    AtkNineGridNode* GetBackgroundNode(ref BaseNode bNode) => AtkPos != -1 ? bNode.GetNode<AtkNineGridNode>((uint)AtkPos) : null!;

    (int, string) GetNameRework(string tNodeText, ref PettableUser user)
    {
        int id = SheetUtils.instance.GetIDFromName(tNodeText);
        if (id > -1) return (id, tNodeText);

        id = RemapUtils.instance.GetPetIDFromClass(user.JobClass);

        List<KeyValuePair<int, string>> listy = RemapUtils.instance.bakedBattlePetSkeletonToName
          .Where(v => tNodeText.Contains(v.Value))
          .OrderBy(v => v.Value.Length)
          .ToList();
        
        if (listy.Count == 0) return (id, string.Empty);
        return (id, listy.Last().Value);
    }
}
