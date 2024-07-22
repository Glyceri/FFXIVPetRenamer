using Una.Drawing;
using System;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using PetRenamer.PetNicknames.ColourProfiling.Interfaces;
using Dalamud.Interface;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Settings;

internal class ColourProfileConfig : ToggleConfig
{
    public readonly RenameTitleNode AuthorNode;

    public readonly QuickClearButton ClearButton;
    public readonly QuickSquareButton ExportButton;

    readonly IColourProfile ColourProfile;

    public ColourProfileConfig(in Configuration configuration, in DalamudServices dalamudServices, IColourProfile cProfile, int index, bool active, Action<int> callback, Action export) : base(configuration, cProfile.Name, active, (value) => callback?.Invoke(index))
    {
        ColourProfile = cProfile;

        Style = new Style()
        {
            Flow = Flow.Vertical,
            StrokeWidth = 1,
            StrokeColor = new Color("Outline"),
        };

       
        HolderNode.ChildNodes.Add(ExportButton = new QuickSquareButton()
        {
            NodeValue = FontAwesomeIcon.FileExport.ToIconString(),
            Style = new Style()
            {
              Size = new Size(35, 15),  
            },
        });
        HolderNode.ChildNodes.Add(ClearButton = new QuickClearButton());
        ChildNodes.Add(AuthorNode = new RenameTitleNode(in dalamudServices, "Author", cProfile.Author));
    }
}
