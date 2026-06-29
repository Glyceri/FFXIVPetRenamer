using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using PetRenamer.PetNicknames.PettableUsers.Enums;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal class CastHook : HookableElement
{
    private const int SuccessFullCastFlag = 534;
    
    private delegate void AddToScreenLogWithLogMessageIdDelegate(nint a1, nint a2, int a3, char a4, int a5, int a6, int a7, int a8);

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
    
    private void AddToScreenLogWithLogMessageIdDetour(nint target, nint castDealer, int unknownCastFlag, char a4, int castId, int a6, int a7, int a8)
    {
        PetServices.PetCastHelper.SetLatestCast(target, castDealer, castId);
        
        AddToScreenLogWithLogMessageIdHook?.Original(target, castDealer, unknownCastFlag, a4, castId, a6, a7, a8);
        
        if (unknownCastFlag != SuccessFullCastFlag)
        {
            return;
        }

        IPettableUser? user = PetServices.UserList.GetUser(castDealer, UserListFindType.PetMeansOwner);

        if (user == null)
        {
            return;
        }
        
        user.OnLastCastChanged((uint)castId);
    }
}