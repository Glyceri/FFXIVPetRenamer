using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Enums;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class PartyService : IParty
{
    private const string PARTY_ADDON_NAME = "_PartyList";
    
    private readonly IPettableUserList      UserList;
    private readonly DalamudServices        DalamudServices;
    private readonly IPettableDirtyListener DirtyListener;
    
    private readonly IPettableUser?[]       Party = new IPettableUser?[IParty.MaxPartyLength];
    
    public PartyService(IPettableUserList userList, DalamudServices dalamudServices, IPettableDirtyListener dirtyListener)
    {
        DalamudServices = dalamudServices;
        UserList        = userList;
        DirtyListener   = dirtyListener;
        
        DirtyListener.RegisterOnPlayerCharacterDirty(OnDirtyPlayer);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, PARTY_ADDON_NAME, LifeCycleUpdate);
    }
    
    public void Dispose()
    { 
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdate);
        
        DirtyListener.UnregisterOnPlayerCharacterDirty(OnDirtyPlayer);
    }

    public int Length 
        { get; private set; }

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
        Length = 0;
        
        for (int i = 0; i < IParty.MaxPartyLength; i++)
        {
            Party[i] = null;
        }
        
        AgentHUD* agentHud = AgentHUD.Instance();
        
        if (agentHud == null)
        {
            DalamudServices.PluginLog.Error("AgentHUD is somehow null, this is quite bad.");
            
            return;
        }

        Length = agentHud->PartyMemberCount;
        
        for (int i = 0; i < Length; i++)
        {
            HudPartyMember partyMember = agentHud->PartyMembers[i];
            BattleChara*   partyChara  = partyMember.Object;
            
            if (partyChara == null)
            {
                continue;
            }
            
            // == 4 means "is player"
            if (partyChara->BattleNpcSubKind != BattleNpcSubKind.NpcPartyMember && (byte)partyChara->BattleNpcSubKind != 4)
            {
                continue;
            }
            
            IPettableUser? user = UserList.GetUser((nint)partyChara, UserListFindType.Direct);
            
            if (user == null)
            {
                continue;
            }
            
            byte index = partyMember.Index;
            
            if (index >= Party.Length || index >= Length)
            {
                continue;
            }
            
            Party[index] = user;
        }
    }
    
    public IEnumerator<IPettableUser?> GetEnumerator()
        => ((IEnumerable<IPettableUser?>)Party).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
