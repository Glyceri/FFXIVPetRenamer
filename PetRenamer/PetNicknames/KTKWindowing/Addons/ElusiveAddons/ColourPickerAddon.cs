using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using PetRenamer.PetNicknames.Services.Interface;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Addons.ElusiveAddons;

internal class ColourPickerAddon : NativeAddon
{
    [SetsRequiredMembers]
    public ColourPickerAddon(IPetServices petServices)
    {
        InternalName     = "PetNicknamesColourPicker";
        Title            = "Colour Picker";
        NativeController = petServices.NativeController;

        Size             = new Vector2(300, 300);
    }

    protected override unsafe void OnSetup(AtkUnitBase* addon)
    {
       
    }

    protected override unsafe void OnFinalize(AtkUnitBase* addon)
    {
        
    }
}
