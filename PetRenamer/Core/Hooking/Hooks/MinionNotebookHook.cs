using Dalamud.Plugin.Services;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
public unsafe class MinionNotebookHook : QuickTextHookableElement
{
    readonly uint[] indexes = new uint[11] { 5, 51001, 51002, 51003, 51004, 51005, 51006, 51007, 51008, 51009, 510010 };

    internal override void OnQuickInit()
    {
        RegisterHook("MinionNoteBook", 67);
        RegisterHook("LovmPaletteEdit", 48);
        RegisterHook("LovmActionDetail", 4);
        foreach(uint index in indexes)
            RegisterHook("MinionNoteBook", new uint[3] { 25, index, 6 });
    }
}
