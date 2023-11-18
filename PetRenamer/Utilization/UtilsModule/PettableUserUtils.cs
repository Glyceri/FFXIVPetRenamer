using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.Interop;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Ipc.FindAnythingIPCHelper;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.Singleton;
using PetRenamer.Utilization.Attributes;
using PetRenamer.Windows.PetWindows;
using System;
using System.Collections.Generic;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class PettableUserUtils : UtilsRegistryType, ISingletonBase<PettableUserUtils>
{
    public static PettableUserUtils instance { get; set; } = null!;

    public unsafe void Solve(PettableUser user)
    {
        user.Reset();
        if (!PluginLink.Configuration.displayCustomNames) return;
        if (PluginHandlers.ClientState.IsPvP) return;

        BattleChara* bChara = PluginLink.CharacterManager->LookupBattleCharaByName(user.UserName, true, (short)user.Homeworld);
        if (bChara == null) return;
        user.SetUser(bChara);

        if (user.SerializableUser.hasCompanion || user.LocalUser) user.SetCompanion(bChara->Character.CompanionObject);
        if (user.SerializableUser.hasBattlePet || user.LocalUser) user.SetBattlePet(AlternativeFindForBChara(bChara));

        bool userChanged = user.SerializableUser.ToggleBackChanged();
        if (!user.LocalUser) return;
        if (userChanged) FindAnythingIPCProvider.RegisterInitialNames();
        if (!user.AnyPetChanged) return;
        PetRenameWindow window = PluginLink.WindowHandler.GetWindow<PetRenameWindow>();
        if (window == null) return;
        if (user.Minion.Changed) window.OpenForMinion(user.Minion.ID);
        if (user.BattlePet.Changed) window.OpenForBattlePet(user.BattlePet.ID);
    }

    unsafe BattleChara* AlternativeFindForBChara(BattleChara* bChara)
    {
        uint objectID = bChara->Character.GameObject.ObjectID;
        Span<Pointer<BattleChara>> charaSpan = PluginLink.CharacterManager->BattleCharaListSpan;
        for(int i = 0; i < charaSpan.Length; i++)
        {
            Pointer<BattleChara> chara = charaSpan[i];
            if (chara.Value == null) continue;
            if (chara.Value == bChara) continue;
            if (chara.Value->Character.GameObject.OwnerID != objectID) continue;
            if (chara.Value->Character.CharacterData.Health == 0) continue;
            return chara;
        }
        return null!;
    }

    public (int, string) GetNameRework(string tNodeText, ref PettableUser user, bool softHook = false)
    {
        tNodeText = tNodeText.Split('\r')[0];
        int id = SheetUtils.instance.GetIDFromName(tNodeText);
        if (id > -1) return (id, tNodeText);

        (int, string) action = GetAction(tNodeText);
        if (action.Item1 != -1) return (user.GetPetSkeleton(softHook, action.Item1), CleanupString(action.Item2));
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
        List<(int, string)> kvps = new List<(int, string)>();
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
