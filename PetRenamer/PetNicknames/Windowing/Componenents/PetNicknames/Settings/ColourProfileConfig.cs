using Una.Drawing;
using System;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Settings;

internal class ColourProfileConfig : ToggleConfig
{
    public readonly RenameTitleNode AuthorNode;

    public readonly QuickClearButton ClearButton;

    public ColourProfileConfig(in Configuration configuration, in DalamudServices dalamudServices, string label, string author, int index, bool active, Action<int> callback) : base(configuration, label, active, (value) => callback?.Invoke(index))
    {
        Style = new Style()
        {
            Flow = Flow.Vertical,
        };

        HolderNode.ChildNodes.Add(ClearButton = new QuickClearButton());

        ChildNodes.Add(AuthorNode = new RenameTitleNode(in dalamudServices, "Author", author));

    }
}
