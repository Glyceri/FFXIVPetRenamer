using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.KTKWindowing.Nodes;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PetRenamer.PetNicknames.Windowing.Enums;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Addons;

internal class PetRenameAddon : KTKAddon
{
    private PetImageNode?    ImageNode;
    private PetFootstepIcon? FootstepNode;
    private PetRenameNode?   PetRenameNode;

    private PetSkeleton?     CurrentSkeleton;

    [SetsRequiredMembers]
    public PetRenameAddon(KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDatabase database, PettableDirtyHandler dirtyHandler) 
        : base(windowHandler, dalamudServices, petServices, userList, database, dirtyHandler) { }

    protected override string WindowInternalName
        => nameof(PetRenameAddon);

    protected override Vector2 WindowSize
        => new Vector2(520, 200);

    protected override bool HasPetBar
        => true;

    protected override unsafe void OnAddonSetup(AtkUnitBase* addon)
    {
        ImageNode       = new PetImageNode(WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size        = new Vector2(142, 142),
        };

        AttachNode(ref ImageNode);

        FootstepNode    = new PetFootstepIcon(WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size        = new Vector2(142, 142),
            Position    = new Vector2(142, 0)
        };

        AttachNode(ref FootstepNode);

        PetRenameNode = new PetRenameNode(WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Position        = new Vector2(200, 0),
            Size            = new Vector2(200, 28),
            IsVisible       = true,
            OnNameComplete  = OnNameEditComplete
        };

        AttachNode(ref PetRenameNode);
    }

    private void OnNameEditComplete(SeString finalName)
    {
        if (UserList.LocalPlayer == null)
        {
            return;
        }

        if (CurrentSkeleton == null) 
        {
            return;
        }

        Vector3? textColour = UserList.LocalPlayer.DataBaseEntry.GetTextColour(CurrentSkeleton.Value);
        Vector3? edgeColour = UserList.LocalPlayer.DataBaseEntry.GetEdgeColour(CurrentSkeleton.Value);

        string newCustomName = finalName.TextValue;

        UserList.LocalPlayer.DataBaseEntry.SetName(CurrentSkeleton.Value, newCustomName, edgeColour, textColour);
    }

    private void DirtyPlayerChar(IPettableUser player)
        => SetDirty();

    protected override unsafe void OnAddonFinalize(AtkUnitBase* addon)
    {
        DirtyHandler.UnregisterOnPlayerCharacterDirty(DirtyPlayerChar);

        ImageNode       = null;
        FootstepNode    = null;
        PetRenameNode   = null;
        CurrentSkeleton = null;
    }

    protected override void OnDirty()
    {
        CurrentSkeleton = null;

        if (ImageNode == null)
        {
            return;
        }

        if (FootstepNode == null)
        {
            return;
        }

        if (PetRenameNode == null)
        {
            return;
        }

        IPettablePet? selectedPet = null;

        if (PetMode == PetWindowMode.Minion)
        {
            selectedPet = UserList.LocalPlayer?.GetYoungestPet(IPettableUser.PetFilter.Minion);
        }
        else if (PetMode == PetWindowMode.BattlePet)
        {
            selectedPet = UserList.LocalPlayer?.GetYoungestPet(IPettableUser.PetFilter.BattlePet);
        }

        if (selectedPet == null)
        {
            ImageNode.PetData            = null;
            FootstepNode.PetData         = null;
            PetRenameNode.NicknameString = string.Empty;
        }
        else
        {
            CurrentSkeleton        = selectedPet.SkeletonID;

            IPetSheetData? petData = selectedPet.PetData;

            if (petData == null)
            {
                return;
            }

            ImageNode.PetData    = petData;
            FootstepNode.PetData = petData;

            string? customNickname = UserList.LocalPlayer?.GetCustomName(petData);

            if (customNickname.IsNullOrWhitespace())
            {
                PetRenameNode.NicknameString = string.Empty;
            }
            else
            {
                PetRenameNode.NicknameString = customNickname;
            }
        }
    }
}
