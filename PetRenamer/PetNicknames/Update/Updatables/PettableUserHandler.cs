using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Update.Interfaces;
using System.Collections.Generic;
using PetRenamer.PetNicknames.PettableUsers;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.Interop;
using System;
using Dalamud.Game.ClientState.Objects.Types;
using System.Reflection;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using Dalamud.Game.ClientState.Objects.SubKinds;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal unsafe class PettableUserHandler : IUpdatable
{
    List<IPettableUser> pettableUsers = new List<IPettableUser>();

    public bool Enabled { get; set; } = true;

    DalamudServices DalamudServices { get; init; }
    IPetServices PetServices { get; init; }
    IPetLog PetLog { get; init; }
    IPettableDatabase PettableDatabase { get; init; }

    public PettableUserHandler(DalamudServices dalamudServices, IPettableDatabase pettableDatabase, IPetServices petServices)
    {
        DalamudServices = dalamudServices;
        PetServices = petServices;
        PetLog = PetServices.PetLog;
        PettableDatabase = pettableDatabase;
    }

    public void OnUpdate(IFramework framework, IPlayerCharacter playerCharacter)
    {
        Span<Pointer<BattleChara>> charaSpan = CharacterManager.Instance()->BattleCharas;

        List<Pointer<BattleChara>> battlePetsThisFrame = new List<Pointer<BattleChara>>();

        int pettableUserCount = pettableUsers.Count;
        int length = charaSpan.Length;
        for (int i = 0; i < length; i++)
        {
            Pointer<BattleChara> chara = charaSpan[i];
            if (chara.Value == null) continue;
            ulong contentID = chara.Value->ContentId;
            if (contentID == 0)
            {
                battlePetsThisFrame.Add(chara);
                continue;
            }

            bool alreadyExists = false;
            for (int c = 0; c < pettableUserCount; c++)
            {
                IPettableUser pettableUser = pettableUsers[c];
                if (pettableUser.ContentID == chara.Value->ContentId)
                {
                    pettableUser.Touched = true;
                    alreadyExists = true;
                    pettableUser.Set(chara);
                    break;
                }
            }
            if (alreadyExists) continue;

            IPettableUser newPettableUser = new PettableUser(PetLog, PettableDatabase, chara);
            pettableUsers.Add(newPettableUser);
            PetLog.Log("Added a new Pettable user: " + newPettableUser.Name + " : " + newPettableUser.ContentID);
        }

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
}
