using Dalamud.Game.Gui.ContextMenu;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.ContextMenus.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;
using System;
using XBMPet = PetRenamer.PetNicknames.Services.ServiceWrappers.Sheets.XBMPetButActuallyWorkingSinceForSomeReasonWePutInUnsusedSheetsAndNowEverythingIsABreakingChangeBecauseWhyWouldntItBeLikeWhatAreWeGenuinelyDoingHere;

namespace PetRenamer.PetNicknames.ContextMenus.ContextMenuElements;

internal unsafe class XBMMonsterNoteBookContextMenu : IContextMenuElement
{
    private readonly IPetServices   PetServices;
    private readonly IWindowHandler WindowHandler;
    
    public XBMMonsterNoteBookContextMenu(IPetServices petServices, IWindowHandler windowHandler)
    {
        PetServices   = petServices;
        WindowHandler = windowHandler;
    }
    
    public string? AddonName
        => "XBMMonsterNotebook";
    
    public Action<IMenuItemClickedArgs>? OnOpenMenu(IMenuOpenedArgs args) 
        => HandleMenu;
    
    private void HandleMenu(IMenuItemClickedArgs args)
    {
        IPettableUser? localUser = PetServices.UserList.LocalPlayer;

        if (localUser == null)
        {
            return;
        }
        
        AtkUnitBase* addonXBMMonsterNoteBook = (AtkUnitBase*)args.AddonPtr;
            
        if (addonXBMMonsterNoteBook == null)
        {
            return;
        }
            
        Span<AtkValue> atkValues = addonXBMMonsterNoteBook->AtkValuesSpan;
            
        if (atkValues.Length < 7)
        {
            return;
        }
            
        uint contextMenuIndex = atkValues[6].UInt;
        
        XBMPet? pet = PetServices.PetSheets.GetSheetXBMPet(contextMenuIndex);
        
        if (pet == null)
        {
            return;
        }
        
        IPetSheetData? petSheetData = PetServices.PetSheets.GetPetFromIcon(pet.Value.Icon);
        
        if (petSheetData == null)
        {
            return;
        }
        
        WindowHandler.GetWindow<PetRenameWindow>()?.SetRenameWindow(petSheetData.Model);
    }
}