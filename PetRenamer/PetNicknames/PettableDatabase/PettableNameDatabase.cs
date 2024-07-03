using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableNameDatabase : INamesDatabase
{
    public int[] IDs { get; private set; } = new int[0];
    public string[] Names { get; private set; } = new string[0];
    public bool IsDirty { get; private set; } = false;

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
            return Names[i];
        }
        return null;
    }

    public void SetName(int ID, string? name)
    {
        if (name == string.Empty) name = null;
        IsDirty = true;
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
}
