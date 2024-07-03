using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.Interop;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableUser : IPettableUser
{
    public string Name { get; private set; } = "";
    public ulong ContentID { get; private set; }
    public ushort Homeworld { get; private set; }
    public uint ObjectID { get; private set; }
    public bool Touched { get; set; }
    public List<IPettablePet> PettablePets { get; } = new List<IPettablePet>();
    public BattleChara* BattleChara { get; }

    IPetLog PetLog { get; init; }
    public IPettableDatabaseEntry DataBaseEntry { get; private set; }

    public PettableUser(IPetLog petLog, IPettableDatabase dataBase, Pointer<BattleChara> battleChara)
    {
        this.PetLog = petLog;
        BattleChara* bChara = battleChara.Value;
        Name = bChara->NameString;
        ContentID = bChara->ContentId;
        Homeworld = bChara->HomeWorld;
        Touched = true;
        DataBaseEntry = dataBase.GetEntry(ContentID);
    }

    public void Destroy()
    {
        
    }

    public void Set(Pointer<BattleChara> pointer)
    {
        
    }

    public void CalculateBattlepets(IPettableUserList pettableUserList)
    {
        
    }
}
