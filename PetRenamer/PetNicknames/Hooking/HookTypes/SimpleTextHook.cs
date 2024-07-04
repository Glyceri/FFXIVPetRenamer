using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.Interfaces;
using System;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

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

        string tNodeText = tNode->NodeText.ToString();
        if (tNodeText == string.Empty || tNodeText == LastAnswer) return;

        PetServices.PetLog.Log("GO!");

        if(!OnTextNode(tNode, tNodeText)) LastAnswer = tNodeText;
    }

    protected virtual bool OnTextNode(AtkTextNode* textNode, string text)
    {
        IPettableUser? user = GetUser();
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
        string cleanedString = CleanupString(baseString);

        PetSheetData? normalPetData = PetServices.PetSheets.GetPetFromName(cleanedString);
        if (normalPetData == null) return normalPetData;
        if (!soft) return normalPetData;

        int? softIndex = PetServices.PetSheets.NameToSoftSkeletonIndex(cleanedString);
        if (softIndex == null) return normalPetData;

        int? softSkeleton = user.DataBaseEntry.GetSoftSkeleton(softIndex.Value);
        if (softSkeleton == null) return normalPetData;

        PetSheetData? softPetData = PetServices.PetSheets.GetPet(softSkeleton.Value);
        if (softPetData == null) return normalPetData;

        return new PetSheetData(softPetData.Value.Model, softPetData.Value.Icon, softPetData.Value.Pronoun, normalPetData.Value.BaseSingular, normalPetData.Value.BasePlural, ref Services);
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

    string CleanupString(string str)
    {
        return str.Replace("サモン・", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                  .Replace("Summon ", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                  .Replace("Invocation ", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                  .Replace("-Beschwörung", string.Empty, StringComparison.InvariantCultureIgnoreCase);
    }
}
