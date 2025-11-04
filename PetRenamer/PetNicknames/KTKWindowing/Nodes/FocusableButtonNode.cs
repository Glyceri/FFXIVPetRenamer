using KamiToolKit.Nodes;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes;

internal class FocusableButtonNode : KTKComponent
{
    public readonly TextureButtonNode TextureButtonNode;

    public FocusableButtonNode(KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, PettableDirtyHandler dirtyHandler) 
        : base(windowHandler, dalamudServices, petServices, dirtyHandler)
    {
        IsVisible = true;

        TextureButtonNode = new TextureButtonNode
        {
            IsVisible          = true,
            TextureCoordinates = new Vector2(0, 26),
            TexturePath        = "ui/uld/MiragePrismPlate2.tex",
            TextureSize        = new Vector2(56, 26),
            NodeFlags          = NodeFlags.Focusable | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorBottom | NodeFlags.AnchorRight | NodeFlags.HasCollision | NodeFlags.RespondToMouse,
        };

        TextureButtonNode.CollisionNode.NodeFlags = NodeFlags.Focusable | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorBottom | NodeFlags.AnchorRight | NodeFlags.HasCollision | NodeFlags.RespondToMouse;

        AttachNode(ref TextureButtonNode);
    }

    public unsafe required byte Index
    {
        get => TextureButtonNode.ComponentBase->CursorNavigationInfo.Index;
        set
        {
            TextureButtonNode.ComponentBase->CursorNavigationInfo.Index = value;

            TextureButtonNode.ComponentBase->CursorNavigationInfo.LeftIndex = (byte)(value - 1);
            TextureButtonNode.ComponentBase->CursorNavigationInfo.RightIndex = (byte)(value + 1);
        }
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        TextureButtonNode.Size = Size;
    }
}
