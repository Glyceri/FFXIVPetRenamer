using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using PetRenamer.PetNicknames.Services.Interface;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes.StyledNodes;

internal class GradiantBarNode : ResNode
{
    private readonly SimpleImageNode ProgressBar;
    private readonly SimpleImageNode OutlineBar;
    private readonly SimpleImageNode FocusOutlineBar;

    private Vector3 _colour;

    public GradiantBarNode(IPetServices petServices)
    {
        IsVisible = true;

        ProgressBar = new SimpleImageNode
        {
            IsVisible          = true,
            TexturePath        = "ui/uld/ConfigSystem.tex",
            TextureCoordinates = new Vector2(0, 50),
            TextureSize        = new Vector2(260, 5),
            WrapMode           = WrapMode.Stretch,
            ImageNodeFlags     = ImageNodeFlags.FlipH,
            ScaleY             = 0.5f,
            ScaleX             = 0.935f,
            Color              = new Vector4(_colour.X, _colour.Y, _colour.Z, 1),
        };

        OutlineBar = new SimpleImageNode
        {
            IsVisible          = true,
            TexturePath        = "ui/uld/Parameter_Gauge.tex",
            TextureCoordinates = new Vector2(0, 0),
            TextureSize        = new Vector2(160, 20),
            WrapMode           = WrapMode.Stretch,
        };

        FocusOutlineBar = new SimpleImageNode
        {
            IsVisible          = false,
            TexturePath        = "ui/uld/Parameter_Gauge.tex",
            TextureCoordinates = new Vector2(0, 20),
            TextureSize        = new Vector2(160, 20),
            WrapMode           = WrapMode.Stretch,
            Color              = new Vector4(0.75f, 0.75f, 0.75f, 1),
        };

        petServices.NativeController.AttachNode(ProgressBar, this);
        petServices.NativeController.AttachNode(OutlineBar, this);
        petServices.NativeController.AttachNode(FocusOutlineBar, this);
    }

    public required Vector3 Colour
    {
        get => _colour;
        set
        {
            _colour           = value;
            ProgressBar.Color = new Vector4(_colour.X, _colour.Y, _colour.Z, 1);
        }
    }

    public void Select()
    {
        FocusOutlineBar.IsVisible = true;
    }

    public void Unselect()
    {
        FocusOutlineBar.IsVisible = false;
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        FocusOutlineBar.Size = Size;
        OutlineBar.Size      = Size;

        ProgressBar.Size     = Size;
        ProgressBar.Y        = Size.Y * 0.5f * 0.5f;
        ProgressBar.X        = Size.X - (Size.X * 0.967f);
    }
}
