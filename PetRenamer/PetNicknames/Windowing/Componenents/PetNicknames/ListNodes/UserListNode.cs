﻿using Dalamud.Interface;
using ImGuiNET;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Images;
using System;
using System.Numerics;
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
    readonly Configuration Configuration;

    readonly IPettableDatabaseEntry ActiveEntry;

    public Action<IPettableDatabaseEntry>? OnView;

    public UserListNode(in DalamudServices services, in Configuration configuration, in IImageDatabase imageDatabase, in IPettableDatabaseEntry entry, bool asLocal)
    {
        Configuration = configuration;
        DalamudServices = services;

        ActiveEntry = entry;
        Style = new Style()
        {
            Flow = Flow.Horizontal,
            BackgroundColor = new Color("ListElementBackground"),
            Size = new Size(412, 60),
            BorderColor = new(new("Outline")),
            BorderWidth = new EdgeSize(1),
            IsAntialiased = false,
        };

        ChildNodes = [
            new Node()
            {
                Style = new Style()
                {
                    Flow = Flow.Vertical,
                    Margin = new EdgeSize(5, 0, 0, 8),
                },
                ChildNodes = [
                    SpeciesNode = new RenameTitleNode(in DalamudServices, Translator.GetLine("Name") + ":", entry.Name),
                    IDNode = new RenameTitleNode(in DalamudServices, Translator.GetLine("Homeworld") + ":", entry.HomeworldName),
                    NicknameNode = new RenameTitleNode(in DalamudServices, Translator.GetLine("Petcount") + ":", entry.ActiveDatabase.Length.ToString()),
                ]
            },
            new Node()
            {
                Style = new Style()
                {
                    Size = new Size(50, 50),
                    Margin = new EdgeSize(5, 0, 0, 20),
                    BorderColor = new BorderColor(new Color("Outline")),
                    BorderWidth = new EdgeSize(4),
                },
                ChildNodes = [
                    IconNode = new ProfilePictureNode(in DalamudServices, in imageDatabase)
                    {
                        Style = new Style()
                        {
                            Size = new Size(46, 46),
                            Anchor = Anchor.MiddleCenter,
                        }
                    },
                ]
            },
            HolderNode = new Node()
            {
                Style = new Style()
                {
                    Flow = Flow.Vertical,
                    Anchor = Anchor.TopRight,
                    Padding = new EdgeSize(5, 5),
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
            ClearButtonNode.Tooltip = Translator.GetLine("UserListElement.WarningClear");
        }

        if (!entry.IsIPC && !entry.IsLegacy)
        {
            HolderNode.RemoveChild(IPCIndicatorNode, true);
        }

        if (entry.IsIPC)
        {
            IPCIndicatorNode.Tooltip = Translator.GetLine("UserListElement.WarningIPC");
        }

        if (entry.IsLegacy)
        {
            IPCIndicatorNode.Tooltip = Translator.GetLine("UserListElement.WarningOldUser");
        }

        IPCIndicatorNode.TagsList.Add("fakeDisabled");

        ClearButtonNode.OnClick += () => DalamudServices.Framework.Run(() => ActiveEntry.Clear(false));
        EyeButtonNode.OnClick += () => DalamudServices.Framework.Run(() => OnView?.Invoke(ActiveEntry));
    }


    protected override void OnDraw(ImDrawListPtr drawList)
    {
        if (!Configuration.uiFlare) return;

        Rect activeRect = SpeciesNode.UnderlineNode.Bounds.ContentRect;
        Rect iconRect = IconNode.Bounds.ContentRect;

        Vector2 activePos = activeRect.TopRight + (activeRect.BottomRight - activeRect.TopRight) * 0.5f - new Vector2(ScaleFactor, 0);
        Vector2 iconPos = iconRect.TopLeft + (iconRect.BottomLeft - iconRect.TopLeft) * 0.5f;
        Vector2 earlyiconPos = iconPos - new Vector2(12, 0) * ScaleFactor;

        drawList.AddLine(activePos, earlyiconPos + new Vector2(ScaleFactor * 0.5f, 0), new Color("Outline").ToUInt(), 2 * ScaleFactor);
        drawList.AddLine(earlyiconPos, iconPos, new Color("Outline").ToUInt(), 2 * ScaleFactor);
    }
}
