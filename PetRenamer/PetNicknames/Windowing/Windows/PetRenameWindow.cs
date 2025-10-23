using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Bindings.ImGui;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Components;
using PetRenamer.PetNicknames.Windowing.Components.Image;
using PetRenamer.PetNicknames.Windowing.Components.Labels;
using PetRenamer.PetNicknames.Windowing.Enums;
using System.Numerics;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using System;

namespace PetRenamer.PetNicknames.Windowing.Windows;

internal class PetRenameWindow : PetWindow
{
    private readonly IPettableUserList UserList;
    private readonly IPetServices PetServices;
    private readonly IPettableDirtyListener DirtyListener;

    protected override Vector2 MinSize     { get; } = new Vector2(570, 250);
    protected override Vector2 MaxSize     { get; } = new Vector2(1500, 250);
    protected override Vector2 DefaultSize { get; } = new Vector2(570, 250);

    protected override bool HasModeToggle  { get; } = true;

    private IPettableUser?           ActiveUser;
    private ulong                    lastContentID   = 0;
    private PetSkeleton              activeSkeleton  = PetSkeleton.CreateInvalid();
    private string?                  lastCustomName  = null!;
    private bool                     isContextOpen   = false;

    private string                   ActiveCustomName = string.Empty;
    private string                   ActualCustomName = string.Empty;
    private Vector3?                 ActiveEdgeColour = null;
    private Vector3?                 ActiveTextColour = null;
    private IPetSheetData?           ActivePetData;
    private ISharedImmediateTexture? ActivePetTexture;

    private float BarHeight 
        => 30 * WindowHandler.GlobalScale;

    public PetRenameWindow(WindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDirtyListener dirtyListener) : base(windowHandler, dalamudServices, petServices.Configuration, "Pet Rename Window", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        UserList = userList;
        PetServices = petServices;
        DirtyListener = dirtyListener;

        DirtyListener.RegisterOnPlayerCharacterDirty(DirtyPlayerChar);
    }

    protected override void OnDispose()
    {
        DirtyListener.UnregisterOnPlayerCharacterDirty(DirtyPlayerChar);
    }

    public void SetRenameWindow(PetSkeleton forSkeleton, bool open)
    {
        activeSkeleton = forSkeleton;

        if (open)
        {
            IsOpen = true;
        }

        if (forSkeleton.SkeletonType == SkeletonType.Minion)
        {
            SetPetMode(PetWindowMode.Minion);
        }
        else
        {
            SetPetMode(PetWindowMode.BattlePet);
        }

        isContextOpen  = true;
        activeSkeleton = forSkeleton;
        ActiveUser     = UserList.LocalPlayer;

        SetNewNode();
    }

    public override void OnOpen() 
    {
        if (isContextOpen)
        {
            return;
        }

        OnDraw();
        GetActiveSkeleton();
    }

    void DirtyPlayerChar(IPettableUser user)
    {
        if (user != ActiveUser)
        {
            return;
        }

        OnDirty();
    }

    protected override void OnDirty()
    {
        if (isContextOpen)
        {
            SetNewNode();

            return;
        }

        GetActiveSkeleton();
    }

    protected override void OnModeChange()
    {
        GetActiveSkeleton();
    }

    protected override void OnDraw()
    {
        Handle();
        DrawElement();
    }

    private void Handle()
    {
        ActiveUser = UserList.LocalPlayer;

        if (lastContentID != ActiveUser?.ContentID)
        {
            lastContentID = ActiveUser?.ContentID ?? 0;
            isContextOpen = false;

            GetActiveSkeleton();
        }
    }

    private void GetActiveSkeleton()
    {
        if (ActiveUser == null)
        {
            CleanOldNode();

            return;
        }

        IPettablePet? pet = ActiveUser.GetYoungestPet(CurrentMode == PetWindowMode.Minion ? IPettableUser.PetFilter.Minion : IPettableUser.PetFilter.BattlePet);
        
        if (pet == null)
        {
            activeSkeleton = PetSkeleton.CreateInvalid();

            CleanOldNode();

            return;
        }

        SetPet(pet);
    }

    private void SetPet(IPettablePet pet)
    {
        if (ActiveUser == null)
        {
            return;
        }

        bool dirty         = activeSkeleton != pet.SkeletonID;

        string? customName = ActiveUser.DataBaseEntry.GetName(activeSkeleton);

        if (lastCustomName != customName)
        {
            lastCustomName = customName;
            dirty          = true;
        }

        activeSkeleton = pet.SkeletonID;

        if (dirty)
        {
            SetNewNode();
        }
    }

