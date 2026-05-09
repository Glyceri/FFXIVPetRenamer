using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace PetRenamer.PetNicknames.IPC;

internal class SharingDictionary : ISharingDictionary
{
    // PetRenamer.PetNicknamesObjectIds        
    private readonly List<GameObjectId>        PetNicknamesObjectIdsArray  = [];
    // PetRenamer.PetNicknamesAddresses
    private readonly List<nint>                PetNicknamesAddressArray    = [];
    // PetRenamer.PetNicknamesStrings
    private readonly List<string>              PetNicknamesStringArray     = [];
    // PetRenamer.PetNicknamesEdgeColours
    private readonly List<Vector3?>            PetNicknamesEdgeColourArray = [];
    // PetRenamer.PetNicknamesTextColours
    private readonly List<Vector3?>            PetNicknamesTextColourArray = [];
    // PetRenamer.GameObjectRenameDict
    private readonly Dictionary<ulong, string> PetNicknameDict             = [];
    
    
    
    private readonly DalamudServices DalamudServices;
    
    public SharingDictionary(DalamudServices dalamudServices)
    {
        DalamudServices = dalamudServices;

        try
        {
            PetNicknameDict             = DalamudServices.DalamudPlugin.GetOrCreateData("PetRenamer.GameObjectRenameDict",    CreateDictionary);
            PetNicknamesObjectIdsArray  = DalamudServices.DalamudPlugin.GetOrCreateData("PetRenamer.PetNicknamesObjectIds",   CreateList<GameObjectId>);
            PetNicknamesAddressArray    = DalamudServices.DalamudPlugin.GetOrCreateData("PetRenamer.PetNicknamesAddresses",   CreateList<nint>);
            PetNicknamesStringArray     = DalamudServices.DalamudPlugin.GetOrCreateData("PetRenamer.PetNicknamesStrings",     CreateList<string>);
            PetNicknamesEdgeColourArray = DalamudServices.DalamudPlugin.GetOrCreateData("PetRenamer.PetNicknamesEdgeColours", CreateList<Vector3?>);
            PetNicknamesTextColourArray = DalamudServices.DalamudPlugin.GetOrCreateData("PetRenamer.PetNicknamesTextColours", CreateList<Vector3?>);
        }
        catch(Exception e) 
        {
            DalamudServices.PluginLog.Error(e, "Exception in data sharing creation.");
        }
    }

    private Dictionary<ulong, string> CreateDictionary() 
        => new Dictionary<ulong, string>();
    
    private List<T> CreateList<T>()
        => [];

    private void HandleLegacyDict(GameObjectId gameObjectId, string? customName)
    {
        _ = PetNicknameDict.Remove(gameObjectId);

        if (customName.IsNullOrWhitespace())
        {
            return;   
        }

        PetNicknameDict.Add(gameObjectId, customName);
    }
    
    private int? GetArrayIndex(nint address)
    {
        int arraySize = PetNicknamesAddressArray.Count;
        
        for (int i = 0; i < arraySize; i++)
        {
            nint gottenAddress = PetNicknamesAddressArray[i];
            
            if (gottenAddress != address)
            {
                continue;
            }
            
            return i;
        }
        
        return null;
    }
    
    private void HandleAsRemove(nint address)
    {
        int? indexOf = GetArrayIndex(address);
        
        if (indexOf == null)
        {
            return;
        }
        
        PetNicknamesObjectIdsArray.RemoveAt(indexOf.Value);
        PetNicknamesAddressArray.RemoveAt(indexOf.Value);
        PetNicknamesStringArray.RemoveAt(indexOf.Value);
        PetNicknamesEdgeColourArray.RemoveAt(indexOf.Value);
        PetNicknamesTextColourArray.RemoveAt(indexOf.Value);
    }
    
    private void HandleAsAdd(GameObjectId objectId, nint address, string customName, Vector3? edgeColour, Vector3? textColour)
    {
        int? indexOf = GetArrayIndex(address);
        
        if (indexOf == null)
        {
            PetNicknamesObjectIdsArray.Add(objectId);
            PetNicknamesAddressArray.Add(address);
            PetNicknamesStringArray.Add(customName);
            PetNicknamesEdgeColourArray.Add(edgeColour);
            PetNicknamesTextColourArray.Add(textColour);
        }
        else
        {
            PetNicknamesObjectIdsArray[indexOf.Value]  = objectId;
            PetNicknamesAddressArray[indexOf.Value]    = address;
            PetNicknamesStringArray[indexOf.Value]     = customName;
            PetNicknamesEdgeColourArray[indexOf.Value] = edgeColour;
            PetNicknamesTextColourArray[indexOf.Value] = textColour;
        }
    }
    
    private void HandleModernLists(GameObjectId gameObjectId, nint address, string? customName, Vector3? edgeColour, Vector3? textColour)
    {
        bool isRemoveAction = customName.IsNullOrWhitespace();
        
        if (isRemoveAction)
        {
            HandleAsRemove(address);
        }
        else
        {
            HandleAsAdd(gameObjectId, address, customName!, edgeColour, textColour);
        }
    }
    
    public void Set(GameObjectId gameObjectId, string? customName, nint address = 0, Vector3? edgeColour = null, Vector3? textColour = null)
    {
        HandleLegacyDict(gameObjectId, customName);
        
        HandleModernLists(gameObjectId, address, customName, edgeColour, textColour);
    }
    
    public ulong[] GetLegacySkeletons()
        => PetNicknameDict.Keys.ToArray();
    
    public string[] GetLegacyCustomNames()
        => PetNicknameDict.Values.ToArray();

    public GameObjectId[] GetGameObjectIds()
        => PetNicknamesObjectIdsArray.ToArray();

    public nint[] GetAddresses()
        => PetNicknamesAddressArray.ToArray();
    
    public string[] GetCustomNames()
        => PetNicknamesStringArray.ToArray();
    
    public Vector3?[] GetEdgeColours()
        => PetNicknamesEdgeColourArray.ToArray();
    
    public Vector3?[] GetTextColours()
        => PetNicknamesTextColourArray.ToArray();

    public void Dispose()
    {
        PetNicknameDict.Clear();
        PetNicknamesAddressArray.Clear();
        PetNicknamesStringArray.Clear();
        PetNicknamesEdgeColourArray.Clear();
        PetNicknamesTextColourArray.Clear();
        PetNicknamesObjectIdsArray.Clear();
        
        DalamudServices.DalamudPlugin.RelinquishData("PetRenamer.GameObjectRenameDict");
        DalamudServices.DalamudPlugin.RelinquishData("PetRenamer.PetNicknamesObjectIds");
        DalamudServices.DalamudPlugin.RelinquishData("PetRenamer.PetNicknamesAddresses");
        DalamudServices.DalamudPlugin.RelinquishData("PetRenamer.PetNicknamesStrings");
        DalamudServices.DalamudPlugin.RelinquishData("PetRenamer.PetNicknamesEdgeColours");
        DalamudServices.DalamudPlugin.RelinquishData("PetRenamer.PetNicknamesTextColours");
    }
}
