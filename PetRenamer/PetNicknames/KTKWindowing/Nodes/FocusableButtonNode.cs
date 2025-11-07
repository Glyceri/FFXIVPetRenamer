using KamiToolKit.Nodes;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System.Numerics;
using PetRenamer.PetNicknames.KTKWindowing.Base;
using System.Diagnostics.CodeAnalysis;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes;

internal unsafe class FocusableButtonNode : KTKResNode
{
    public readonly TextureButtonNode TextureButtonNode;

    [SetsRequiredMembers]
    public FocusableButtonNode(KTKAddon parentAddon, KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, PettableDirtyHandler dirtyHandler) 
        : base (parentAddon, windowHandler, dalamudServices, petServices, dirtyHandler)
    {
        TextureButtonNode = new TextureButtonNode
        {
            IsVisible = true,
            TextureCoordinates = new Vector2(0, 26),
            TexturePath = "ui/uld/MiragePrismPlate2.tex",
            TextureSize = new Vector2(56, 26),
        };

        petServices.NativeController.AttachNode(TextureButtonNode, this);
    }

    public required byte NavigationIndex
    {
        get => TextureButtonNode.ComponentBase->CursorNavigationInfo.Index;
        set => TextureButtonNode.ComponentBase->CursorNavigationInfo.Index = value;
    }

    public byte LeftIndex
    {
        get => TextureButtonNode.ComponentBase->CursorNavigationInfo.LeftIndex;
        set => TextureButtonNode.ComponentBase->CursorNavigationInfo.LeftIndex = value;
    }

    public byte RightIndex
    {
        get => TextureButtonNode.ComponentBase->CursorNavigationInfo.RightIndex;
        set => TextureButtonNode.ComponentBase->CursorNavigationInfo.RightIndex = value;
    }

    public byte UpIndex
    {
        get => TextureButtonNode.ComponentBase->CursorNavigationInfo.UpIndex;
        set => TextureButtonNode.ComponentBase->CursorNavigationInfo.UpIndex = value;
    }

    public byte DownIndex
    {
        get => TextureButtonNode.ComponentBase->CursorNavigationInfo.DownIndex;
        set => TextureButtonNode.ComponentBase->CursorNavigationInfo.DownIndex = value;
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        TextureButtonNode.Size = Size;
    }
}
