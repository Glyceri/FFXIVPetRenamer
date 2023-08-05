using System.Numerics;

namespace PetRenamer.Theming;

internal abstract class Theme
{
    internal abstract Vector4 defaultBackground { get; set; }
    internal abstract Vector4 titleBgActive { get; set; }
    internal abstract Vector4 titleBg { get; set; }
    internal abstract Vector4 tileBgCollapsed { get; set; }

    internal abstract Vector4 whiteText { get; set; }
    internal abstract Vector4 defaultText { get; set; }
    internal abstract Vector4 errorText { get; set; }
    internal abstract Vector4 highlightedText { get; set; }
    internal abstract Vector4 readableBlueText { get; set; }

    internal abstract Vector4 idleColor { get; set; }

    internal abstract Vector4 buttonHovered { get; set; }
    internal abstract Vector4 buttonPressed { get; set; }
    internal abstract Vector4 button { get; set; }

    internal abstract Vector4 textFieldHovered { get; set; }
    internal abstract Vector4 textFieldPressed { get; set; }
    internal abstract Vector4 textField { get; set; }

    internal abstract Vector4 xButtonHovered { get; set; }
    internal abstract Vector4 xButtonPressed { get; set; }
    internal abstract Vector4 xButton { get; set; }

    internal abstract Vector4 listBox { get; set; }
    internal abstract Vector4 scrollBarBG { get; set; }
}
