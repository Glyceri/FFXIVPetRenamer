using ImGuiNET;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using System;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class PetRenameNode : Node
{
    readonly IconNode IconNode;
   // readonly CircleImageNode CircleImageNode;

    readonly IPetSheetData ActivePet;

    const int Margin = 3;

    string? CurrentValue = null;

    public Action<string?>? OnSave;

    readonly DalamudServices DalamudServices;

    public PetRenameNode(string? customName, in IPetSheetData activePet, in DalamudServices services)
    {
        DalamudServices = services;
        CurrentValue = customName;
        ActivePet = activePet;

        ChildNodes = [
            // Rename Node part
            new Node()
            {
                Style = new Style()
                {
                    Size = new Size (100, 20),
                    BorderColor = new BorderColor(new Color(255, 255, 255)),
                    BorderWidth = new(0, 0, 2, 0),
                    ShadowSize = new EdgeSize(0, 0, 64, 0),
                   
                },
                NodeValue = "Hedgehoglet",
            },

            // This is the node that holds the image together
            new Node()
            {
                ChildNodes = [
                    IconNode = new IconNode(in services, activePet.Icon)
                    {
                        Style = new Style()
                        {
                            Size = new Size(120, 120)
                        }
                    }
                ]
            }
            
        ];
    }

    public void Setup(string? customName, in IPetSheetData activePet)
    {
        IconNode.IconID = activePet.Icon;
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        base.OnDraw(drawList);
    }
}
