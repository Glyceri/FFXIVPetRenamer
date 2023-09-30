using System.Numerics;

namespace PetRenamer.Theming.Themes;

internal class GreenTheme : BaseTheme
{
    internal override Vector4 defaultBackground { get; set; } = new Vector4(0.60f, 0.70f, 0.80f, 0.95f);
    internal override Vector4 titleBgActive { get; set; } = new Vector4(0.30f, 1f, 0.50f, 1f);
    internal override Vector4 panelColour { get; set; } = new Vector4(0.20f, 0.40f, 0.3f, 1f);
    internal override Vector4 tileBgCollapsed { get; set; } = new Vector4(0.1f, 0.1f, 0.1f, 1f);

    internal override Vector4 defaultText { get; set; } = new Vector4(0.95f, 0.95f, 0.95f, 1f);
    internal override Vector4 alternativeText { get; set; } = new Vector4(0.25f, 0.25f, 0.25f, 1f);

    internal override Vector4 basicLabelColour { get; set; } = new Vector4(0.4f, 0.45f, 0.4f, 1f);

    internal override Vector4 buttonHovered { get; set; } = new Vector4(0.5f, 1f, 0.5f, 1f);
    internal override Vector4 buttonPressed { get; set; } = new Vector4(0.36f, 1f, 0.36f, 1f);
    internal override Vector4 button { get; set; } = new Vector4(0.3f, 1f, 0.3f, 1f);

    internal override Vector4 buttonAlternativeHovered { get; set; } = new Vector4(0.5f, 1f, 0.5f, 1f);
    internal override Vector4 buttonAlternativePressed { get; set; } = new Vector4(0.36f, 1f, 0.36f, 1f);
    internal override Vector4 buttonAlternative { get; set; } = new Vector4(0.3f, 0.8f, 0.3f, 1f);
}
