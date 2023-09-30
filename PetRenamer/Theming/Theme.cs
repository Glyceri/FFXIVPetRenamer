using System.Numerics;

namespace PetRenamer.Theming;

internal abstract class Theme
{
    internal abstract Vector4 defaultBackground { get; set; }
    internal abstract Vector4 titleBgActive { get; set; }
    internal abstract Vector4 panelColour { get; set; }
    internal abstract Vector4 tileBgCollapsed { get; set; }

    internal abstract Vector4 defaultText { get; set; }
    internal abstract Vector4 alternativeText { get; set; }

    internal abstract Vector4 basicLabelColour { get; set; }

    internal abstract Vector4 buttonHovered { get; set; }
    internal abstract Vector4 buttonPressed { get; set; }
    internal abstract Vector4 button { get; set; }

    internal abstract Vector4 buttonAlternativeHovered { get; set; }
    internal abstract Vector4 buttonAlternativePressed { get; set; }
    internal abstract Vector4 buttonAlternative { get; set; }
}
