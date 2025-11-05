using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using PetRenamer.PetNicknames.Services.Interface;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes.StylizedButton;

internal abstract class StylizedListButton : NineGridButtonNode
{
    protected readonly IPetServices PetServices;

    protected readonly TextNode TextNode;

    private Vector2 disabledTextureCoordinates = Vector2.Zero;
    private Vector2 enabledTextureCoordinates  = Vector2.Zero;

    public StylizedListButton(IPetServices petServices)
        : base(petServices)
    {
        PetServices        = petServices;

        IsVisible          = true;

        TextNode = new TextNode
        {
            IsVisible     = true,
            AlignmentType = AlignmentType.Center,
        };

        petServices.NativeController.AttachNode(TextNode, this);
    }

    public void Click()
    {
        OnClick?.Invoke();
    }

    public required SeString LabelText
    {
        get => TextNode.SeString;
        set => TextNode.SeString = value;
    }

    public Vector2 DisabledTextureCoordinates
    {
        get => disabledTextureCoordinates;
        set => disabledTextureCoordinates = value;
    }

    public Vector2 EnabledTextureCoordinates
    {
        get => enabledTextureCoordinates;
        set => enabledTextureCoordinates = value;
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        TextNode.Size = new Vector2(Width, Height);

        UpdateButtonSelector();
    }

    protected void UpdateButtonSelector()
    {
        if (IsChecked)
        {
            TextureCoordinates = enabledTextureCoordinates;
        }
        else
        {
            TextureCoordinates = disabledTextureCoordinates;
        }
    }

    public bool IsSelected
    {
        get => IsChecked;
        set
        {
            if (IsChecked == value)
            {
                return;
            }

            IsChecked = value;

            UpdateButtonSelector();
        }
    }
}
