using System.Numerics;

namespace PetRenamer.Theming;

[System.Serializable]
public abstract class Theme
{
    public abstract Vector4 imageReplacementColour { get; set; }
    public abstract Vector4 titleBgActive { get; set; }
    public abstract Vector4 tileBgCollapsed { get; set; }

    public abstract Vector4 defaultText { get; set; }
    public abstract Vector4 alternativeText { get; set; }
    public abstract Vector4 tooltipText { get; set; }

    public abstract Vector4 basicLabelColour { get; set; }
    public abstract Vector4 ipcLabelColour { get; set; }

    public abstract Vector4 panelColour { get; set; }

    public abstract Vector4 buttonHovered { get; set; }
    public abstract Vector4 buttonPressed { get; set; }
    public abstract Vector4 button { get; set; }

    public abstract Vector4 buttonAlternativeHovered { get; set; }
    public abstract Vector4 buttonAlternativePressed { get; set; }
    public abstract Vector4 buttonAlternative { get; set; }
}
