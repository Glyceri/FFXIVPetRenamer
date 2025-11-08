using FFXIVClientStructs.FFXIV.Component.GUI;

namespace PetRenamer.PetNicknames.KTKWindowing.Interfaces;

internal interface IRequireUpdate
{
    public unsafe void OnAddonUpdate(AtkUnitBase* addon);
}
