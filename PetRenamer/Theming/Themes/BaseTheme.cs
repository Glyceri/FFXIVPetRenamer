using System.Numerics;

namespace PetRenamer.Theming.Themes;

internal class BaseTheme : Theme 
{
    internal override Vector4 defaultBackground { get; set; } = new Vector4(0.60f, 0.70f, 0.80f, 0.95f);
    internal override Vector4 titleBgActive { get; set; } = new Vector4(0.30f, 0.50f, 1f, 1f);
    internal override Vector4 titleBg { get; set; } = new Vector4(0.2f, 0.3f, 0.4f, 1f);
    internal override Vector4 tileBgCollapsed { get; set; } = new Vector4(0.1f, 0.1f, 0.1f, 1f);

    internal override Vector4 whiteText { get; set; } = new Vector4(0.95f, 0.95f, 0.95f, 1f);
    internal override Vector4 defaultText { get; set; } = new Vector4(0.95f, 0.95f, 0.95f, 1f);
    internal override Vector4 errorText { get; set; } = new Vector4(1, 0, 0, 1.0f);
    internal override Vector4 highlightedText { get; set; } = new Vector4(0.6f, 0.6f, 1f, 1f);
    internal override Vector4 readableBlueText { get; set; } = new Vector4(0.8f, 0.8f, 1f, 1f);

    internal override Vector4 idleColor { get; set; } = new Vector4(0.4f, 0.4f, 0.5f, 1f);

    internal override Vector4 buttonHovered { get; set; } = new Vector4(0.5f, 0.5f, 1f, 1f);
    internal override Vector4 buttonPressed { get; set; } = new Vector4(0.36f, 0.36f, 1f, 1f);
    internal override Vector4 button { get; set; } = new Vector4(0.3f, 0.3f, 1f, 1f);

    internal override Vector4 textFieldHovered { get; set; } = new Vector4(0.5f, 0.5f, 1f, 1f);
    internal override Vector4 textFieldPressed { get; set; } = new Vector4(0.36f, 0.36f, 1f, 1f);
    internal override Vector4 textField { get; set; } = new Vector4(0.3f, 0.3f, 0.8f, 1f);

    internal override Vector4 xButtonHovered { get; set; } = new Vector4(0.5f, 0.5f, 0.8f, 1f);
    internal override Vector4 xButtonPressed { get; set; } = new Vector4(0.36f, 0.36f, 0.8f, 1f);
    internal override Vector4 xButton { get; set; } = new Vector4(0.3f, 0.3f, 0.8f, 1f);

    internal override Vector4 listBox { get; set; } = new Vector4(0.26f, 0.26f, 0.33f, 1f);
    internal override Vector4 scrollBarBG { get; set; } = new Vector4(0.29f, 0.29f, 0.36f, 1f);
}
