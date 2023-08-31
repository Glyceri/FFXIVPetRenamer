using System.Numerics;

namespace PetRenamer.Theming.Themes;

internal class RedTheme : BaseTheme
{
    internal override Vector4 titleBgActive { get; set; } = new Vector4(0.8f, 0.30f, 0.30f, 1f);
    internal override Vector4 titleBg { get; set; } = new Vector4(0.60f, 0.20f, 0.2f, 1f);
    internal override Vector4 tileBgCollapsed { get; set; } = new Vector4(0.1f, 0.1f, 0.1f, 1f);

    internal override Vector4 buttonHovered { get; set; } = new Vector4(1f, 0.5f,  0.5f, 1f);
    internal override Vector4 buttonPressed { get; set; } = new Vector4(1f, 0.36f,  0.36f, 1f);
    internal override Vector4 button { get; set; } = new Vector4(1f, 0.3f,  0.3f, 1f);

    internal override Vector4 textFieldHovered { get; set; } = new Vector4(1f, 0.5f,  0.5f, 1f);
    internal override Vector4 textFieldPressed { get; set; } = new Vector4(1f, 0.36f,  0.36f, 1f);
    internal override Vector4 textField { get; set; } = new Vector4(0.8f, 0.3f,  0.3f, 1f);

    internal override Vector4 xButtonHovered { get; set; } = new Vector4(0.8f, 0.5f,  0.5f, 1f);
    internal override Vector4 xButtonPressed { get; set; } = new Vector4(0.8f, 0.36f,  0.36f, 1f);
    internal override Vector4 xButton { get; set; } = new Vector4(0.8f, 0.3f, 0.3f, 1f);

    internal override Vector4 highlightedText { get; set; } = new Vector4(1f, 0.6f, 0.6f, 1f);

    internal override Vector4 idleColor { get; set; } = new Vector4(0.5f, 0.4f, 0.4f, 1f);

    internal override Vector4 listBox { get; set; } = new Vector4(0.36f, 0.26f, 0.26f, 1f);
}
