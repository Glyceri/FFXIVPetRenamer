using Dalamud.Hooking;
using Dalamud.Utility;
using Dalamud.Utility.Signatures;
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
    
    private delegate uint LocalizeProcessNounDelegate(Localize* localize, Utf8String* sheetName, Utf8String* outcomeString);
    
    [Signature("E8 ?? ?? ?? ?? 84 C0 74 ?? ?? ?? ?? 4C 8D 4D", DetourName = nameof(LocalizeProcessNounDetour))]
    private readonly Hook<LocalizeProcessNounDelegate>? LocalizeProcessNounHook = null!;
    
    private readonly List<string> AllowedSheetNames = [];
    
    public PronounHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices)
    {
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
    
    private uint LocalizeProcessNounDetour(Localize* localize, Utf8String* sheetName, Utf8String* outcomeString)
    {
        uint returner = LocalizeProcessNounHook!.OriginalDisposeSafe(localize, sheetName, outcomeString);
        
        if (sheetName == null)
        {
            return returner;
        }
        
        string sheetString = sheetName->ExtractText();
        
        if (!AllowedSheetNames.Contains(sheetString))
        {
            return returner;
        }
        
        PetServices.PetLog.DevLogInfo($"ProcessNounDetour: [{sheetName->ToString()}] [{outcomeString->ToString()}].");
        
        PreviousLastGottenPronoun = LastGottenPronoun;
        LastGottenPronoun         = outcomeString->StringPtr.AsDalamudSeString();
        
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