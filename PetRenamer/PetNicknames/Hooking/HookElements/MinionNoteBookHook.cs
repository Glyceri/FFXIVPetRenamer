using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class MinionNoteBookHook : HookableElement
{
    private static readonly NameTypeFactory NoteBookNameType = new NameTypeFactory()
    {
        GermanNameType = NameType.Pronoun,
    };
    
    public MinionNoteBookHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices) { }

    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostReceiveEvent, "MinionNoteBook",      HandlePostRefreshNoteBook);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh,      "MJIMinionNoteBook",   HandlePostRefreshMJINoteBook);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PreDraw,          "LovmPaletteEdit",     HandlePostRefreshLovmPaletteEdit);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PreDraw,          "LovmActionDetail",    HandlePostRefreshLovmActionDetail);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostReceiveEvent, "YKWNote",             HandlePostRefreshYKWNote);
    }
    
    protected override void OnDispose()
    {
        DalamudServices.AddonLifecycle.UnregisterListener(HandlePostRefreshNoteBook);
        DalamudServices.AddonLifecycle.UnregisterListener(HandlePostRefreshMJINoteBook);
        DalamudServices.AddonLifecycle.UnregisterListener(HandlePostRefreshLovmPaletteEdit);
        DalamudServices.AddonLifecycle.UnregisterListener(HandlePostRefreshLovmActionDetail);
        DalamudServices.AddonLifecycle.UnregisterListener(HandlePostRefreshYKWNote);
    }
    
    private void HandleBook(AtkUnitBase* atkUnitBase, uint textNodeIndex)
    {
        if (atkUnitBase == null)
        {
            return;
        }
        
        if (PetServices.UserList.LocalPlayer == null)
        {
            return;
        }
        
        AtkTextNode* textNode = atkUnitBase->GetTextNodeById(textNodeIndex);
        
        if (textNode == null)
        {
            return;
        }
        
        IPetSheetData? petData = PetServices.PetSheets.GetPetFromName(textNode->NodeText.ExtractText());
        
        PetServices.StringHelper.ReplaceAtkString(PetServices.Configuration.ShowNamesInMinionBookColour, textNode, petData, NoteBookNameType.GetValue(DalamudServices));
    }
    
    private void HandlePostRefreshNoteBook(AddonEvent addonEvent, AddonArgs args)
        => HandleBook((AtkUnitBase*)args.Addon.Address, 67);

    private void HandlePostRefreshMJINoteBook(AddonEvent addonEvent, AddonArgs args) 
        => HandleBook((AtkUnitBase*)args.Addon.Address, 65);
    
    private void HandlePostRefreshLovmPaletteEdit(AddonEvent addonEvent, AddonArgs args)
        => HandleBook((AtkUnitBase*)args.Addon.Address, 48);
    
    private void HandlePostRefreshLovmActionDetail(AddonEvent addonEvent, AddonArgs args)
        => HandleBook((AtkUnitBase*)args.Addon.Address, 4);
    
    private void HandlePostRefreshYKWNote(AddonEvent addonEvent, AddonArgs args)
        => HandleBook((AtkUnitBase*)args.Addon.Address, 28);
}