using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using PetRenamer.PetNicknames.PettableUsers.Enums;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class EmoteHook : HookableElement
{
    private readonly Hook<EmoteManager.Delegates.ExecuteEmote> ExecuteEmoteHook;
    private readonly Hook<EmoteController.Delegates.PlayEmote> PlayEmoteHook;
    
    public EmoteHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices)
    {
        ExecuteEmoteHook = DalamudServices.Hooking.HookFromAddress<EmoteManager.Delegates.ExecuteEmote>((nint)EmoteManager.MemberFunctionPointers.ExecuteEmote, ExecuteEmoteDetour);
        PlayEmoteHook    = DalamudServices.Hooking.HookFromAddress<EmoteController.Delegates.PlayEmote>((nint)EmoteController.MemberFunctionPointers.PlayEmote, PlayEmoteDetour);
    }

    public override void Init()
    {
        ExecuteEmoteHook.Enable();
        PlayEmoteHook.Enable();
    }

    protected override void OnDispose()
    {
        ExecuteEmoteHook.Dispose();   
        PlayEmoteHook.Dispose();
    }
    
    private bool PlayEmoteDetour(EmoteController* thisPtr, uint emoteId, EmoteController.PlayEmoteOption* options)
    {
        HandlePlayEmote(thisPtr, emoteId, options);
        
        return PlayEmoteHook.Original(thisPtr, emoteId, options);
    }
    
    private void HandlePlayEmote(EmoteController* thisPtr, uint emoteId, EmoteController.PlayEmoteOption* options)
    {
        if (thisPtr == null || options == null)
        {
            return;
        }
        
        IPettableUser? user        = PetServices.UserList.GetUser((nint)thisPtr->OwnerObject, UserListFindType.Direct);
        IPettableUser? localPlayer = PetServices.UserList.LocalPlayer;
        
        if (user == null || localPlayer == null)
        {
            return;
        }
        
        if (user == localPlayer)
        {
            return;
        }
        
        HandleEmote(user, emoteId, options);
    }
    
    private bool ExecuteEmoteDetour(EmoteManager* thisPtr, ushort emoteId, EmoteController.PlayEmoteOption* options)
    {
        HandleExecuteEmote(emoteId, options);
        
        return ExecuteEmoteHook.Original(thisPtr, emoteId, options);
    }
    
    private void HandleExecuteEmote(uint emoteId, EmoteController.PlayEmoteOption* options)
    {
        if (options == null)
        {
            return;
        }
        
        IPettableUser? localPlayer = PetServices.UserList.LocalPlayer;
        
        if (localPlayer == null)
        {
            return;
        }
        
        HandleEmote(localPlayer, emoteId, options);
    }
    
    private void HandleEmote(IPettableUser executor, uint emoteId, EmoteController.PlayEmoteOption* options)
    {
        if (options->DisableLogMessage)
        {
            return;
        }
        
        IPettablePet? targetedPet = PetServices.UserList.GetPet(options->TargetId);
        
        if (targetedPet == null)
        {
            return;
        }
        
        PetServices.PetLog.DevLog(executor.DataBaseEntry.Name + $" just did an emote [{emoteId}] on: " + targetedPet.SkeletonId + " from: " + targetedPet.Owner?.DataBaseEntry.Name);
        PetServices.EmoteService.RegisterEmote(executor, emoteId, targetedPet);
    }
}