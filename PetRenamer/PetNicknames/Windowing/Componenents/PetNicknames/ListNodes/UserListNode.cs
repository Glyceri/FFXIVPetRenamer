using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Images;
using System;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.ListNodes;

internal class UserListNode : Node
{
    readonly ProfilePictureNode IconNode;
    readonly QuickClearButton ClearButtonNode;
    readonly QuickSquareButton EyeButtonNode;

    readonly RenameTitleNode SpeciesNode;
    readonly RenameTitleNode IDNode;
    readonly RenameTitleNode NicknameNode;

    readonly DalamudServices DalamudServices;

    readonly IPettableDatabaseEntry ActiveEntry;

    public Action<IPettableDatabaseEntry>? OnView;

    public UserListNode(in DalamudServices services, in IImageDatabase imageDatabase, in IPettableDatabaseEntry entry, bool asLocal)
    {
        DalamudServices = services;
        ActiveEntry = entry;
        Style = new Style()
        {
            Flow = Flow.Horizontal,
            BackgroundColor = new Color(26, 20, 0, 130),
            Size = new Size(412, 70),
            RoundedCorners = RoundedCorners.All,
            BorderRadius = 8,
            StrokeRadius = 8,
            BorderColor = new(new("Window.TitlebarBorder")),
            BorderWidth = new EdgeSize(1),
            IsAntialiased = false,
        };

        ChildNodes = [
            new Node()
            {
                Style = new Style()
                {
                    Flow = Flow.Vertical,
                    Margin = new EdgeSize(11, 0, 0, 8),
                },
                ChildNodes = [
                    SpeciesNode = new RenameTitleNode(in DalamudServices, "Name:", entry.Name),
                    IDNode = new RenameTitleNode(in DalamudServices, "Homeworld:", entry.HomeworldName),
                    NicknameNode = new RenameTitleNode(in DalamudServices, "Petcount:", entry.ActiveDatabase.Length.ToString()),
                ]
            },
            IconNode = new ProfilePictureNode(in DalamudServices, in imageDatabase)
            {
                Style = new Style()
                {
                    Size = new Size(50, 50),
                    Margin = new EdgeSize(10, 0, 0, 15),
                }
            },
            new Node()
            {
                Style = new Style()
                {
                    Flow = Flow.Vertical,
                    Anchor = Anchor.TopRight,
                    Padding = new EdgeSize(10, 10),
                    Gap = 4,
                },
                ChildNodes = [
                    ClearButtonNode = new QuickClearButton(),
                    EyeButtonNode = new QuickSquareButton()
                ]
            }
        ];

        IconNode.SetUser(entry);
        IconNode.RedownloadNode.Style.FontSize = 8;
        IconNode.RedownloadNode.Style.Size = new Size(24, 24);

        ClearButtonNode.OnClick += () => DalamudServices.Framework.Run(() => ActiveEntry.Clear());
        EyeButtonNode.OnClick += () => DalamudServices.Framework.Run(() => OnView?.Invoke(ActiveEntry));
    }
}
