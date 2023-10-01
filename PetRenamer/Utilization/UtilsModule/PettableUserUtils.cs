﻿using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.Attributes;
using PetRenamer.Core.Singleton;
using PetRenamer.Core.PettableUserSystem;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Windows.PetWindows;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using DGameObject = Dalamud.Game.ClientState.Objects.Types.GameObject;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class PettableUserUtils : UtilsRegistryType, ISingletonBase<PettableUserUtils>
{
    public static PettableUserUtils instance { get; set; } = null!;

    public unsafe void Solve(PettableUser user)
    {
        if (user == null) return;

        user.Reset();
        if (!PluginLink.Configuration.displayCustomNames) return;

        BattleChara * bChara = PluginLink.CharacterManager->LookupBattleCharaByName(user.UserName, true, (short)user.Homeworld);
        if (bChara == null) return;
        user.SetUser(bChara);

        if (user.SerializableUser.hasCompanion || user.LocalUser)
        {
            int companionIndex = bChara->Character.GameObject.ObjectIndex + 1;
            Companion* companion = (Companion*)GameObjectManager.GetGameObjectByIndex(companionIndex);
            user.SetCompanion(companion);
        }

        if (user.SerializableUser.hasBattlePet || user.LocalUser)
        {
            BattleChara* battlePet = PluginLink.CharacterManager->LookupPetByOwnerObject(bChara);
            if (battlePet != null)
                if (battlePet->Character.CharacterData.Health == 0)
                    battlePet = AlternativeFindForBChara(bChara, battlePet);
            user.SetBattlePet(battlePet);
        }
        if (!PluginLink.Configuration.automaticallySwitchPetmode) return;
        if (!user.LocalUser) return;
        if (user.UserChanged)
        {
            PetRenameWindow window = PluginLink.WindowHandler.GetWindow<PetRenameWindow>();
            if (window == null) return;
            if (user.Minion.Changed)            window.OpenForMinion(user.Minion.ID);
            else if (user.BattlePet.Changed)    window.OpenForBattlePet(user.BattlePet.ID);
            else                                window.OpenForId(user.UserChangedID);
        }
    }

    unsafe BattleChara* AlternativeFindForBChara(BattleChara* bChara, BattleChara* basePet)
    {
        for (int i = 2; i < PluginHandlers.ObjectTable.Length; i += 2)
        {
            DGameObject? current = PluginHandlers.ObjectTable[i];
            if (current == null) continue;
            BattleChara* curPet = (BattleChara*)current.Address;
            if (curPet == null) continue;
            if (curPet == basePet) continue;
            if (curPet->Character.GameObject.OwnerID == bChara->Character.GameObject.ObjectID)
                return curPet;
        }
        return basePet;
    }
}
