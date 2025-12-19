using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.Services;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.IPC;

internal class SharingDictionary : ISharingDictionary
{
    private readonly DalamudServices DalamudServices;

    // Data Sharing
    private readonly Dictionary<ulong, string> PetNicknameDict = [];

    public SharingDictionary(DalamudServices dalamudServices)
    {
        DalamudServices = dalamudServices;

        try
        {
            // Data sharing
            PetNicknameDict = DalamudServices.DalamudPlugin.GetOrCreateData($"PetRenamer.GameObjectRenameDict", CreateDictionary);
        }
        catch(Exception e) 
        {
            DalamudServices.PluginLog.Error(e, "Error in Rename Dict Creation");
        }
    }

    private Dictionary<ulong, string> CreateDictionary()
    {
        return new Dictionary<ulong, string>();
    }

    public void Set(GameObjectId gameObjectID, string? customName)
    {
        _ = PetNicknameDict.Remove(gameObjectID);

        if (customName.IsNullOrWhitespace())
        {
            return;   
        }

        PetNicknameDict.Add(gameObjectID, customName);
    }

    public void Dispose()
    {
        PetNicknameDict.Clear();

        DalamudServices.DalamudPlugin.RelinquishData($"PetRenamer.GameObjectRenameDict");
    }
}
