using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI.Arrays;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class PartyService : IParty
{
    private readonly IPettableUserList      UserList;
    private readonly DalamudServices        DalamudServices;
    private readonly IPettableDirtyListener DirtyListener;
    
    private readonly IPettableUser?[]  Party = new IPettableUser?[IParty.MaxPartyLength];
    
    public PartyService(IPettableUserList userList, DalamudServices dalamudServices, IPettableDirtyListener dirtyListener)
    {
        DalamudServices = dalamudServices;
        UserList        = userList;
        DirtyListener   = dirtyListener;
        
        DirtyListener.RegisterOnPlayerCharacterDirty(OnDirtyPlayer);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "_PartyList", LifeCycleUpdate);
    }
    
    public void Dispose()
    { 
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdate);
        DirtyListener.UnregisterOnPlayerCharacterDirty(OnDirtyPlayer);
    }

    public unsafe int Length 
        => PartyListNumberArray.Instance()->PartyListCount;

    public IPettableUser? this[int index]
        => Party[index];
    
    private void LifeCycleUpdate(AddonEvent addonEvent, AddonArgs addonArgs)
        => SetupParty();
    
    private void OnDirtyPlayer(IPettableUser user)
    {
        if (!user.IsLocalPlayer)
        {
            return;
        }
        
        SetupParty();
    }
    
    private unsafe void SetupParty()
    {
        int partySize = PartyListNumberArray.Instance()->PartyListCount;
        
        for (int i = 0; i < IParty.MaxPartyLength; i++)
        {
            Party[i] = null;
            
            if (i >= partySize)
            {
                continue;
            }
            
            PartyListNumberArray.PartyListMemberNumberArray partyMember = PartyListNumberArray.Instance()->PartyMembers[i];
            
            Party[i] = UserList.GetUserFromEntityId((uint)partyMember.ContentId);
        }
    }
    
    public IEnumerator<IPettableUser?> GetEnumerator()
        => ((IEnumerable<IPettableUser?>)Party).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}