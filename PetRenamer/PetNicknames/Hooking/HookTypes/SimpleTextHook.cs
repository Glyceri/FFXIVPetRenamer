using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.Interfaces;
using System;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookTypes;

internal unsafe class SimpleTextHook : ITextHook
{
    protected string EarlyLastAnswer = "";
    protected string LastAnswer = "";

    public bool Faulty { get; protected set; } = true;

    protected DalamudServices Services = null!;
    protected IPettableUserList PettableUserList { get; set; } = null!;
    protected IPetServices PetServices { get; set; } = null!;
    IPettableDirtyListener DirtyListener { get; set; } = null!;

    protected uint[] TextPos { get; set; } = Array.Empty<uint>();
    protected Func<int, bool> AllowedToFunction = _ => false;

    protected bool IsSoft;

    IPettableUser? lastPettableUser = null;

    public virtual void Setup(DalamudServices services, IPettableUserList userList, IPetServices petServices, IPettableDirtyListener dirtyListener, string AddonName, uint[] textPos, Func<int, bool> allowedCallback, bool isSoft = false)
    {
        Services = services;
        PettableUserList = userList;
        PetServices = petServices;
        DirtyListener = dirtyListener;
        TextPos = textPos;
        AllowedToFunction = allowedCallback;
        IsSoft = isSoft;

        DirtyListener.RegisterOnDirtyName(OnName);
        DirtyListener.RegisterOnDirtyEntry(OnEntry);
        DirtyListener.RegisterOnClearEntry(OnEntry);

        services.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, AddonName, HandleUpdate);
    }

    public void SetUnfaulty() => Faulty = false;
    public void SetFaulty() => Faulty = true;

    protected void HandleUpdate(AddonEvent addonEvent, AddonArgs addonArgs) => HandleRework((AtkUnitBase*)addonArgs.Addon);
    
    
    void OnName(INamesDatabase nameDatabase)
    {
        SetDirty();
    }

    void OnEntry(IPettableDatabaseEntry entry)
    {
        SetDirty();
    }

    bool isDirty = false;

    void SetDirty()
    {
        isDirty = true;
    }

    void ClearDirty()
    {
        isDirty = false;
    }

    void HandleRework(AtkUnitBase* baseElement)
    {
        if (BlockedCheck()) return;

        if (TextPos.Length == 0) return;
        if (!baseElement->IsVisible) return;
       
        BaseNode bNode = new BaseNode(baseElement);
        AtkTextNode* tNode = GetTextNode(in bNode);
        if (tNode == null) return;

        // Make sure it only runs once
        string tNodeText = tNode->NodeText.ToString();
        if ((tNodeText == string.Empty || tNodeText == LastAnswer) && !isDirty) return;

        ClearDirty();

        if (!OnTextNode(tNode, tNodeText)) LastAnswer = tNodeText;
    }

    protected virtual bool BlockedCheck() => Faulty;

    protected virtual bool OnTextNode(AtkTextNode* textNode, string text)
    {
        IPettableUser? user = lastPettableUser = GetUser();
        if (user == null) return false;

        IPetSheetData? pet = GetPetData(text, in user);
        if (pet == null) return false;

        string? customName = user.DataBaseEntry.GetName(pet.Model);
        if (customName == null) return false;

        SetText(textNode, text, customName, pet);
        return true;
    }

    protected virtual void SetText(AtkTextNode* textNode, string text, string customName, IPetSheetData pPet)
    {
        if (!CheckIfCanFunction(text, pPet)) return;
        LastAnswer = PetServices.StringHelper.ReplaceATKString(textNode, text, customName, pPet);
    }

    protected virtual bool CheckIfCanFunction(string text, IPetSheetData pPet)
    {
        if (AllowedToFunction == null) return true;
        if (AllowedToFunction.Invoke(pPet.Model)) return true;
        LastAnswer = text;
        return false;
    }

    protected virtual IPetSheetData? GetPetData(string text, in IPettableUser user) => PetServices.PetSheets.GetPetFromString(text, in user, IsSoft);

    protected virtual IPettableUser? GetUser() => PettableUserList.LocalPlayer;

    protected virtual AtkTextNode* GetTextNode(in BaseNode bNode)
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

    public void Dispose()
    {
        OnDispose();
        DirtyListener.UnregisterOnDirtyName(OnName);
        DirtyListener.UnregisterOnDirtyEntry(OnEntry);
        DirtyListener.UnregisterOnClearEntry(OnEntry);

        Services.AddonLifecycle.UnregisterListener(AddonEvent.PostRequestedUpdate, HandleUpdate);
    }

    public virtual void OnDispose() { }
}
