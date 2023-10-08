using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.Attributes;
using PetRenamer.Core.Singleton;
using PetRenamer.Core.PettableUserSystem;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Windows.PetWindows;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using DGameObject = Dalamud.Game.ClientState.Objects.Types.GameObject;
using System.Collections.Generic;

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

        BattleChara* bChara = PluginLink.CharacterManager->LookupBattleCharaByName(StringUtils.instance.MakeTitleCase(user.UserName.ToLowerInvariant()), true, (short)user.Homeworld);
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
        user.SerializableUser.ToggleBackChanged();
        if (!user.LocalUser) return;
        if (!user.AnyPetChanged) return;
        PetRenameWindow window = PluginLink.WindowHandler.GetWindow<PetRenameWindow>();
        if (window == null) return;
        if (user.Minion.Changed) window.OpenForMinion(user.Minion.ID);
        if (user.BattlePet.Changed) window.OpenForBattlePet(user.BattlePet.ID);
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

    public (int, string) GetNameRework(string tNodeText, ref PettableUser user, bool softHook = false)
    {
        int id = SheetUtils.instance.GetIDFromName(tNodeText);
        if (id > -1) return (id, tNodeText);

        foreach(KeyValuePair<int, string> kvp in RemapUtils.instance.bakedBattlePetSkeletonToName)
        {
            if (!tNodeText.Equals(kvp.Value, System.StringComparison.InvariantCultureIgnoreCase)) continue;
            return (kvp.Key, kvp.Value);
        }

        (int, string) action = GetAction(tNodeText);
        return (user.GetPetSkeleton(softHook, action.Item1), CleanupString(action.Item2));
    }

    string CleanupString(string str)
    {
        return str.Replace("サモン・", string.Empty, System.StringComparison.InvariantCultureIgnoreCase)
                  .Replace("Summon ", string.Empty, System.StringComparison.InvariantCultureIgnoreCase)
                  .Replace("Invocation ", string.Empty, System.StringComparison.InvariantCultureIgnoreCase)
                  .Replace("-Beschwörung", string.Empty, System.StringComparison.InvariantCultureIgnoreCase);
    }

    (int, string) GetAction(string tNodeText)
    {
        foreach (KeyValuePair<int, string> kvp in RemapUtils.instance.bakedActionIDToName)
        {
            if (!tNodeText.Contains(kvp.Value)) continue;
            foreach (KeyValuePair<int, uint> kvp2 in RemapUtils.instance.petIDToAction)
            {
                if (kvp2.Value != kvp.Key) continue;
                return (kvp2.Key, kvp.Value);
            }
        }
        return (-1, string.Empty);
    }
}
