using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.Attributes;
using PetRenamer.Core.Singleton;
using PetRenamer.Core.PettableUserSystem;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Windows.PetWindows;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using DGameObject = Dalamud.Game.ClientState.Objects.Types.GameObject;
using System;
using System.Runtime.InteropServices;
using PetRenamer.Logging;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class PettableUserUtils : UtilsRegistryType, ISingletonBase<PettableUserUtils>
{
    public static PettableUserUtils instance { get; set; } = null!;

    public unsafe void Solve(PettableUser user, bool complete, bool petOnly)
    {
        if (user == null) return;
        if (!PluginLink.Configuration.displayCustomNames) { user.Reset(); return; }

        BattleChara* bChara = (BattleChara*)user.nintUser;

        user.Reset();

        if (bChara != null && GetData(bChara->Character) == user.Data) user.SetUser(bChara);
        else if (complete) user.SetUser(bChara = PluginLink.CharacterManager->LookupBattleCharaByName(user.UserName, true, (short)user.Homeworld));
        if (user.nintUser == nint.Zero || bChara == null) return;

        if (user.SerializableUser.hasCompanion || user.LocalUser)
        {
            if (user.Minion.Pet != nint.Zero)
            {
                (string, uint) data = GetData2(((Companion*)user.Minion.Pet)->Character);
                if (data.Item2 != bChara->Character.GameObject.ObjectID || data.Item1 != user.Minion.BaseName)
                    user.Minion.FullReset();
            }

            if (user.Minion.Pet == nint.Zero && (complete || petOnly))
            {
                int companionIndex = bChara->Character.GameObject.ObjectIndex + 1;
                Companion* companion = (Companion*)GameObjectManager.GetGameObjectByIndex(companionIndex);
                user.SetCompanion(companion);
            }
            else user.SetCompanion((Companion*)user.Minion.Pet);
        }

        if (user.SerializableUser.hasBattlePet || user.LocalUser)
        {
            if (user.BattlePet.Pet != nint.Zero)
            {               (string, uint) data = GetData2(((BattleChara*)user.BattlePet.Pet)->Character);
                if (data.Item2 != bChara->Character.GameObject.ObjectID || data.Item1 != user.BattlePet.BaseName)
                    user.BattlePet.FullReset();
            }

            if (user.BattlePet.Pet == nint.Zero && (complete || petOnly))
            {
                BattleChara* battlePet = PluginLink.CharacterManager->LookupPetByOwnerObject(bChara);
                if (battlePet != null)
                    if (battlePet->Character.CharacterData.Health == 0)
                        battlePet = AlternativeFindForBChara(bChara, battlePet);
                user.SetBattlePet(battlePet);
            }
            else user.SetBattlePet((BattleChara*)user.BattlePet.Pet);
        }
        if (!PluginLink.Configuration.automaticallySwitchPetmode) return;
        if (!user.LocalUser) return;
        if (user.UserChanged)
        {
            if (user.Minion.Changed) GetWindow?.OpenForMinion(user.Minion.ID);
            else if (user.BattlePet.Changed) GetWindow?.OpenForBattlePet(user.BattlePet.ID);
            else GetWindow?.OpenForId(user.UserChangedID);
        }
    }

    PetRenameWindow GetWindow => PluginLink.WindowHandler.GetWindow<PetRenameWindow>();

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

    unsafe (string, ushort) GetData(Character bChara) => (Marshal.PtrToStringUTF8((IntPtr)bChara.GameObject.Name)!, bChara.HomeWorld);
    unsafe (string, uint) GetData2(Character bChara) => (Marshal.PtrToStringUTF8((IntPtr)bChara.GameObject.Name)!, bChara.CompanionOwnerID);
}
