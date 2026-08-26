using Dalamud.Game.Gui.FlyText;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using PetRenamer.PetNicknames.PettableUsers.Enums;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class CastHook : HookableElement
{
    private const int SuccessFullCastFlag = 534;
    
    private delegate void AddToScreenLogWithLogMessageIdDelegate(BattleChara* a1, BattleChara* a2, int logMessageId, char unk4, int castId, int statusId, int stackCount, int damageType);

    [Signature("E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? B9 9E 64 00 00", DetourName = nameof(AddToScreenLogWithLogMessageIdDetour))]
    private readonly Hook<AddToScreenLogWithLogMessageIdDelegate>? AddToScreenLogWithLogMessageIdHook = null;
    
    public CastHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices) { }

    public override void Init()
    {
        AddToScreenLogWithLogMessageIdHook?.Enable();
    }
    
    protected override void OnDispose()
    {
        AddToScreenLogWithLogMessageIdHook?.Dispose();
    }
    
    private void AddToScreenLogWithLogMessageIdDetour(BattleChara* target, BattleChara* castDealer, int logMessageId, char a4, int castId, int a6, int a7, int a8)
    {
        PetServices.PetCastHelper.SetLatestCast((nint)target, (nint)castDealer, castId);
        
        AddToScreenLogWithLogMessageIdHook?.Original(target, castDealer, logMessageId, a4, castId, a6, a7, a8);
        
        if (logMessageId != SuccessFullCastFlag)
        {
            return;
        }

        IPettableUser? user = PetServices.UserList.GetUser((nint)castDealer, UserListFindType.PetMeansOwner);

        if (user == null)
        {
            return;
        }
        
        user.OnLastCastChanged((uint)castId);
    }
}