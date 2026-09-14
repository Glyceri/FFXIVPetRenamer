using Dalamud.Hooking;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.Text;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;
using System.Collections.Generic;
using System.Reflection;
using DalamudSeString = Dalamud.Game.Text.SeStringHandling.SeString;
using XBMPet = PetRenamer.PetNicknames.Services.ServiceWrappers.Sheets.XBMPetButActuallyWorkingSinceForSomeReasonWePutInUnsusedSheetsAndNowEverythingIsABreakingChangeBecauseWhyWouldntItBeLikeWhatAreWeGenuinelyDoingHere;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class PronounHook : HookableElement, IPronounHook
{
    public DalamudSeString? LastGottenPronoun         { get; private set; }
    public DalamudSeString? PreviousLastGottenPronoun { get; private set; }
    
    private readonly Hook<Localize.Delegates.ProcessNoun> LocalizeProcessNounHook;
    
    private readonly List<(string, Func<IPetSheets, uint, bool>)> AllowedSheetNames = [];
    
    public PronounHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices)
    {
        LocalizeProcessNounHook = DalamudServices.Hooking.HookFromAddress<Localize.Delegates.ProcessNoun>((nint)Localize.MemberFunctionPointers.ProcessNoun, LocalizeProcessNounDetour);
        
        Register<Companion>(HandleValidCompanion);
        Register<BNpcName>(HandleValidPet);
        Register<Pet>(HandleValidPet);
        Register<XBMPet>(HandleValidXBMPet);
        
        PetServices.NameService.RegisterPronounHook(this);
    }

    public override void Init()
    {
        LocalizeProcessNounHook?.Enable();
    }
    
    protected override void OnDispose()
    {
        LocalizeProcessNounHook?.Dispose();
    }
    
    private bool LocalizeProcessNounDetour(Localize* localize, Localize.NounParams* nounParams, Utf8String* outString)
    {
        bool returner = LocalizeProcessNounHook!.OriginalDisposeSafe(localize, nounParams, outString);
        
        if (nounParams == null)
        {
            return returner;
        }
        
        if (nounParams->SheetName.IsEmpty)
        {
            return returner;
        }
        
        string sheetString = nounParams->SheetName.ExtractText();
        
        bool contains = false;
        int  index    = 0;
        
        for (int i = 0; i < AllowedSheetNames.Count; i++)
        {
            if (!string.Equals(AllowedSheetNames[i].Item1, sheetString, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }
            
            contains = true;
            index    = i;
            
            break;
        }
        
        if (!contains)
        {
            return returner;
        }
        
        PetServices.PetLog.DevLogInfo($"ProcessNounDetour: [{nounParams->SheetName.ToString()}] [{outString->ToString()}] [RowId: {nounParams->RowId}, ArticleType: {nounParams->ArticleType}, GrammaticalCase: {nounParams->GrammaticalCase}, LinkMarker: {nounParams->LinkMarker}].");
        
        if (!AllowedSheetNames[index].Item2(PetServices.PetSheets, (uint)nounParams->RowId))
        {
            return returner;
        }
        
        PreviousLastGottenPronoun = LastGottenPronoun;
        LastGottenPronoun         = outString->StringPtr.AsDalamudSeString();
        
        return returner;
    }
    
    private void Register<T>(Func<IPetSheets, uint, bool> checkFunc)
        where T : struct, IExcelRow<T>
    {
        SheetAttribute? sheetAttribute = typeof(T).GetCustomAttribute<SheetAttribute>();
        
        if (sheetAttribute == null)
        {
            return;
        }
        
        if (sheetAttribute.Name.IsNullOrWhitespace())
        {
            return;
        }
        
        AllowedSheetNames.Add((sheetAttribute.Name, checkFunc));
    }
    
    private static bool HandleValidCompanion(IPetSheets petSheets, uint rowId)
    {
        Companion? companion = petSheets.GetSheetCompanion(rowId);
        
        if (companion == null)
        {
            return false;
        }
        
        IPetSheetData? petSheetData = petSheets.GetPet(new PetSkeleton(SkeletonType.Minion, companion.Value.Model.RowId));
        
        return petSheetData != null;
    }
    
    private static bool HandleValidPet(IPetSheets petSheets, uint rowId)
        => petSheets.GetPetFromBnpcName(rowId) != null;
    
    private static bool HandleValidXBMPet(IPetSheets petSheets, uint rowId)
    {
        XBMPet? pet = petSheets.GetSheetXBMPet(rowId);
        
        if (pet == null)
        {
            return false;
        }
        
        IPetSheetData? petSheetData = petSheets.GetPetFromIcon(pet.Value.Icon);
        
        return petSheetData != null;
    }
}