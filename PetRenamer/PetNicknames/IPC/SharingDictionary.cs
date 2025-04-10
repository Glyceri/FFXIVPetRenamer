using Dalamud.Utility;
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

    public SharingDictionary(DalamudServices dalamudServices)
    {
        DalamudServices = dalamudServices;

        try
        {
            // Data sharing
            PetNicknameDict = DalamudServices.DalamudPlugin.GetOrCreateData($"PetRenamer.GameObjectRenameDict", () => new Dictionary<ulong, string>());
        }
        catch(Exception e) 
        {
            DalamudServices.PluginLog.Error(e, "Error in Rename Dict Creation");
        }
    }

    public void Set(GameObjectId gameObjectID, string? customName)
    {
        PetNicknameDict.Remove(gameObjectID);
        if (!customName.IsNullOrWhitespace())
        {
            PetNicknameDict.Add(gameObjectID, customName);
        }
    }

    public void Dispose()
    {
        PetNicknameDict.Clear();
        DalamudServices.DalamudPlugin.RelinquishData($"PetRenamer.GameObjectRenameDict");
    }
}
