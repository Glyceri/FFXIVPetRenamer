using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using PetRenamer.PetNicknames.KTKWindowing.Base;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Addons;

internal class PetListAddon : KTKAddon
{
    private CheckboxNode? firstCheckboxNode;
    private CheckboxNode? secondCheckboxNode;

    [SetsRequiredMembers]
    public PetListAddon(KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDatabase database, PettableDirtyHandler dirtyHandler)
       : base(windowHandler, dalamudServices, petServices, userList, database, dirtyHandler) { }

    protected override string WindowInternalName
        => nameof(PetListAddon);

    protected override Vector2 WindowSize
        => new Vector2(520, 200);

    protected override bool HasPetBar
        => true;

    public override string WindowTooltip
        => "Pet List & Sharing";

    protected override unsafe void OnAddonSetup(AtkUnitBase* addon)
    {
        firstCheckboxNode = new CheckboxNode
        {
            NodeId    = 1,
            Size      = new Vector2(20.0f, 20.0f),
            Position  = ContentStartPosition,
            IsVisible = true,
            String    = "First",
        };
        AttachNode(firstCheckboxNode);

        firstCheckboxNode.ComponentBase->CursorNavigationInfo.Index = 1;
        firstCheckboxNode.ComponentBase->CursorNavigationInfo.UpIndex = 2;
        firstCheckboxNode.ComponentBase->CursorNavigationInfo.DownIndex = 2;

        addon->FocusNode = firstCheckboxNode;

        secondCheckboxNode = new CheckboxNode
        {
            NodeId    = 2,
            Size      = new Vector2(20.0f, 20.0f),
            Position  = ContentStartPosition + new Vector2(0.0f, firstCheckboxNode.X + firstCheckboxNode.Height),
            IsVisible = true,
            String    = "Second",
        };
        AttachNode(secondCheckboxNode);

        secondCheckboxNode.ComponentBase->CursorNavigationInfo.Index = 2;
        secondCheckboxNode.ComponentBase->CursorNavigationInfo.UpIndex = 1;
        secondCheckboxNode.ComponentBase->CursorNavigationInfo.DownIndex = 1;
    }
}
