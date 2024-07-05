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
    protected Func<int, bool> AllowedToFunction = _ => false;

    protected bool IsSoft;

    IPettableUser? lastPettableUser = null;

    public virtual void Setup(DalamudServices services, IPettableUserList userList, IPetServices petServices, string AddonName, uint[] textPos, Func<int, bool> allowedCallback, bool isSoft = false)
    {
        Services = services;
        PettableUserList = userList;
        PetServices = petServices;
        TextPos = textPos;
        AllowedToFunction = allowedCallback;
        IsSoft = isSoft;
        services.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, AddonName, HandleUpdate);
    }

    public void SetUnfaulty() => Faulty = false;

    protected void HandleUpdate(AddonEvent addonEvent, AddonArgs addonArgs) => HandleRework((AtkUnitBase*)addonArgs.Addon);
    
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

    protected virtual void SetText(AtkTextNode* textNode, string text, string customName, PetSheetData pPet)
    {
        if (!CheckIfCanFunction(text, pPet)) return;
        LastAnswer = PetServices.StringHelper.ReplaceATKString(textNode, text, customName, pPet);
    }

    protected virtual bool CheckIfCanFunction(string text, PetSheetData pPet)
    {
        if (AllowedToFunction == null) return true;
        if (AllowedToFunction.Invoke(pPet.Model)) return true;
        LastAnswer = text;
        return false;
    }

    protected virtual PetSheetData? GetPetData(string text, ref IPettableUser user) => PetServices.PetSheets.GetPetFromString(text, ref user, IsSoft);

    protected virtual IPettableUser? GetUser() => PettableUserList.LocalPlayer;

    protected virtual AtkTextNode* GetTextNode(ref BaseNode bNode)
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
