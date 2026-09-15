using Dalamud.Game;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.PettableUsers.Enums;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Structs;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class CastHook : HookableElement
{
    private const int UsedLogMessageId = 534; // <head(<if([gstr1=gstr2],you,<if(gnum7,<ennoun(ObjStr,2,gnum7,1,1)>,gstr2)>)>)> <if([gstr1=gstr2],cast,casts)> <string(lstr1)>.
    
    private readonly Hook<BattleLog.Delegates.AddToScreenLogWithLogMessageId> AddToScreenLogWithLogMessageIdHook;
    
    public CastHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices)
    {
        AddToScreenLogWithLogMessageIdHook = DalamudServices.Hooking.HookFromAddress<BattleLog.Delegates.AddToScreenLogWithLogMessageId>((nint)BattleLog.MemberFunctionPointers.AddToScreenLogWithLogMessageId, AddToScreenLogWithLogMessageIdDetour);
    }

    public override void Init()
    {
        AddToScreenLogWithLogMessageIdHook.Enable();
    }
    
    protected override void OnDispose()
    {
        AddToScreenLogWithLogMessageIdHook.Dispose();
    }
    
    private void AddToScreenLogWithLogMessageIdDetour(BattleChara* target, BattleChara* source, int logMessageId, byte actionKind, uint actionId, int value1, int value2, int value3)
    {
        ActionData currentActionData = new ActionData(actionId, (ActionKind)actionKind);
        
        PetServices.PetLog.LogFatal("JUST SET ACTION TO: " + currentActionData);
        
        PetServices.PetCastHelper.SetLatestCast((nint)target, (nint)source, currentActionData);
        
        AddToScreenLogWithLogMessageIdHook?.Original(target, source, logMessageId, actionKind, actionId, value1, value2, value3);
        
        if (logMessageId != UsedLogMessageId)
        {
            return;
        }

        IPettableUser? user = PetServices.UserList.GetUser((nint)source, UserListFindType.PetMeansOwner);

        if (user == null)
        {
            return;
        }
        
        user.OnLastCastChanged(currentActionData);
    }
}