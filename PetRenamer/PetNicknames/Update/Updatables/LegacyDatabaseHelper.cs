using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Update.Interfaces;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal class LegacyDatabaseHelper : IUpdatable
{
    const double secondsPerCheck = 1;

    public bool Enabled { get; set; } = true;

    readonly DalamudServices DalamudServices;
    readonly IPetServices PetServices;
    readonly IPetLog PetLog;
    readonly IPettableDatabase PettableDatabase;
    readonly IPettableDatabase LegacyPettableDatabase;
    readonly IPettableUserList UserList;

    int offset = 0;

    IPettableUser? lastUser;

    public LegacyDatabaseHelper(in DalamudServices dalamudServices, in IPettableDatabase legacyPettableDatabase, in IPettableDatabase pettableDatabase, in IPetServices petServices, in IPettableUserList userList)
    {
        DalamudServices = dalamudServices;
        PetServices = petServices;
        PetLog = PetServices.PetLog;
        PettableDatabase = pettableDatabase;
        LegacyPettableDatabase = legacyPettableDatabase;
        UserList = userList;

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

    void HandleLegacyDatabase()
    {
        IPettableDatabaseEntry[] entries = LegacyPettableDatabase.DatabaseEntries;

        int entriesLength = entries.Length;
        if (entriesLength == 0) return;

        IPettableUser? localPlayer = UserList.LocalPlayer;
        if (localPlayer == null)
        {
            lastUser = null;
            return;
        }

        if (lastUser == null)
        {
            lastUser = localPlayer;
            HandleAsNull(in localPlayer, in entries, entriesLength);
            return;
        }

        int max = entriesLength - 1;

        if (offset > max) offset = max;
        if (offset < 0) offset = max;

        IPettableDatabaseEntry entry = entries[offset];
        FindCharacter(entry);
        offset--;
    }

    void HandleAsNull(in IPettableUser localPlayer, in IPettableDatabaseEntry[] entries, int entriesLength)
    {
        for (int i = 0; i < entriesLength; i++)
        {
            IPettableDatabaseEntry currentEntry = entries[i];
            if (currentEntry.Homeworld != localPlayer.Homeworld) continue;
            if (currentEntry.Name != localPlayer.Name) continue;
            
            FindCharacter(currentEntry);
            break;
        }
    }

    unsafe void FindCharacter(IPettableDatabaseEntry entry)
    {
        BattleChara* character = CharacterManager.Instance()->LookupBattleCharaByName(entry.Name, true, (short)entry.Homeworld);
        if (character != null)
        {
            entry.UpdateContentID(character->ContentId, true);
            LegacyPettableDatabase.RemoveEntry(entry);
            entry.MoveToDataBase(PettableDatabase);
        }
    }
}
