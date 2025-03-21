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
using System.Numerics;
using Lumina.Text.ReadOnly;

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
    protected bool AllowColours;

    string AddonName = string.Empty;

    protected IPettableUser? CurrentUser;
    protected IPetSheetData? CurrentPet;
    protected IPettableDatabaseEntry? CurrentDatabaseEntry;

    public virtual void Setup(DalamudServices services, IPettableUserList userList, IPetServices petServices, IPettableDirtyListener dirtyListener, string addonName, uint[] textPos, Func<int, bool> allowedCallback, bool allowColours, bool isSoft = false)
    {
        Services = services;
        PettableUserList = userList;
        PetServices = petServices;
        DirtyListener = dirtyListener;
        TextPos = textPos;
        AllowedToFunction = allowedCallback;
        AllowColours = allowColours;
        IsSoft = isSoft;
        AddonName = addonName;

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

    protected void SetDirty()
    {
        isDirty = true;
    }

    protected void ClearDirty()
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
        string tNodeText = new ReadOnlySeStringSpan(tNode->NodeText).ExtractText();
        if ((tNodeText == string.Empty || tNodeText == LastAnswer) && !isDirty) return;
        ClearDirty();

        if (!OnTextNode(tNode, tNodeText)) LastAnswer = tNodeText;
    }

    protected virtual bool BlockedCheck() => Faulty;

    protected virtual bool OnTextNode(AtkTextNode* textNode, string text)
    {
        CurrentUser = GetUser();
        if (CurrentUser == null) return false;

        CurrentPet = GetPetData(text, in CurrentUser);
        if (CurrentPet == null) return false;

        CurrentDatabaseEntry = CurrentUser.DataBaseEntry;
        if (CurrentDatabaseEntry == null) return false;

        string? customName = CurrentDatabaseEntry.GetName(CurrentPet.Model);
        if (customName == null) return false;

        SetText(textNode, text, customName, CurrentPet);
        return true;
    }

    protected virtual void SetText(AtkTextNode* textNode, string text, string customName, IPetSheetData pPet)
    {
        if (!CheckIfCanFunction(text, pPet)) return;

        Vector3? edgeColour = null;
        Vector3? textColour = null;

        if (AllowColours)
        {
            GetColours(out edgeColour, out textColour);
        }

        LastAnswer = PetServices.StringHelper.ReplaceATKString(textNode, text, customName, edgeColour, textColour, pPet);
    }

    protected virtual void GetColours(out Vector3? edgeColour, out Vector3? textColour)
    {
        edgeColour = null;
        textColour = null;

        if (CurrentUser == null) return;
        if (CurrentPet == null) return;
        if (CurrentDatabaseEntry == null) return;

        int colourSetting = PetServices.Configuration.showColours;

        if (colourSetting >= 2) return;
        if (colourSetting == 1 && !CurrentUser.IsLocalPlayer) return;

        edgeColour = CurrentDatabaseEntry.GetEdgeColour(CurrentPet.Model);
        textColour = CurrentDatabaseEntry.GetTextColour(CurrentPet.Model);
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
