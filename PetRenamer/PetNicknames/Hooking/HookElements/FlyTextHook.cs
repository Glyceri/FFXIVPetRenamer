using Dalamud.Game.Gui.FlyText;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal class FlyTextHook : HookableElement
{
    private const           int   SuccessFullCastFlag  = 534;
    private static readonly int[] LogMessageValidFlags = 
    [
        640,
        642
    ];
    
    private delegate void AddToScreenLogWithLogMessageIdDelegate(nint a1, nint a2, int a3, char a4, int a5, int a6, int a7, int a8);
    private delegate nint LogMethodDelegate(nint a1, int a2, nint a3, nint a4);
    
    [Signature("E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? B9 9E 64 00 00", DetourName = nameof(AddToScreenLogWithLogMessageIdDetour))]
    private readonly Hook<AddToScreenLogWithLogMessageIdDelegate>? AddToScreenLogWithLogMessageIdHook = null;

    [Signature("E8 ?? ?? ?? ?? 45 38 7E 25", DetourName = nameof(LogMessageDetour))]
    private readonly Hook<LogMethodDelegate>? LogMethodHook = null;

    public FlyTextHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IPettableDirtyListener dirtyListener) 
        : base(services, userList, petServices, dirtyListener) { }

    public override void Init()
    {
        DalamudServices.FlyTextGui.FlyTextCreated += OnFlyTextCreated;

        AddToScreenLogWithLogMessageIdHook?.Enable();
        LogMethodHook?.Enable();
    }

    private void OnFlyTextCreated(ref FlyTextKind kind, ref int val1, ref int val2, ref SeString text1, ref SeString text2, ref uint color, ref uint icon, ref uint damageTypeIcon, ref float yOffset, ref bool handled)
    {
        if (!PetServices.Configuration.showOnFlyout)
        {
            return;
        }

        /*
        IPettablePet? casterPet = UserList.GetPet(lastCastDealer);

        if (casterPet == null)
        {
            return;
        }
        
        string? customName = casterPet.CustomName;
        
        if (customName == null)
        {
            return;
        }

        IPetSheetData? sheetData = casterPet.PetData;
        
        if (sheetData == null)
        {
            return;
        }
*/
        //PetServices.StringHelper.ReplaceSeString(ref text1, customName, sheetData);
        //PetServices.StringHelper.ReplaceSeString(ref text2, customName, sheetData);
    }

    private void AddToScreenLogWithLogMessageIdDetour(nint target, nint castDealer, int unknownCastFlag, char a4, int castId, int a6, int a7, int a8)
    {
        AddToScreenLogWithLogMessageIdHook?.Original(target, castDealer, unknownCastFlag, a4, castId, a6, a7, a8);
        
        PetServices.PetCastHelper.SetLatestCast(target, castDealer, castId);
        
        if (unknownCastFlag != SuccessFullCastFlag)
        {
            return;
        }
        
        if (PetServices.PetSheets.CastToSoftIndex((uint)castId) == null)
        {
            return;
        }

        IPettableUser? user = UserList.GetUser(castDealer);

        user?.OnLastCastChanged((uint)castId);
    }

    private nint LogMessageDetour(nint a1, int a2, nint user, nint a4)
    {
        bool isValid = false;
        
        for (int i = 0; i < LogMessageValidFlags.Length; i++)
        {
            if (a2 != LogMessageValidFlags[i])
            {
                continue;
            }
            
            isValid = true;
            
            break;
        }

        PetServices.PetActionHelper.SetLatestUser(user, isValid);

        return LogMethodHook!.Original(a1, a2, user, a4);
    }

    protected override void OnDispose()
    {
        DalamudServices.FlyTextGui.FlyTextCreated -= OnFlyTextCreated;

        AddToScreenLogWithLogMessageIdHook?.Dispose();
        LogMethodHook?.Dispose();
    }
}
