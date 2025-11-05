using Dalamud.Game.Text;
using PetRenamer.PetNicknames.KTKWindowing.Nodes.StylizedButton;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes;

internal class QuickButton<T> : KTKComponent where T : KTKAddon
{
    public readonly HighlightableLightStylizedButton Button;

    private Func<bool> shouldBeVisible = () => true;

    private SeIconChar labelText;

    public QuickButton(KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, PettableDirtyHandler dirtyHandler) 
        : base(windowHandler, dalamudServices, petServices, dirtyHandler)
    {
        IsVisible         = true;

        Button            = new HighlightableLightStylizedButton(PetServices)
        {
            LabelText     = string.Empty,
            IsVisible     = true,
            IsSelected    = WindowHandler.IsOpen<T>(),
            OnClick       = () =>
            {
                if (PetServices.Configuration.quickButtonsToggle )
                {
                    WindowHandler.Toggle<T>();
                }
                else
                {
                    if (!WindowHandler.IsOpen<T>())
                    {
                        WindowHandler.Open<T>();
                    }
                }

                OnDirty();
            }
        };

        Button.CollisionNode.Tooltip = windowHandler.GetAddon<T>()?.WindowTooltip ?? "If you see this something is cooked ...";

        AttachNode(ref Button);
    }

    public void Click()
        => Button.OnClick?.Invoke();

    public required SeIconChar LabelText
    {
        get => labelText;
        set => Button.LabelText = (labelText = value).ToIconString();
    }

    public required Func<bool> ShouldBeVisible
    { 
        get => shouldBeVisible;
        set => shouldBeVisible = value;
    }

    public bool IsActive
        => IsVisible = Button.IsVisible = shouldBeVisible();

    protected override void OnDirty()
        => Button.IsSelected = WindowHandler.IsOpen<T>();

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        Button.Size = Size;
    }
}
