using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Update.Interfaces;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.Services.Interface;
using Dalamud.Game.ClientState.Objects.SubKinds;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal class LegacyDatabaseHelper : IUpdatable
{
    public bool Enabled { get; set; } = true;

    DalamudServices DalamudServices { get; init; }
    IPetServices PetServices { get; init; }
    IPetLog PetLog { get; init; }
    IPettableDatabase PettableDatabase { get; init; }

    public LegacyDatabaseHelper(DalamudServices dalamudServices, IPettableDatabase pettableDatabase, IPetServices petServices)
    {
        DalamudServices = dalamudServices;
        PetServices = petServices;
        PetLog = PetServices.PetLog;
        PettableDatabase = pettableDatabase;
    }

    double timer = 0;

    public void OnUpdate(IFramework framework, IPlayerCharacter playerCharacter)
    {
        if (!PettableDatabase.ContainsLegacy) return;
        double elapsedSeconds = framework.UpdateDelta.TotalSeconds;
        timer += elapsedSeconds;

        if (timer >= 5)
        {
            timer = 0;
            HandleLegacyDatabase();
        }
    }

    unsafe void HandleLegacyDatabase()
    {
        IPettableDatabaseEntry[] entries = PettableDatabase.DatabaseEntries;
        foreach(IPettableDatabaseEntry entry in entries)
        {
            if (!entry.IsLegacy) continue;
            BattleChara* character = CharacterManager.Instance()->LookupBattleCharaByName(entry.Name, true);
            if (character == null) continue;
            entry.RemoveLegacyStatusWith(character->ContentId);
        }
        PettableDatabase.CheckLegacyStatus();
    }
}
