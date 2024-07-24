using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.Services;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.IPC;

internal class SharingDictionary : ISharingDictionary
{
    readonly DalamudServices DalamudServices;

    // Data Sharing
    readonly Dictionary<ulong, string> PetNicknameDict = new Dictionary<ulong, string>();

    public SharingDictionary(in DalamudServices dalamudServices)
    {
        DalamudServices = dalamudServices;

        try
        {
            // Data sharing
            PetNicknameDict = DalamudServices.PetNicknamesPlugin.GetOrCreateData($"PetRenamer.GameObjectRenameDict", () => new Dictionary<ulong, string>());
        }
        catch { }
    }

    public void Set(GameObjectId gameObjectID, string customName)
    {
        PetNicknameDict.Add(gameObjectID, customName);
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
