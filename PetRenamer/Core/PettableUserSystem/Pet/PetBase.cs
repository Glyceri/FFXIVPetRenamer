using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using System.Runtime.InteropServices;

namespace PetRenamer.Core.PettableUserSystem.Pet;

public class PetBase
{
    public nint Pet => _pet;
    public int ID => _id;
    public int Index => _index;
    public uint ObjectID => _objectID;

    public string CustomName => _customName ?? string.Empty;
    public string BaseName => _baseName ?? string.Empty;
    public string BaseNamePlural => _baseNamePlural ?? string.Empty;

    public bool Changed => _petChanged;

    public bool IsNull => _id != -1;
    public bool Has => _pet != nint.Zero;

    nint _pet;

    int _id;
    int _index;
    uint _objectID;

    string _customName = string.Empty;
    string _baseName = string.Empty;
    string _baseNamePlural = string.Empty;

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
        _objectID = gObject.GameObject.ObjectID;

        if (_lastID == _id && _lastPointer == _pet) return;

        _petChanged = true;
        _lastPointer = _pet;
        _lastID = _id;
        _baseName = SheetUtils.instance.GetPetName(_id, NameType.Singular);
        _baseNamePlural = SheetUtils.instance.GetPetName(_id, NameType.Plural);
        _customName = serializableUserV3.GetNameFor(_id)!;
    }

    public void SetChanged() => _petChanged = true;  

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
        _objectID = 0;
    }
}
