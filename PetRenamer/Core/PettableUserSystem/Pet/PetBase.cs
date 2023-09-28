using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.Core.Serialization;
using System.Runtime.InteropServices;

namespace PetRenamer.Core.PettableUserSystem.Pet;

internal class PetBase
{
    public nint Pet => _pet;
    public int ID => _id;
    public int Index => _index;

    public string CustomName => _customName;
    public string BaseName => _baseName;

    public bool PetChanged => _petChanged;

    public bool IsNull => _id != -1;
    public bool Has => _pet != nint.Zero;
    public string GetName => CustomName == string.Empty ? BaseName : CustomName;

    nint _pet;

    int _id;
    int _index;

    string _customName = string.Empty;
    string _baseName = string.Empty;

    bool _petChanged;

    int _lastID;
    nint _lastPointer;

    public unsafe void Set(nint pet, int id, SerializableUserV3 serializableUserV3)
    {
        _pet = pet;
        if (pet == nint.Zero)
        {
            _index = -1;
            _lastID = -1;
            Reset();
            return;
        }

        Character gObject = *(Character*)pet;

        _id = id;
        _index = gObject.GameObject.ObjectIndex;

        if (_lastID == _id && _lastPointer == _pet) return;

        _petChanged = true;
        _lastPointer = _pet;
        _lastID = _id;
        _baseName = Marshal.PtrToStringUTF8((nint)gObject.GameObject.Name)!;
        _customName = serializableUserV3.GetNameFor(_id)!;
    }

    public void FullReset()
    {
        _pet = nint.Zero;
        _petChanged = false;
        Reset();
    }

    public void Clear()
    {
        _lastID = -1;
        _customName = string.Empty;
    }

    void Reset()
    {
        _id = -1;
        _index = -1;
    }
}
