using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.Interop;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableUser : IPettableUser
{
    public string Name { get; private set; } = "";
    public ulong ContentID { get; private set; }
    public ushort Homeworld { get; private set; }
    public bool Touched { get; set; }
    public BattleChara* BattleChara { get; }

    IPetLog petLog { get; init; }
    public IPettableDatabaseEntry DataBaseEntry { get; private set; }

    public PettableUser(IPetLog petLog, IPettableDatabase dataBase, Pointer<BattleChara> battleChara)
    {
        this.petLog = petLog;
        BattleChara* bChara = battleChara.Value;
        Name = bChara->NameString;
        ContentID = bChara->ContentId;
        Homeworld = bChara->HomeWorld;
        Touched = true;
        DataBaseEntry = dataBase.GetEntry(ContentID);

        petLog.LogFatal(Name + " has: " + DataBaseEntry.IDs.Length + " nicknames in their database!");
    }

    public void Destroy()
    {

    }

    public void Set(Pointer<BattleChara> pointer)
    {
       
    }
}
