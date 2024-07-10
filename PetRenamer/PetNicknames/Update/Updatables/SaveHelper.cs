using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Update.Interfaces;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal class SaveHelper : IUpdatable
{
    readonly IPettableDatabase Database;
    readonly Configuration Configuration;

    public bool Enabled { get; set; } = true;

    public SaveHelper(in Configuration configuration, in IPettableDatabase database)
    {
        Configuration = configuration;
        Database = database;
    }

    public void OnUpdate(IFramework framework)
    {
        IPettableDatabaseEntry[] entries = Database.DatabaseEntries;
        int length = entries.Length;

        bool hasDirty = false;

        for (int i = 0; i < length; i++)
        {
            IPettableDatabaseEntry entry = entries[i];
            if (!entry.IsActive) continue;
            if (!entry.IsDirty) continue;
            hasDirty = true;
            break;
        }

        if (!hasDirty) return;

        Configuration.Save();
    }
}
