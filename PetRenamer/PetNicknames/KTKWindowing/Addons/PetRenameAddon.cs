using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addons;
using KamiToolKit.Nodes;
using KamiToolKit.Widgets.Parts;
using PetRenamer.PetNicknames.Hooking.Enum;
using PetRenamer.PetNicknames.KTKWindowing.Base;
using PetRenamer.PetNicknames.KTKWindowing.Nodes;
using PetRenamer.PetNicknames.KTKWindowing.Nodes.FunctionalNodes;
using PetRenamer.PetNicknames.KTKWindowing.Nodes.StyledNodes;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
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

    private GlowyBarNode?      GlowyBarNode;
    private SimpleOutlineNode? SimpleOutlineNode;
    private DoubleArrowNode?   DoubleArrowNode;
    private ColourButton?      ColourButton;
    private ColourButton?      ColourButton2;
    private ColourButton?      ColourButton3;
    private ColourButton?      ColourButton4;
    private ColourButton?      ColourButton5;

    private MaskTest? MaskTest;

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



        NineGridNode = new SimpleNineGridNode
        {
            IsVisible = true,
            TexturePath = "ui/uld/BgParts.tex",
            Position = new Vector2(200, 35),
            Size = new Vector2(200, 100),
            TextureCoordinates = new Vector2(61, 37),
            TextureSize = new Vector2(16, 16),
            TopOffset = 6,
            BottomOffset = 6,
            LeftOffset = 6,
            RightOffset = 6,
        };

        AttachNode(ref NineGridNode);

        GlowyBarNode = new GlowyBarNode
        {
            Position = new Vector2(300, 100),
        };

        AttachNode(GlowyBarNode);

        SimpleOutlineNode = new SimpleOutlineNode
        {
            Position = new Vector2(412, 140),
            Width    = 100,
            Height   = 22,
        };

        AttachNode(SimpleOutlineNode);

        DoubleArrowNode = new DoubleArrowNode
        {
            Position = new Vector2(300, 140),
            ScaleX = 0.5f
        };

        AttachNode(DoubleArrowNode);


        ColourButton = new ColourButton(WindowHandler, PetServices)
        {
            Position = new Vector2(200, 150),
            Size     = new Vector2(30, 30),
            Colour   = new Vector3(1, 0, 0),
        };

        NavigationHelper.SetNavigation(ref ColourButton, new ControllerNavigation
        {
            Index      = 1,
            LeftIndex  = 5,
            RightIndex = 2,
            LeftStop   = true,
        });

        AttachNode(ColourButton);

        ColourButton2 = new ColourButton(WindowHandler, PetServices)
        {
            Position = new Vector2(232, 150),
            Size     = new Vector2(30, 30),
            Colour   = new Vector3(1, 0, 0),
        };

        NavigationHelper.SetNavigation(ref ColourButton2, new ControllerNavigation
        {
            Index      = 2,
            LeftIndex  = 1,
            RightIndex = 3,
        });

        AttachNode(ColourButton2);



        ColourButton3 = new ColourButton(WindowHandler, PetServices)
        {
            Position = new Vector2(264, 150),
            Size     = new Vector2(30, 30),
            Colour   = new Vector3(1, 0, 0),
        };

        NavigationHelper.SetNavigation(ref ColourButton3, new ControllerNavigation
        {
            Index      = 3,
            LeftIndex  = 2,
            RightIndex = 4,
        });

        AttachNode(ColourButton3);



        ColourButton4 = new ColourButton(WindowHandler, PetServices)
        {
            Position = new Vector2(296, 150),
            Size     = new Vector2(30, 30),
            Colour   = new Vector3(1, 0, 0),
        };

        NavigationHelper.SetNavigation(ref ColourButton4, new ControllerNavigation
        {
            Index      = 4,
            LeftIndex  = 3,
            RightIndex = 5,
        });

        AttachNode(ColourButton4);



        ColourButton5 = new ColourButton(WindowHandler, PetServices)
        {
            Position = new Vector2(328, 150),
            Size     = new Vector2(30, 30),
            Colour   = new Vector3(1, 0, 0),
        };

        NavigationHelper.SetNavigation(ref ColourButton5, new ControllerNavigation
        {
            Index      = 5,
            LeftIndex  = 4,
            RightIndex = 1,
            RightStop  = true,
        });

        AttachNode(ColourButton5);

        MaskTest = new MaskTest(DalamudServices, PetServices)
        {
            Size = new Vector2(50, 50),
            Position = new Vector2(50, 80),
            IsVisible = true,
        };

        AttachNode(MaskTest);

        ImageNode = new PetImageNode(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size = new Vector2(142, 142),
            IsVisible = false
        };

        AttachNode(ref ImageNode);

        FootstepNode = new PetFootstepIcon(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size = new Vector2(142, 142),
            Position = new Vector2(142, 0)
        };

        AttachNode(ref FootstepNode);

        PetRenameNode = new PetRenameNode(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Position = new Vector2(200, 0),
            Size = new Vector2(200, 28),
            IsVisible = true,
            OnNameComplete = OnNameEditComplete
        };

        AttachNode(ref PetRenameNode);

        TextColourPicker = new ColorPreviewNode()
        {
            Position = new Vector2(393, 0),
            Size = new Vector2(28, 28),
            IsVisible = true,
            Color = new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
        };

        AttachNode(ref TextColourPicker);


        EdgeColourPicker = new ColorPreviewNode()
        {
            Position = new Vector2(420, 0),
            Size = new Vector2(28, 28),
            IsVisible = true,
            Color = new Vector4(0, 0, 0, 0.5f),
        };

        AttachNode(ref EdgeColourPicker);



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
        MaskTest = null;
    }

    // External call to set the active skeleton of the window (always bound to local player c:)
    public void SetPetSkeleton(PetSkeleton skeleton)
    {
        // TODO: IMPLEMENT
    }

    protected override void OnDirty()
        => SetData();

    protected override unsafe void OnDraw(AtkUnitBase* addon)
    {
        MaskTest?.Draw();
    }

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