    private void SetNewNode()
    {
        if (!IsOpen)
        {
            return;
        }

        if (ActiveUser == null)
        {
            return;
        }

        if (activeSkeleton.IsInvalid())
        {
            return;
        }

        IPetSheetData? data = PetServices.PetSheets.GetPet(activeSkeleton);

        if (data == null)
        {
            return;
        }

        string? customName  = ActiveUser.DataBaseEntry.GetName(activeSkeleton);

        lastCustomName      = customName;

        Vector3? edgeColour = ActiveUser.DataBaseEntry.GetEdgeColour(activeSkeleton);
        Vector3? textColour = ActiveUser.DataBaseEntry.GetTextColour(activeSkeleton);

        Setup(customName, edgeColour, textColour, in data);
    }

    private void CleanOldNode() 
        => Setup(null, null, null, null);

    private void Setup(string? customName, Vector3? edgeColour, Vector3? textColour, in IPetSheetData? petData)
    {
        ActiveCustomName = customName ?? string.Empty;
        ActualCustomName = ActiveCustomName;
        ActiveEdgeColour = edgeColour;
        ActiveTextColour = textColour;
        ActivePetData    = petData;

        if (petData != null)
        {
            ActivePetTexture = DalamudServices.TextureProvider.GetFromGameIcon(petData.Icon);
        }
        else
        {
            ActivePetTexture = null;
        }
    }

    private void OnSave(string? newName, Vector3? edgeColour, Vector3? textColour)
        => ActiveUser?.DataBaseEntry?.SetName(activeSkeleton, newName ?? string.Empty, edgeColour, textColour);
    
    private void DrawElement()
    {
        ImGuiStylePtr stylePtr = ImGui.GetStyle();
        float framePaddingX    = stylePtr.ItemSpacing.X;

        Vector2 region = ImGui.GetContentRegionAvail();
        float regionHeight = region.Y;

        if (Listbox.Begin("##RenameHolder", ImGui.GetContentRegionAvail() - new Vector2(regionHeight + framePaddingX, 0)))
        {
            DrawInternals();

            Listbox.End();
        }

        ImGui.SameLine();

        DrawImageInternals(regionHeight);
    }

    private void DrawInternals()
    {
        if (ActivePetData == null)
        {
            CenteredLabel.Draw(Translator.GetLine("PetRenameNode.PleaseSummonWarning"), new Vector2(ImGui.GetContentRegionAvail().X * 0.8f, BarHeight));
        }
        else
        {
            DrawPetData();
        }
    }

    private void DrawPetData()
    {
        Vector2 contentSpot = new Vector2(ImGui.GetContentRegionAvail().X, BarHeight);

        LabledLabel.Draw($"{Translator.GetLine(CurrentMode == PetWindowMode.Minion ? "PetRenameNode.Species" : "PetRenameNode.Species2")}:", ActivePetData?.BaseSingular ?? Translator.GetLine("..."), contentSpot);
        LabledLabel.Draw("ID:", ActivePetData?.Model.SkeletonId.ToString() ?? Translator.GetLine("..."), contentSpot);
        LabledLabel.Draw($"{Translator.GetLine("PetRenameNode.Race")}:", ActivePetData?.RaceName ?? Translator.GetLine("..."), contentSpot);
        LabledLabel.Draw($"{Translator.GetLine("PetRenameNode.Behaviour")}:", ActivePetData?.BehaviourName ?? Translator.GetLine("..."), contentSpot);

        if (RenameLabel.Draw($"{Translator.GetLine("PetRenameNode.Nickname")}:", ActiveCustomName == ActualCustomName, ref ActiveCustomName, ref ActiveEdgeColour, ref ActiveTextColour, contentSpot))
        {
            OnSave(ActiveCustomName, ActiveEdgeColour, ActiveTextColour);
        }

        ActiveCustomName = ActiveCustomName.Replace(Environment.NewLine, string.Empty);
    }

    private void DrawImageInternals(float regionHeight)
    {
        Vector2 size = new Vector2(regionHeight, regionHeight);

        if (ActivePetTexture == null)
        {
            IDalamudTextureWrap? searchTexture = SearchImage.SearchTextureWrap;

            if (searchTexture != null)
            {
                BoxedImage.Draw(searchTexture, size);
                
            }
        }
        else if (ActivePetData != null)
        {
            if (Listbox.Begin("##image", size))
            {
                BoxedImage.DrawMinion(ActivePetData, in DalamudServices, in Configuration, ImGui.GetContentRegionAvail());

                Listbox.End();
            }
        }
    }
}
