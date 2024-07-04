using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.Interfaces;
using System;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Hooking.HookTypes;

internal unsafe class SimpleTextHook : ITextHook
{
    protected string EarlyLastAnswer = "";
    protected string LastAnswer = "";

    public bool Faulty { get; protected set; } = true;

    protected DalamudServices Services = null!;
    protected IPettableUserList PettableUserList { get; set; } = null!;
    protected IPetServices PetServices { get; set; } = null!;

    protected uint[] TextPos { get; set; } = new uint[0];
    Func<int, bool> AllowedToFunction = _ => false;

    protected bool IsSoft;

    IPettableUser? lastPettableUser = null;

    public void Setup(DalamudServices services, IPettableUserList userList, IPetServices petServices, string AddonName, uint[] textPos, Func<int, bool> allowedCallback, bool isSoft = false)
    {
        Services = services;
        PettableUserList = userList;
        PetServices = petServices;
        TextPos = textPos;
        AllowedToFunction = allowedCallback;
        IsSoft = isSoft;
        services.AddonLifecycle.RegisterListener(AddonEvent.PreUpdate, AddonName, HandleUpdate);
    }

    public void SetUnfaulty() => Faulty = false;

    void HandleUpdate(AddonEvent addonEvent, AddonArgs addonArgs) => HandleRework((AtkUnitBase*)addonArgs.Addon);
    
    void HandleRework(AtkUnitBase* baseElement)
    {
        if (Faulty) return;

        if (TextPos.Length == 0) return;
        if (!baseElement->IsVisible) return;
       
        BaseNode bNode = new BaseNode(baseElement);
        AtkTextNode* tNode = GetTextNode(ref bNode);
        if (tNode == null) return;

        // Make sure it only runs once
        string tNodeText = tNode->NodeText.ToString();
        if ((tNodeText == string.Empty || tNodeText == LastAnswer) && (lastPettableUser != null && !lastPettableUser.DataBaseEntry.Dirty)) return;

        if (!OnTextNode(tNode, tNodeText)) LastAnswer = tNodeText;
    }

    protected virtual bool OnTextNode(AtkTextNode* textNode, string text)
    {
        IPettableUser? user = lastPettableUser = GetUser();
        if (user == null) return false;

        PetSheetData? pet = GetPetData(text, ref user);
        if (pet == null) return false;
        PetSheetData pPet = pet.Value;

        string? customName = user.DataBaseEntry.GetName(pPet.Model);
        if (customName == null) return false;

        SetText(textNode, text, customName, pPet);
        return true;
    }

    protected void SetText(AtkTextNode* textNode, string text, string customName, PetSheetData pPet)
    {
        if (AllowedToFunction != null)
        {
            if (!AllowedToFunction.Invoke(pPet.Model))
            {
                LastAnswer = text;
                return;
            }
        }
        LastAnswer = PetServices.StringHelper.ReplaceATKString(textNode, text, customName, pPet);
    }

    protected virtual PetSheetData? GetPetData(string text, ref IPettableUser user) => GetPetFromString(text, ref user, IsSoft);

    protected virtual IPettableUser? GetUser() => PettableUserList.LocalPlayer;

    protected virtual PetSheetData? GetPetFromString(string baseString, ref IPettableUser user, bool soft)
    {
        List<PetSheetData> data = PetServices.PetSheets.GetListFromLine(baseString);

        if (data.Count == 0) return null;

        data.Sort((i1, i2) => i1.BaseSingular.CompareTo(i2.BaseSingular));
        data.Reverse();

        PetSheetData normalPetData = data[0];

        if (!soft) return normalPetData;

        int? softIndex = PetServices.PetSheets.NameToSoftSkeletonIndex(normalPetData.BasePlural);
        if (softIndex == null) return normalPetData;

        int? softSkeleton = user.DataBaseEntry.GetSoftSkeleton(softIndex.Value);
        if (softSkeleton == null) return normalPetData;

        PetSheetData? softPetData = PetServices.PetSheets.GetPet(softSkeleton.Value);
        if (softPetData == null) return normalPetData;

        return new PetSheetData(softPetData.Value.Model, softPetData.Value.Icon, softPetData.Value.Pronoun, normalPetData.BaseSingular, normalPetData.BasePlural, ref Services);
    }

    protected AtkTextNode* GetTextNode(ref BaseNode bNode)
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
}
