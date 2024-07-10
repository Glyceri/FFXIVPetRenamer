using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableNameDatabase : INamesDatabase
{
    public int[] IDs { get; private set; } = new int[0];
    public string[] Names { get; private set; } = new string[0];
    public bool IsDirty { get; private set; } = false;
    public bool IsDirtyForUI { get; private set; } = false;

    public PettableNameDatabase(int[] ids, string[] names)
    {
        Names = names;
        IDs = ids;
    }

    public string? GetName(int ID)
    {
        for (int i = 0; i < IDs.Length; i++)
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
        IsDirty = true;
        IsDirtyForUI = true;
        if (name == string.Empty) name = null;
        int index = IndexOf(ID);
        if (index != -1)
        {
            if (name != null) Names[index] = name;
            else RemoveAtIndex(index);
            return;
        }
        else if(name != null) Add(ID, name);
    }

    int IndexOf(int ID)
    {
        for (int i = 0; i < IDs.Length;i++)
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
}
