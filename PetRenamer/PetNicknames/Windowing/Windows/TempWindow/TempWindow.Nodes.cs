using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Windows.TempWindow;

internal partial class TempWindow
{
    protected sealed override Node Node { get; } = new()
    {
        Stylesheet = PetRenameStyleSheet,
        Id = "PetRenameWindow2",         
        ChildNodes = [

        ]
    };
}
