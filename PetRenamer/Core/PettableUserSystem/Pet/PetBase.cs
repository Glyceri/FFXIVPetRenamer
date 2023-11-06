using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using PetRenamer.Commands.Commands;
using PetRenamer.Core.Serialization;
using PetRenamer.Logging;
using PetRenamer.Utilization.UtilsModule;
using System.Diagnostics;

namespace PetRenamer.Core.PettableUserSystem.Pet;

public class PetBase
{
    public nint Pet => _pet;
    public int ID => _id;
    public int Index => _index;
    public uint ObjectID => _objectID;

    public string IPCName => _ipcCustomName ?? string.Empty;
    public string RawCustomName => _customName ?? string.Empty;
    public string CustomName => IPCName == string.Empty ? (_customName ?? string.Empty) : IPCName;
    public string BaseName => _baseName ?? string.Empty;
    public string BaseNamePlural => _baseNamePlural ?? string.Empty;
    public string BaseNameCapitalized => StringUtils.instance.MakeTitleCase(BaseName);
    public string BaseNamePluralCapitalized => StringUtils.instance.MakeTitleCase(BaseNamePlural);
    public string UsedName => Faulty ? BaseNameCapitalized : IPCName == string.Empty ? CustomName == string.Empty ? BaseNameCapitalized : CustomName : IPCName; // Yes, very readable :)

    public bool nameChanged => _nameChanged;
    public bool Changed => _petChanged;
    public bool Faulty => _faulty;

    public bool IsNull => _id != -1;
    public bool Has => _pet != nint.Zero;

    public bool IsIPCPet => _ipcCustomName != null && _ipcCustomName != string.Empty;

    nint _pet;

    int _id;
    int _index;
    uint _objectID;

    string _ipcCustomName = string.Empty;
    string _customName = string.Empty;
    string _baseName = string.Empty;
    string _baseNamePlural = string.Empty;

    bool _petChanged;
    bool _nameChanged;

    int _lastID;
    nint _lastPointer;
    bool _faulty;

    public unsafe void Set(nint pet, int id, SerializableUserV3 serializableUserV3)
    {
        _pet = pet;
        if (_lastID != id || _lastPointer != pet) _petChanged = true;

        if (pet == nint.Zero)
        {
            _index = -1;
            _lastID = -1;
            _lastPointer = pet;
            Reset();
            return;
        }

        Character gObject = *(Character*)pet;
        _faulty = CatchFaultyPlayer(gObject.GameObject);
        if (id == -2621) _faulty = false;

        _id = id;
        _index = gObject.GameObject.ObjectIndex;
        _objectID = gObject.GameObject.ObjectID;

        _nameChanged = serializableUserV3.changed;

        if (!_petChanged && !_nameChanged) return;

        _lastPointer = _pet;
        _lastID = _id;

        _baseName = SheetUtils.instance.GetPetName(_id, NameType.Singular);
        _baseNamePlural = SheetUtils.instance.GetPetName(_id, NameType.Plural);
        _customName = serializableUserV3.GetNameFor(_id)!;

        _ipcCustomName = string.Empty;
    }

    public void SetIPCName(string name) => _ipcCustomName = name;

    unsafe bool CatchFaultyPlayer(GameObject gObject)
    {
        if (!gObject.IsCharacter()) return true;
        if (gObject.GetDrawObject() == null) return false;
        if (((CharacterBase*)gObject.GetDrawObject())->GetModelType() != CharacterBase.ModelType.Monster) return true;
        return false;
    }

    public void SetChanged() => _petChanged = true;  

    public void FullReset()
    {
        _pet = nint.Zero;
        SoftReset();
    }

    public void SoftReset()
    {
        _petChanged = false;
        Reset();
    }

    public void Clear()
    {
        _lastID = -1;
        _customName = string.Empty;
        _ipcCustomName = string.Empty;
    }

    void Reset()
    {
        _id = -1;
        _index = -1;
        _objectID = 0;
    }
}
