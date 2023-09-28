using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.Core.Serialization;
using System.Runtime.InteropServices;
using System;
using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace PetRenamer.Core.PettableUserSystem.Pet;

internal class PetBase
{
    nint _pet;
    int _skeletonID;
    int _ID;
    int _index;
    string _customName = string.Empty;
    string _baseName = string.Empty;

    bool _changed;

    nint _lastPet;
    int _lastSkeletonID;
    int _lastID;
    int _lastIndex;
    string _lastCustomName = string.Empty;
    string _lastBaseName = string.Empty;

    public nint Pet => _pet;
    public int SkeletonID => _skeletonID;
    public int ID => _ID;
    public int Index => _index;
    public string CustomName => _customName;
    public string BaseName => _baseName;
    public bool Changed => _changed;


    public unsafe void Set(Character* pet, int ID, int skeletonID, SerializableUserV3 serializableUser)
    {
        _pet = (nint)pet;
        if (pet == null)
        {
            Reset();
            return;
        }

        GameObject gObject = pet->GameObject;
        _index = gObject.ObjectIndex;
        _baseName = Marshal.PtrToStringUTF8((IntPtr)gObject.Name) ?? string.Empty;
        _customName = string.Empty;

        for(int i = 0; i < serializableUser.length; i++)
        {
            if (serializableUser.ids[i] != ID) continue;
            _customName = serializableUser.names[i];
            break;
        }

        if (_lastPet != _pet || _lastID != ID || _lastSkeletonID != _skeletonID || _lastIndex != _index || _lastCustomName != _customName || _lastBaseName != _baseName)
        {
            _changed = true;
            _lastPet = _pet;
            _lastID = ID;
            _lastSkeletonID = skeletonID;
            _lastIndex = _index;
            _lastCustomName = _customName;
            _lastBaseName = _baseName;
        }
    }

    public void Clear()
    {
        _customName = string.Empty;
        _lastID = -1;
    }

    public void FullReset()
    {
        _changed = false;
        _pet = nint.Zero;
        Reset(false);
    }

    void Reset(bool alsoClearLasts = true)
    {
        _ID = -1;
        _skeletonID = -1;
        _index = -1;

        if (!alsoClearLasts) return;
        _lastID = -1;
    }
}
