using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Update.Interfaces;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal class LegacyDatabaseHelper : IUpdatable
{
    const double secondsPerCheck = 1;

    public bool Enabled { get; set; } = true;

    DalamudServices DalamudServices { get; init; }
    IPetServices PetServices { get; init; }
    IPetLog PetLog { get; init; }
    IPettableDatabase PettableDatabase { get; init; }
    IPettableDatabase LegacyPettableDatabase { get; init; }

    int offset = 0;

    public LegacyDatabaseHelper(DalamudServices dalamudServices, IPettableDatabase legacyPettableDatabase, IPettableDatabase pettableDatabase, IPetServices petServices)
    {
        DalamudServices = dalamudServices;
        PetServices = petServices;
        PetLog = PetServices.PetLog;
        PettableDatabase = pettableDatabase;
        LegacyPettableDatabase = legacyPettableDatabase;

        offset = LegacyPettableDatabase.DatabaseEntries.Length - 1;
    }


    double timer = secondsPerCheck;

    public void OnUpdate(IFramework framework)
    {
        double elapsedSeconds = framework.UpdateDelta.TotalSeconds;
        timer += elapsedSeconds;

        if (timer >= secondsPerCheck)
        {
            timer = 0;
            HandleLegacyDatabase();
        }
    }

    unsafe void HandleLegacyDatabase()
    {
        IPettableDatabaseEntry[] entries = LegacyPettableDatabase.DatabaseEntries;

        int entriesLength = entries.Length;

        if (entriesLength == 0) return;

        int max = entriesLength - 1;

        if (offset > max) offset = max;
        if (offset < 0) offset = max;

        IPettableDatabaseEntry entry = entries[offset];

        BattleChara* character = CharacterManager.Instance()->LookupBattleCharaByName(entry.Name, true, (short)entry.Homeworld);
        if (character != null)
        {
            entry.UpdateContentID(character->ContentId, true);
            LegacyPettableDatabase.RemoveEntry(entry);
            entry.MoveToDataBase(PettableDatabase);
        }

        offset--;
    }
}
