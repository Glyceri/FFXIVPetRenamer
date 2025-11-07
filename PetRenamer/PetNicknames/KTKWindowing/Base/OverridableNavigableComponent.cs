using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.KTKWindowing.Base;

internal unsafe class OverridableNavigableComponent : KTKResNode
{
    private CollisionNode? _overridenNode = null;

    public OverridableNavigableComponent(KTKAddon parentAddon, KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, PettableDirtyHandler dirtyHandler) 
        : base(parentAddon, windowHandler, dalamudServices, petServices, dirtyHandler) { }

    /*
    public bool UpStopFlag
    {
        get => DrawFlags.HasFlag(DrawFlags.DisableRapidUp);
        set
        {
            if (value)
            {
                DrawFlags |= DrawFlags.DisableRapidUp;
            }
            else
            {
                DrawFlags &= ~DrawFlags.DisableRapidUp;
            }
        }
    }

    public bool DownStopFlag
    {
        get => DrawFlags.HasFlag(DrawFlags.DisableRapidDown);
        set
        {
            if (value)
            {
                DrawFlags |= DrawFlags.DisableRapidDown;
            }
            else
            {
                DrawFlags &= ~DrawFlags.DisableRapidDown;
            }
        }
    }

    public bool LeftStopFlag
    {
        get => DrawFlags.HasFlag(DrawFlags.DisableRapidLeft);
        set
        {
            if (value)
            {
                DrawFlags |= DrawFlags.DisableRapidLeft;
            }
            else
            {
                DrawFlags &= ~DrawFlags.DisableRapidLeft;
            }
        }
    }

    public bool RightStopFlag
    {
        get => DrawFlags.HasFlag(DrawFlags.DisableRapidRight);
        set
        {
            if (value)
            {
                DrawFlags |= DrawFlags.DisableRapidRight;
            }
            else
            {
                DrawFlags &= ~DrawFlags.DisableRapidRight;
            }
        }
    }
    */
}
