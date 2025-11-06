using KamiToolKit.Nodes;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.KTKWindowing.Base;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes;

internal class FocusableButtonNode : NavigableComponent
{
    public readonly TextureButtonNode TextureButtonNode;

    public FocusableButtonNode(KTKAddon parentAddon, KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, PettableDirtyHandler dirtyHandler) 
        : base(parentAddon, windowHandler, dalamudServices, petServices, dirtyHandler)
    {
        TextureButtonNode = new TextureButtonNode
        {
            IsVisible          = true,
            TextureCoordinates = new Vector2(0, 26),
            TexturePath        = "ui/uld/MiragePrismPlate2.tex",
            TextureSize        = new Vector2(56, 26),
            NodeFlags          = NodeFlags.Focusable | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.AnchorBottom | NodeFlags.AnchorRight | NodeFlags.HasCollision | NodeFlags.RespondToMouse,
        };

        StandinNode = TextureButtonNode.CollisionNode;

        AttachNode(ref TextureButtonNode);
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        TextureButtonNode.Size = Size;
    }
}
