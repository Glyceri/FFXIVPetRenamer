using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Party;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.Singleton;
using PetRenamer.Logging;
using PetRenamer.Utilization.Attributes;
using System.Collections.Generic;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class PartyUtils : UtilsRegistryType, ISingletonBase<PartyUtils>
{
    public static PartyUtils instance { get; set; } = null!;
    public PartyPlayer[] members { get; private set; } = new PartyPlayer[8];
    public int Length { get; private set; } = 0;

    internal override void OnRegistered() => Setup();
    public unsafe bool Update(ref IFramework frameWork, ref PlayerCharacter player)
    {
        Length = 0;
        if (PluginHandlers.PartyList.Count == 0)
        {
            PettableUser localUser = PluginLink.PettableUserHandler.LocalUser()!;
            if (localUser == null) return false;
            members[0].Set(localUser.nintUser);
            Length = 1;
        }
        else
        {
            for (int i = 0; i < PluginHandlers.PartyList.Length; i++) 
            {
                PartyMember pMember = PluginHandlers.PartyList[i]!;
                if (pMember == null) break;
                if (pMember.GameObject == null) continue;
                members[i].Set(pMember.GameObject!.Address);
                Length = i + 1;
            }
        }

        for (int i = 0; i < Length; i++)
            if (members[i].changed)
                return true;

        return false;
    }

    void Setup()
    {
        for(int i = 0; i < members.Length; i++)
            members[i] = new PartyPlayer(nint.Zero);
        Length = 0;
    }
}

public class PartyPlayer
{
    public nint Player { get; private set; } = nint.Zero;
    public nint BattlePet { get; private set; } = nint.Zero;
    public nint Chocobo { get; private set; } = nint.Zero;

    public bool changed { get; private set; } = false;

    public int RenderFlags { get; private set; } = 0;
    public PartyPlayer(nint player) => Set(player);
    public List<nint> ActivePets 
    { 
        get 
        { 
            List<nint> pets = new List<nint>();
            if (BattlePet != nint.Zero && RenderFlags == 0) pets.Add(BattlePet);
            if (Chocobo != nint.Zero) pets.Add(Chocobo);
            return pets;
        } 
    }

    public unsafe void Set(nint player)
    {
        changed = false;
        if (player == nint.Zero)
        {
            if (Player != player) changed = true;
            Player = nint.Zero;
            BattlePet = nint.Zero;
            Chocobo = nint.Zero;
            return;
        }
        Player = player;

        nint tempChocobo = (nint)PluginLink.CharacterManager->LookupBuddyByOwnerObject((BattleChara*)player);
        if (Chocobo != tempChocobo) changed = true;
        Chocobo = tempChocobo;

        nint tempPet = (nint)PluginLink.CharacterManager->LookupPetByOwnerObject((BattleChara*)player);
        if (BattlePet != tempPet || (PluginLink.PettableUserHandler.GetPet(tempPet)?.nameChanged ?? false)) changed = true;
        BattlePet = tempPet;

        if (BattlePet == nint.Zero) return;
        int renderFlags = ((BattleChara*)BattlePet)->Character.GameObject.RenderFlags;
        if (renderFlags != RenderFlags) changed = true;
        RenderFlags = renderFlags;
    }
}