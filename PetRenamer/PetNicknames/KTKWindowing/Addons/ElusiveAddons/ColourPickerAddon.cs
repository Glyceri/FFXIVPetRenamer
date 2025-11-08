using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.Enum;
using PetRenamer.PetNicknames.KTKWindowing.Base;
using PetRenamer.PetNicknames.KTKWindowing.Interfaces;
using PetRenamer.PetNicknames.KTKWindowing.Nodes.FunctionalNodes;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Addons.ElusiveAddons;

internal class ColourPickerAddon : KTKAddon, ICustomInput
{
    private ColourBar? ColourBarR;
    private ColourBar? ColourBarG;
    private ColourBar? ColourBarB;

    [SetsRequiredMembers]
    public ColourPickerAddon(KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDatabase database, PettableDirtyHandler dirtyHandler)
        : base(windowHandler, dalamudServices, petServices, userList, database, dirtyHandler) { }

    public override string WindowName
        => "Colour Picker";

    protected override Vector2 WindowSize
        => new Vector2(300, 300);

    protected override bool HasPetBar
        => false;

    protected override unsafe void OnAddonSetup(AtkUnitBase* addon)
    {
        ColourBarR   = new ColourBar(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size     = new Vector2(200, 30),
            Position = new Vector2(0, 0),
            Colour   = new Vector3(1, 0, 0),
        };

        AttachNode(ref ColourBarR);

        ColourBarG   = new ColourBar(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size     = new Vector2(200, 30),
            Position = new Vector2(0, 25),
            Colour   = new Vector3(0, 1, 0),
        };

        AttachNode(ref ColourBarG);

        ColourBarB   = new ColourBar(this, WindowHandler, DalamudServices, PetServices, DirtyHandler)
        {
            Size     = new Vector2(200, 30),
            Position = new Vector2(0, 50),
            Colour   = new Vector3(0, 0, 1),
        };

        AttachNode(ref ColourBarB);

        ColourBarR.OnSelected   += () => OnSelected(ColourBarR);
        ColourBarR.OnUnselected += () => OnUnselected(ColourBarR);

        ColourBarG.OnSelected   += () => OnSelected(ColourBarG);
        ColourBarG.OnUnselected += () => OnUnselected(ColourBarG);

        ColourBarB.OnSelected   += () => OnSelected(ColourBarB);
        ColourBarB.OnUnselected += () => OnUnselected(ColourBarB);

        TransientGuideHandler?.RegisterGuide(ColourBarR.GuideRegistration);
        TransientGuideHandler?.RegisterGuide(ColourBarG.GuideRegistration);
        TransientGuideHandler?.RegisterGuide(ColourBarB.GuideRegistration);

        TransientGuideHandler?.SetGuide(ColourBarR.GuideRegistration);
    }

    protected override unsafe void OnAddonUpdate(AtkUnitBase* addon)
    {
        bool requestRefresh = false;

        if (ColourBarR != null)
        {
            ColourBarR.OnAddonUpdate(addon);
            requestRefresh |= ColourBarR.RequestRefresh;
        }

        if (ColourBarG != null)
        {
            ColourBarG.OnAddonUpdate(addon);
            requestRefresh |= ColourBarG.RequestRefresh;
        }

        if (ColourBarB != null)
        {
            ColourBarB.OnAddonUpdate(addon);
            requestRefresh |= ColourBarB.RequestRefresh;
        }

        if (requestRefresh)
        {
            TransientGuideHandler?.RefreshOperationsGuide();
        }
    }

    private void OnSelected(ColourBar colourBar)
    {
        ColourBarR?.Unselect();
        ColourBarG?.Unselect();
        ColourBarB?.Unselect();

        TransientGuideHandler?.SetGuide(colourBar.GuideRegistration, false);
        colourBar.Select();
    }

    private void OnUnselected(ColourBar colourBar)
    {
        colourBar.Unselect();
    }

    public bool OnCustomGuideInput(NavigationInputId inputId, AtkEventData.AtkInputData.InputState inputState)
    {


        return false;
    }

    public override bool OnCustomInput(NavigationInputId inputId, AtkEventData.AtkInputData.InputState inputState)
    {
        if (inputState != AtkEventData.AtkInputData.InputState.Down)
        {
            return false;
        }

        if (inputId == NavigationInputId.Down)
        {
            if (TransientGuideHandler == null)
            {
                return false;
            }

            TransientGuideHandler.SelectNextGuide();

            return true;
        }

        if (inputId == NavigationInputId.Up)
        {
            if (TransientGuideHandler == null)
            {
                return false;
            }

            TransientGuideHandler.SelectPreviousGuide();

            return true;
        }

        return false;
    }

    protected override unsafe void OnAddonFinalize(AtkUnitBase* addon)
    {
        if (ColourBarR != null)
        {
            TransientGuideHandler?.DeregisterGuide(ColourBarR.GuideRegistration);
            ColourBarR = null;
        }

        if (ColourBarG != null)
        {
            TransientGuideHandler?.DeregisterGuide(ColourBarG.GuideRegistration);
            ColourBarG = null;
        }

        if (ColourBarB != null)
        {
            TransientGuideHandler?.DeregisterGuide(ColourBarB.GuideRegistration);
            ColourBarB = null;
        }
    }
}
