using KamiToolKit.Nodes;
using PetRenamer.PetNicknames.Services.Interface;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes.FunctionalNodes;

internal class ColourButton : ButtonBase
{
    private readonly SimpleImageNode ColourImage;
    private readonly SimpleImageNode BorderImageNode;
    private readonly SimpleImageNode SelectedImageNode;
    private readonly SimpleImageNode NoColourImage;
    private readonly SimpleImageNode StrikethroughImage;

    private Vector3? _currentColour;

    public ColourButton(KTKWindowHandler windowHandler, IPetServices petServices)
    {
        IsVisible = true;
        IsEnabled = true;

        OnClick   = windowHandler.ColourPicker.Open;
        //OnClick = windowHandler.KTKColorPickerReference.Open;

        ColourImage = new SimpleImageNode
        {
            TexturePath        = "ui/uld/ListColorChooser.tex",
            TextureCoordinates = new Vector2(5, 53),
            TextureSize        = new Vector2(30, 30),
            WrapMode           = WrapMode.Stretch,
            IsVisible          = true,
        };

        petServices.NativeController.AttachNode(ColourImage, this);

        NoColourImage = new SimpleImageNode
        {
            TexturePath        = "ui/uld/ListColorChooser.tex",
            TextureCoordinates = new Vector2(53, 93),
            TextureSize        = new Vector2(30, 30), 
            WrapMode           = WrapMode.Stretch,
            IsVisible          = false,
        };

        petServices.NativeController.AttachNode(NoColourImage, this);

        BorderImageNode = new SimpleImageNode
        {
            TexturePath        = "ui/uld/ListColorChooser.tex",
            TextureCoordinates = new Vector2(45, 53),
            TextureSize        = new Vector2(30, 30),
            WrapMode           = WrapMode.Stretch,
            IsVisible          = true,
        };

        petServices.NativeController.AttachNode(BorderImageNode, this);

        SelectedImageNode = new SimpleImageNode
        {
            TexturePath        = "ui/uld/ListColorChooser.tex",
            TextureCoordinates = new Vector2(85, 53),
            TextureSize        = new Vector2(30, 30),
            WrapMode           = WrapMode.Stretch,
            IsVisible          = false,
        };

        petServices.NativeController.AttachNode(SelectedImageNode, this);

        StrikethroughImage = new SimpleImageNode
        {
            TexturePath        = "ui/uld/ListColorChooser.tex",
            //TextureCoordinates = new Vector2(93, 93),
            //TextureSize        = new Vector2(30, 30),

            TextureCoordinates = new Vector2(96, 96),
            TextureSize        = new Vector2(24, 24),
            WrapMode           = WrapMode.Stretch,
            

            Color              = new Vector4(1, 1, 1, 1),
            Origin             = new Vector2(12, 12),
            Scale              = new Vector2(0.8f),
            IsVisible          = false,
        };

        petServices.NativeController.AttachNode(StrikethroughImage, this);

        CollisionNode.AddEvent(AtkEventType.FocusStart, () =>
        {
            SelectedImageNode.IsVisible = true;
        });

        CollisionNode.AddEvent(AtkEventType.FocusStop, () =>
        {
            SelectedImageNode.IsVisible = false;
        });

        LoadTimelines();

        InitializeComponentEvents();
    }

    public required Vector3? Colour
    {
        get => _currentColour;
        set
        {
            _currentColour = value;

            if (value == null)
            {
                StrikethroughImage.IsVisible = true;
                NoColourImage.IsVisible      = true;
                ColourImage.IsVisible        = false;
            }
            else
            {
                StrikethroughImage.IsVisible = false;
                NoColourImage.IsVisible      = false;
                ColourImage.IsVisible        = true;
                ColourImage.Color            = new Vector4(value.Value.X, value.Value.Y, value.Value.Z, 1);
            }
        }
    }

    private void LoadTimelines()
    {
        LoadThreePartTimelines(this, ColourImage, SelectedImageNode,  new Vector2(0, 0f));
        LoadThreePartTimelines(this, ColourImage, BorderImageNode,    new Vector2(0, 0f));
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        ColourImage.Size            = Size;
        SelectedImageNode.Size      = Size;
        BorderImageNode.Size        = Size;
        NoColourImage.Size          = Size;
        StrikethroughImage.Size     = Size;
        StrikethroughImage.Position = new Vector2(1, 1);
    }
}
