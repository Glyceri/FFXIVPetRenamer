using Dalamud.Utility;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PN.S;
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableNameDatabase : INamesDatabase
{
    public int[] IDs { get; private set; } = new int[0];
    public string[] Names { get; private set; } = new string[0];
    public bool IsDirty { get; private set; } = false;
    public bool IsDirtyForUI { get; private set; } = false;
    public int Length { get => IDs.Length; }

    public PettableNameDatabase(int[] ids, string[] names)
    {
        Names = names;
        IDs = ids;
    }

    public string? GetName(int ID)
    {
        for (int i = 0; i < Length; i++)
        {
            if (IDs[i] != ID) continue;
            string customName = Names[i];
            if (customName == string.Empty) return null;
            return customName;
        }
        return null;
    }

    public void SetName(int ID, string? name)
    {
        if (ID == -1) return;
        string? validName = MakeNameValid(name);
        SetDirty();
        int index = IndexOf(ID);
        if (index != -1)
        {
            if (validName != null) Names[index] = validName;
            else RemoveAtIndex(index);
            return;
        }
        else if(validName != null) Add(ID, validName);
    }

    int IndexOf(int ID)
    {
        for (int i = 0; i < Length; i++)
        {
            if (IDs[i] == ID) return i;
        }
        return -1;
    }

    void RemoveAtIndex(int index)
    {
        List<int> newIds = IDs.ToList();
        List<string> newNames = Names.ToList();
        newIds.RemoveAt(index);
        newNames.RemoveAt(index);
        IDs = newIds.ToArray();
        Names = newNames.ToArray();
    }

    void Add(int id, string name)
    {
        List<int> newIds = IDs.ToList();
        List<string> newNames = Names.ToList();
        newIds.Add(id);
        newNames.Add(name);
        IDs = newIds.ToArray();
        Names = newNames.ToArray();
    }

    public void MarkDirtyAsNoticed() => IsDirty = false;
    public void MarkDirtyUIAsNotified() => IsDirtyForUI = false;

    public SerializableNameData SerializeData() => new SerializableNameData(this);

    public void Update(int[] ids, string[] names)
    {
        if (ids.Length != names.Length)
        {
            return;
        }

        IDs = [];
        Names = [];

        for (int i = 0; i < ids.Length; i++)
        {
            SetName(ids[i], names[i]);
        }
    }

    void SetDirty()
    {
        IsDirty = true;
        IsDirtyForUI = true;
    }

    string? MakeNameValid(string? name)
    {
        if (name.IsNullOrWhitespace()) return null;

        if (name.Length > PluginConstants.ffxivNameSize)
        {
            name = name.Substring(0, PluginConstants.ffxivNameSize);
        }

        name = name.Replace(PluginConstants.forbiddenCharacter.ToString(), string.Empty);

        name = name.Trim();
        if (name.IsNullOrWhitespace()) return null;

        return name;
    }
}
