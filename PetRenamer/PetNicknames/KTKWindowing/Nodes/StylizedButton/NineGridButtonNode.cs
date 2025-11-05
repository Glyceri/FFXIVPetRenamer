using KamiToolKit.Nodes;
using PetRenamer.PetNicknames.Services.Interface;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes.StylizedButton;

internal class NineGridButtonNode : ButtonBase
{
    public readonly SimpleNineGridNode ImageNode;

    public NineGridButtonNode(IPetServices petServices)
    {
        ImageNode = new SimpleNineGridNode
        {
            IsVisible = true,
        };

        petServices.NativeController.AttachNode(ImageNode, this);

        LoadTimelines();

        InitializeComponentEvents();
    }

    public float LeftOffset
    {
        get => ImageNode.LeftOffset;
        set => ImageNode.LeftOffset = value;
    }

    public float RightOffset
    {
        get => ImageNode.RightOffset;
        set => ImageNode.RightOffset = value;
    }

    public required string TexturePath
    {
        get => ImageNode.TexturePath;
        set => ImageNode.TexturePath = value;
    }

    public Vector2 TextureCoordinates
    {
        get => ImageNode.TextureCoordinates;
        set => ImageNode.TextureCoordinates = value;
    }

    public required Vector2 TextureSize
    {
        get => ImageNode.TextureSize;
        set => ImageNode.TextureSize = value;
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        ImageNode.Size = Size;
    }

    private void LoadTimelines()
        => LoadTwoPartTimelines(this, ImageNode);
}