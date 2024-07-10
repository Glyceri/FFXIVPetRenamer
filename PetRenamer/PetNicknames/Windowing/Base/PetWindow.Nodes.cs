using Dalamud.Interface;
using PetRenamer.PetNicknames.Windowing.Base.Style;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Base;

internal abstract partial class PetWindow
{
    readonly Node _windowNode = new Node()
    {
        Stylesheet = WindowStyles.WindowStylesheet,
        ClassList = ["window"],
        ChildNodes = [
            new Node()
            {
                Id = "Titlebar",
                ClassList = ["window--titlebar"],
                ChildNodes = [
                    new Node()
                    {
                        Id = "TitleLeftAnchor",
                        Style = new() { Anchor = Anchor.TopLeft },
                    },
                    new Node()
                    {
                        Id = "TitleMiddleAnchor",
                        Style = new() { Anchor = Anchor.MiddleCenter },
                        ChildNodes = [
                            new Node()
                            {
                                Id = "TitleText",
                                ClassList = ["window--titlebar-text"],
                                NodeValue = "Untitled Window",
                            },
                        ]
                    },
                    new Node()
                    {
                        Id = "TitleRightAnchor",
                        Style = new() { Anchor = Anchor.TopRight },
                        ChildNodes = [
                            new Node()
                            {
                                Id = "CloseButton",
                                ClassList = ["window--close-button"],
                                NodeValue = FontAwesomeIcon.Times.ToIconString(),
                            },
                            new Node()
                            {
                                Id = "CloseButton2",
                                ClassList = ["window--close-button"],
                                NodeValue = FontAwesomeIcon.Times.ToIconString(),
                            },
                        ]
                    },
                ]
            },
            new Node()
            {
                Id = "Content",
                ClassList = ["window--content"],
            }
        ]
    };

}
