using Dalamud.Interface;
using ImGuiNET;
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
    readonly QuickSquareButton IPCIndicatorNode;

    readonly RenameTitleNode SpeciesNode;
    readonly RenameTitleNode IDNode;
    readonly RenameTitleNode NicknameNode;

    readonly Node HolderNode;

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
            HolderNode = new Node()
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
                    EyeButtonNode = new QuickSquareButton(),
                    IPCIndicatorNode = new QuickSquareButton()
                    {
                        NodeValue = FontAwesomeIcon.Exclamation.ToIconString(),
                    }
                ]
            }
        ];

        IconNode.SetUser(entry);
        IconNode.RedownloadNode.Style.FontSize = 8;
        IconNode.RedownloadNode.Style.Size = new Size(24, 24);

        if (asLocal)
        {
            ClearButtonNode.Locked = true;
            ClearButtonNode.Tooltip = "You cannot clear when online";
        }

        if (!entry.IsIPC && !entry.IsLegacy)
        {
            HolderNode.RemoveChild(IPCIndicatorNode);
        }

        if (entry.IsIPC)
        {
            IPCIndicatorNode.Tooltip = "This users is temporarily added via an external plugin and will not be saved.";
        }

        if (entry.IsLegacy)
        {
            IPCIndicatorNode.Tooltip = "This users is from your old savefile. Meet them in game so it will update.";
        }

        IPCIndicatorNode.TagsList.Add("fakeDisabled");

        ClearButtonNode.OnClick += () => DalamudServices.Framework.Run(() => ActiveEntry.Clear());
        EyeButtonNode.OnClick += () => DalamudServices.Framework.Run(() => OnView?.Invoke(ActiveEntry));
    }
}
