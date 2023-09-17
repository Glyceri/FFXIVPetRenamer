using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Core.Serialization;
using System.Runtime.InteropServices;
using System;

namespace PetRenamer.Core.Helpers;

public class GeneralPetData
{
    nint _pet;
    int _id;
    int _skeletonID;

    string _customName = string.Empty;
    string _baseName = string.Empty;
    int _index;

    bool _petChanged;

    int _lastID;
    int _lastSkeletonID;
    nint _lastPointer;

    public readonly string identifier;

    public GeneralPetData(string identifier) => this.identifier = identifier;

    public unsafe void Set(nint pet, int id, ref SerializableUserV3 serializableUserV3, int skeletonID = -1)
    {
        if (pet == nint.Zero)
        {
            _index = -1;
            _lastID = -1;
            Reset();
            return;
        }

        _pet = pet;
        GameObject gObject = *(GameObject*)pet;
       
        _id = id;
        _skeletonID = skeletonID;
        _index = gObject.ObjectIndex;

        if (_lastID == _id && _lastSkeletonID == _skeletonID && _lastPointer == _pet) return;

        _petChanged = true;
        _lastPointer = _pet;
        _lastID = _id;
        _lastSkeletonID = _skeletonID;
        _baseName = Marshal.PtrToStringUTF8((nint)gObject.Name)!;
        _customName = serializableUserV3.GetNameFor(_id)!;
    }

    void

    public void FullReset()
    {
        _pet = nint.Zero;
        _petChanged = false;
        Reset();
    }

    void Reset()
    {
        _id = -1;
        _skeletonID = -1;
        _index = -1;
    }
}
