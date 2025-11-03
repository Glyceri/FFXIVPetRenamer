using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Utility;
using KamiToolKit.Nodes;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes;

internal class PetRenameNode : KTKComponent
{
    private const    string        PlaceholderString = "Nickname . . .";

    private readonly TextInputNode TextInputNode;

    public PetRenameNode(KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, PettableDirtyHandler dirtyHandler) 
        : base(windowHandler, dalamudServices, petServices, dirtyHandler)
    {
        TextInputNode         = new TextInputNode
        {
            PlaceholderString = PlaceholderString,
            IsVisible         = true,
        };

        AttachNode(ref TextInputNode);
    }

    public required Action<SeString>? OnNameComplete
    {
        get => TextInputNode.OnInputComplete;
        set => TextInputNode.OnInputComplete = value;
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        TextInputNode.Size = new Vector2(Width - 5.0f, Height);
        TextInputNode.Position = new Vector2(0.0f, 0.0f);
    }

    public SeString NicknameString
    {
        get => TextInputNode.SeString;
        set
        {
            TextInputNode.SeString = value;

            // Fixes a bug where setting the string without focus doesnt clear the placeholder string
            if (value.TextValue.IsNullOrWhitespace())
            {
                TextInputNode.PlaceholderString = PlaceholderString;
            }
            else
            {
                TextInputNode.PlaceholderString = string.Empty;
            }
        }
    }
}
