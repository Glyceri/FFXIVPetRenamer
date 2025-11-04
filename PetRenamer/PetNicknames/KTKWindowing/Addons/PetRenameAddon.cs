using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Widgets.Parts;
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
    private PetImageNode?      ImageNode;
    private PetFootstepIcon?   FootstepNode;
    private PetRenameNode?     PetRenameNode;

    private PetSkeleton?       CurrentSkeleton;

    private ColorPreviewNode?  TextColourPicker;
    private ColorPreviewNode?  EdgeColourPicker;

    private NineGridNode?      NineGridNode;

    private FocusableButtonNode? FocusableButtonNode1;
    private FocusableButtonNode? FocusableButtonNode2;
    private FocusableButtonNode? FocusableButtonNode3;
    private FocusableButtonNode? FocusableButtonNode4;
    private FocusableButtonNode? FocusableButtonNode5;

    [SetsRequiredMembers]
    public PetRenameAddon(KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDatabase database, PettableDirtyHandler dirtyHandler) 
        : base(windowHandler, dalamudServices, petServices, userList, database, dirtyHandler) { }

    protected override string WindowInternalName
        => nameof(PetRenameAddon);

    protected override Vector2 WindowSize
        => new Vector2(520, 200);

    protected override bool HasPetBar
        => true;

    public override string WindowTooltip
        => "Pet Nicknames";

    protected override unsafe void OnAddonSetup(AtkUnitBase* addon)
    {
        DirtyHandler.RegisterOnPlayerCharacterDirty(OnDirtyPlayer);



        NineGridNode           = new SimpleNineGridNode
        {
            IsVisible          = true,
            TexturePath        = "ui/uld/BgParts.tex",
            Position           = new Vector2(200, 35),
            Size               = new Vector2(200, 100),
            TextureCoordinates = new Vector2(61, 37),
            TextureSize        = new Vector2(16, 16),
            TopOffset          = 6,
            BottomOffset       = 6,
            LeftOffset         = 6,
            RightOffset        = 6,
        };
        
        AttachNode(ref NineGridNode);

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

        TextColourPicker = new ColorPreviewNode()
        {
            Position  = new Vector2(393, 0),
            Size      = new Vector2(28, 28),
            IsVisible = true,
            Color     = new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
        };

        AttachNode(ref TextColourPicker);


        EdgeColourPicker = new ColorPreviewNode()
        {
            Position  = new Vector2(420, 0),
            Size      = new Vector2(28, 28),
            IsVisible = true,
            Color     = new Vector4(0, 0, 0, 0.5f),
        };

        AttachNode(ref EdgeColourPicker);


        

        FocusableButtonNode1 = new FocusableButtonNode(WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size = new Vector2(90, 90),
            Index = 1,
        };

        AttachNode(ref FocusableButtonNode1);

        addon->FocusNode = FocusableButtonNode1.TextureButtonNode;
        addon->CursorTarget = FocusableButtonNode1.TextureButtonNode.CollisionNode;

        FocusableButtonNode2 = new FocusableButtonNode(WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size = new Vector2(90, 90),
            Position = new Vector2(90, 0),
            Index = 2,
        };

        AttachNode(ref FocusableButtonNode2);

        FocusableButtonNode3 = new FocusableButtonNode(WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size = new Vector2(90, 90),
            Position = new Vector2(180, 0),
            Index = 3,
        };

        AttachNode(ref FocusableButtonNode3);

        FocusableButtonNode4 = new FocusableButtonNode(WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size = new Vector2(90, 90),
            Position = new Vector2(270, 0),
            Index = 4,
        };

        AttachNode(ref FocusableButtonNode4);

        FocusableButtonNode5 = new FocusableButtonNode(WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size = new Vector2(90, 90),
            Position = new Vector2(360, 0),
            Index = 5,
        };

        AttachNode(ref FocusableButtonNode5);

        SetData();
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

    private void OnDirtyPlayer(IPettableUser player)
        => SetDirty();

    protected override unsafe void OnAddonFinalize(AtkUnitBase* addon)
    {
        DirtyHandler.UnregisterOnPlayerCharacterDirty(OnDirtyPlayer);

        ImageNode       = null;
        FootstepNode    = null;
        PetRenameNode   = null;
        CurrentSkeleton = null;
    }

    protected override void OnDirty()
        => SetData();

    private void SetData()
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
            CurrentSkeleton = selectedPet.SkeletonID;

            IPetSheetData? petData = selectedPet.PetData;

            if (petData == null)
            {
                return;
            }

            ImageNode.PetData      = petData;
            FootstepNode.PetData   = petData;

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
