using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Update.Interfaces;
using System.Collections.Generic;
using PetRenamer.PetNicknames.PettableUsers;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.Interop;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal unsafe class PettableUserHandler : IUpdatable
{
    List<IPettableUser> pettableUsers = new List<IPettableUser>();

    public bool Enabled { get; set; } = true;

    DalamudServices DalamudServices { get; init; }
    PetServices PetServices { get; init; }
    IPetLog PetLog { get; init; }

    public PettableUserHandler(DalamudServices dalamudServices, PetServices petServices)
    {
        DalamudServices = dalamudServices;
        PetServices = petServices;
        PetLog = PetServices.PetLog;
    }

    public void OnUpdate(IFramework framework)
    {
        int pettableUserCount = pettableUsers.Count;

        for (int i = pettableUserCount - 1; i >= 0 ; i--)
        {
            IPettableUser pettableUser = pettableUsers[i];
            if (pettableUser.Touched)
            {
                pettableUser.Touched = false;
                continue;
            }

            PetLog.Log("Removed the Pettable User: " + pettableUser.Name + " : " + pettableUser.ContentID);
            pettableUser.Destroy();
            pettableUsers.Remove(pettableUser); 
        }
    }

    public void SetCurrentBatch(ref List<Pointer<BattleChara>> batch)
    {
        int batchLength = batch.Count;
        int currBattleCharaListCount = pettableUsers.Count;
        for (int b = 0; b < batchLength; b++)
        {
            Pointer<BattleChara> pointer = batch[b];
            bool alreadyExists = false;
            for (int i = 0; i < currBattleCharaListCount; i++)
            {
                IPettableUser pettableUser = pettableUsers[i];
                if (pettableUser.ContentID == pointer.Value->ContentId)
                {
                    pettableUser.Touched = true;
                    alreadyExists = true;
                    pettableUser.Set(pointer);
                    break;
                }
            }
            if (alreadyExists) continue;

            IPettableUser newPettableUser = new PettableUser(PetLog, pointer);
            pettableUsers.Add(newPettableUser);
            PetLog.Log("Added a new Pettable user: " + newPettableUser.Name + " : " +  newPettableUser.ContentID);
        }
    }
}
