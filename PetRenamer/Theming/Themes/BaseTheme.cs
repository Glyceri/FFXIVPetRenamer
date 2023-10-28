using System.Numerics;

namespace PetRenamer.Theming.Themes;

public class BaseTheme : Theme 
{
    public override Vector4 imageReplacementColour { get; set; } = new Vector4(0.60f, 0.70f, 0.80f, 0.95f);
    public override Vector4 titleBgActive { get; set; } = new Vector4(0.30f, 0.50f, 1f, 1f);
    public override Vector4 panelColour { get; set; } = new Vector4(0.2f, 0.3f, 0.4f, 1f);
    public override Vector4 tileBgCollapsed { get; set; } = new Vector4(0.1f, 0.1f, 0.1f, 1f);

    public override Vector4 defaultText { get; set; } = new Vector4(0.95f, 0.95f, 0.95f, 1f);
    public override Vector4 alternativeText { get; set; } = new Vector4(0.95f, 0.95f, 0.95f, 1f);
    public override Vector4 tooltipText { get; set; } = new Vector4(0.95f, 0.95f, 0.95f, 1f);

    public override Vector4 basicLabelColour { get; set; } = new Vector4(0.4f, 0.4f, 0.5f, 1f);
    public override Vector4 ipcLabelColour { get; set; } = new Vector4(0.3f, 0.4f, 0.5f, 1f);

    public override Vector4 buttonHovered { get; set; } = new Vector4(0.5f, 0.5f, 1f, 1f);
    public override Vector4 buttonPressed { get; set; } = new Vector4(0.36f, 0.36f, 1f, 1f);
    public override Vector4 button { get; set; } = new Vector4(0.3f, 0.3f, 1f, 1f);

    public override Vector4 buttonAlternativeHovered { get; set; } = new Vector4(0.5f, 0.5f, 1f, 1f);
    public override Vector4 buttonAlternativePressed { get; set; } = new Vector4(0.36f, 0.36f, 1f, 1f);
    public override Vector4 buttonAlternative { get; set; } = new Vector4(0.3f, 0.3f, 0.8f, 1f);
    
}
