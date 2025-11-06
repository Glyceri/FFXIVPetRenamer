using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Widgets.Parts;
using PetRenamer.PetNicknames.KTKWindowing.Base;
using PetRenamer.PetNicknames.KTKWindowing.ControllerNavigation.Implementations;
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

    private FocusableButtonNode? FocusableButtonNode6;
    private FocusableButtonNode? FocusableButtonNode7;
    private FocusableButtonNode? FocusableButtonNode8;
    private FocusableButtonNode? FocusableButtonNode9;
    private FocusableButtonNode? FocusableButtonNode10;

    [SetsRequiredMembers]
    public PetRenameAddon(KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDatabase database, PettableDirtyHandler dirtyHandler) 
        : base(windowHandler, dalamudServices, petServices, userList, database, dirtyHandler) 
    {
        ControllerNavigator = new BasicControllerNavigation(PetServices);
    }

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

        ImageNode       = new PetImageNode(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size        = new Vector2(142, 142),
        };

        AttachNode(ref ImageNode);

        FootstepNode    = new PetFootstepIcon(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size        = new Vector2(142, 142),
            Position    = new Vector2(142, 0)
        };

        AttachNode(ref FootstepNode);

        PetRenameNode = new PetRenameNode(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
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


        

        FocusableButtonNode1 = new FocusableButtonNode(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size     = new Vector2(90, 90),
            Position = new Vector2(0, -15),
            NavigationIndex    = 1,
            RightIndex = 2,
            LeftIndex = 5,
            UpIndex = 6,
            DownIndex = 6,
            LeftStopFlag = true,
        };

        AttachNode(ref FocusableButtonNode1);

        ControllerNavigator.SetFocus(FocusableButtonNode1);        

        FocusableButtonNode2 = new FocusableButtonNode(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size     = new Vector2(90, 90),
            Position = new Vector2(90, -15),
            NavigationIndex = 2,
            RightIndex = 3,
            LeftIndex = 1,
            UpIndex = 7,
            DownIndex = 7,
        };

        AttachNode(ref FocusableButtonNode2);

        FocusableButtonNode3 = new FocusableButtonNode(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size     = new Vector2(90, 90),
            Position = new Vector2(180, -15),
            NavigationIndex = 3,
            RightIndex = 4,
            LeftIndex = 2,
            UpIndex = 8,
            DownIndex = 8,
        };

        AttachNode(ref FocusableButtonNode3);

        FocusableButtonNode4 = new FocusableButtonNode(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size     = new Vector2(90, 90),
            Position = new Vector2(270, -15),
            NavigationIndex = 4,
            RightIndex = 5,
            LeftIndex = 3,
            UpIndex = 9,
            DownIndex = 9,
        };

        AttachNode(ref FocusableButtonNode4);

        FocusableButtonNode5 = new FocusableButtonNode(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size     = new Vector2(90, 90),
            Position = new Vector2(360, -15),
            NavigationIndex = 5,
            RightIndex = 1,
            LeftIndex = 4,
            UpIndex = 10,
            DownIndex = 10,
            RightStopFlag = true,
        };

        AttachNode(ref FocusableButtonNode5);


        FocusableButtonNode6 = new FocusableButtonNode(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size = new Vector2(90, 90),
            Position = new Vector2(0, 65),
            NavigationIndex = 6,
            RightIndex = 7,
            LeftIndex = 10,
            UpIndex = 1,
            DownIndex = 1,
            LeftStopFlag = true,
        };

        AttachNode(ref FocusableButtonNode6);

        FocusableButtonNode7 = new FocusableButtonNode(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size = new Vector2(90, 90),
            Position = new Vector2(90, 65),
            NavigationIndex = 7,
            RightIndex = 8,
            LeftIndex = 6,
            UpIndex = 2,
            DownIndex = 2,
        };

        AttachNode(ref FocusableButtonNode7);

        FocusableButtonNode8 = new FocusableButtonNode(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size = new Vector2(90, 90),
            Position = new Vector2(180, 65),
            NavigationIndex = 8,
            RightIndex = 9,
            LeftIndex = 7,
            UpIndex = 3,
            DownIndex = 3,
        };

        AttachNode(ref FocusableButtonNode8);

        FocusableButtonNode9 = new FocusableButtonNode(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size = new Vector2(90, 90),
            Position = new Vector2(270, 65),
            NavigationIndex = 9,
            RightIndex = 10,
            LeftIndex = 8,
            UpIndex = 4,
            DownIndex = 4,
        };

        AttachNode(ref FocusableButtonNode9);

        FocusableButtonNode10 = new FocusableButtonNode(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size = new Vector2(90, 90),
            Position = new Vector2(360, 65),
            NavigationIndex = 10,
            RightIndex = 6,
            LeftIndex = 9,
            UpIndex = 5,
            DownIndex = 5,
            RightStopFlag = true,
        };

        AttachNode(ref FocusableButtonNode10);


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

    // External call to set the active skeleton of the window (always bound to local player c:)
    public void SetPetSkeleton(PetSkeleton skeleton)
    {
        // TODO: IMPLEMENT
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
