using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.Services;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.IPC;

internal class SharingDictionary : ISharingDictionary
{
    readonly DalamudServices DalamudServices;

    // Data Sharing
    readonly Dictionary<uint, string> PetNicknameDict = new Dictionary<uint, string>();

    public SharingDictionary(in DalamudServices dalamudServices)
    {
        DalamudServices = dalamudServices;

        // Data sharing
        PetNicknameDict = DalamudServices.PetNicknamesPlugin.GetOrCreateData($"PetRenamer.GameObjectRenameDict", () => new Dictionary<uint, string>());
    }

    public void Set(GameObjectId gameObjectID, string customName)
    {
        PetNicknameDict.Add(gameObjectID.ObjectId, customName);
    }

    public void Clear()
    {
        PetNicknameDict.Clear();
    }

    public void Dispose()
    {
        PetNicknameDict.Clear();
        DalamudServices.PetNicknamesPlugin.RelinquishData($"PetRenamer.GameObjectRenameDict");
    }
}
