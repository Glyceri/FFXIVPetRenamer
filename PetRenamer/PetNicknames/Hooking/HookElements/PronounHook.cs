using Dalamud.Hooking;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.Text;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System.Collections.Generic;
using System.Reflection;
using DalamudSeString = Dalamud.Game.Text.SeStringHandling.SeString;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class PronounHook : HookableElement, IPronounHook
{
    public DalamudSeString? LastGottenPronoun         { get; private set; }
    public DalamudSeString? PreviousLastGottenPronoun { get; private set; }
    
    private readonly Hook<Localize.Delegates.ProcessNoun> LocalizeProcessNounHook;
    
    private readonly List<string> AllowedSheetNames = [];
    
    public PronounHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices)
    {
        LocalizeProcessNounHook = DalamudServices.Hooking.HookFromAddress<Localize.Delegates.ProcessNoun>((nint)Localize.MemberFunctionPointers.ProcessNoun, LocalizeProcessNounDetour);
        
        Register<Companion>();
        Register<BNpcName>();
        Register<Pet>();
        Register<XBMPet>();
        
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
        
        if (!AllowedSheetNames.Contains(sheetString))
        {
            return returner;
        }
        
        PetServices.PetLog.DevLogInfo($"ProcessNounDetour: [{nounParams->SheetName.ToString()}] [{outString->ToString()}].");
        
        PreviousLastGottenPronoun = LastGottenPronoun;
        LastGottenPronoun         = outString->StringPtr.AsDalamudSeString();
        
        return returner;
    }
    
    private void Register<T>()
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
        
        AllowedSheetNames.Add(sheetAttribute.Name);
    }
}