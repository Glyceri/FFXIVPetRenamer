using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Windows.TempWindow;

internal partial class TempWindow
{
    protected sealed override Node Node { get; } = new()
    {
        Stylesheet = PetRenameStyleSheet,
        Id = "PetRenameNode",
        Style = new Style()
        {
            Flow = Flow.Horizontal,
            Padding = new(3, 1, 0, 1)
        },
        ChildNodes = [
            
        ]
    };
}
