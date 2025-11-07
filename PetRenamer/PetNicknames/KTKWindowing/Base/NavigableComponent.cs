using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System.Diagnostics.CodeAnalysis;

namespace PetRenamer.PetNicknames.KTKWindowing.Base;

internal abstract class NavigableComponent : OverridableNavigableComponent
{
    [SetsRequiredMembers]
    protected NavigableComponent(KTKAddon parentAddon, KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, PettableDirtyHandler dirtyHandler) 
        : base(parentAddon, windowHandler, dalamudServices, petServices, dirtyHandler) 
    {
        
    }
}
