using FFXIVClientStructs.FFXIV.Client.Game.Object;
using System;
using System.Numerics;

namespace PetRenamer.PetNicknames.IPC.Interfaces;

internal interface ISharingDictionary : IDisposable
{
    void Set(GameObjectId gameObjectId, string? customName, nint address = 0, Vector3? edgeColour = null, Vector3? textColour = null);
    
    ulong[]    GetLegacySkeletons();
    string[]   GetLegacyCustomNames();
    
    GameObjectId[] GetGameObjectIds();
    nint[]         GetAddresses();
    string[]       GetCustomNames();
    Vector3?[]     GetEdgeColours();
    Vector3?[]     GetTextColours();
}
