using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Ipc.FindAnythingIPCHelper;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.Singleton;
using PetRenamer.Utilization.Attributes;
using PetRenamer.Windows.PetWindows;
using System;
using System.Collections.Generic;
using DGameObject = Dalamud.Game.ClientState.Objects.Types.GameObject;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class PettableUserUtils : UtilsRegistryType, ISingletonBase<PettableUserUtils>
{
    public static PettableUserUtils instance { get; set; } = null!;

    public unsafe void Solve(PettableUser user, bool dontCare = false)
    {
        if (user == null) return;

        user.Reset();
        if (!PluginLink.Configuration.displayCustomNames) return;

        BattleChara* bChara = PluginLink.CharacterManager->LookupBattleCharaByName(StringUtils.instance.MakeTitleCase(user.UserName.ToLowerInvariant()), true, (short)user.Homeworld);
        if (bChara == null) return;
        user.SetUser(bChara);

        if (user.SerializableUser.hasCompanion || user.LocalUser || dontCare)
        {
            int companionIndex = bChara->Character.GameObject.ObjectIndex + 1;
            Companion* companion = (Companion*)GameObjectManager.GetGameObjectByIndex(companionIndex);
            user.SetCompanion(companion);
        }

        if (user.SerializableUser.hasBattlePet || user.LocalUser || dontCare)
        {
            BattleChara* battlePet = PluginLink.CharacterManager->LookupPetByOwnerObject(bChara);
            if (battlePet != null)
                if (battlePet->Character.CharacterData.Health == 0)
                    battlePet = AlternativeFindForBChara(bChara, battlePet);

            user.SetBattlePet(battlePet);
        }
        bool userChanged = user.SerializableUser.ToggleBackChanged();
        if (!user.LocalUser) return;
        if (userChanged) FindAnythingIPCProvider.RegisterInitialNames();
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
        tNodeText = tNodeText.Split('\r')[0];
        int id = SheetUtils.instance.GetIDFromName(tNodeText);
        if (id > -1) return (id, tNodeText);

        (int, string) action = GetAction(tNodeText);
        if(action.Item1 != -1) return (user.GetPetSkeleton(softHook, action.Item1), CleanupString(action.Item2));
        foreach (KeyValuePair<int, string> kvp in RemapUtils.instance.bakedBattlePetSkeletonToName)
        {
            if (!tNodeText.Equals(kvp.Value, StringComparison.InvariantCultureIgnoreCase) && 
                !tNodeText.StartsWith(kvp.Value, StringComparison.InvariantCultureIgnoreCase) && 
                !tNodeText.EndsWith(kvp.Value, StringComparison.InvariantCultureIgnoreCase)) continue;
            return (kvp.Key, kvp.Value);
        }
        return (id, tNodeText);
    }

    string CleanupString(string str)
    {
        return str.Replace("サモン・", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                  .Replace("Summon ", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                  .Replace("Invocation ", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                  .Replace("Invocation ", string.Empty, StringComparison.InvariantCultureIgnoreCase);
    }

    (int, string) GetAction(string tNodeText)
    {
        List<(int, string)> kvps = new List<(int, string)> ();
        foreach (KeyValuePair<int, string> kvp in RemapUtils.instance.bakedActionIDToName)
        {
            if (!tNodeText.Contains(kvp.Value, StringComparison.InvariantCultureIgnoreCase)) continue;
            kvps.Add((kvp.Key, kvp.Value));
        }

        if (kvps.Count == 0) return (-1, string.Empty);

        kvps.Sort((a, b) => a.Item2.Length.CompareTo(b.Item2.Length));
        kvps.Reverse();

        foreach (KeyValuePair<int, uint> kvp2 in RemapUtils.instance.petIDToAction)
        {
            if (kvp2.Value != kvps[0].Item1) continue;
            return (kvp2.Key, kvps[0].Item2);
        }

        return (-1, string.Empty);
    }
}
