using Dalamud.Utility;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PN.S;
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableNameDatabase : INamesDatabase
{
    public int[] IDs { get; private set; } = [];
    public string[] Names { get; private set; } = [];
    public int Length { get => IDs.Length; }

    readonly IPettableDirtyCaller? DirtyCaller = null;

    public PettableNameDatabase(int[] ids, string[] names, in IPettableDirtyCaller dirtyCaller)
    {
        Names = names;
        IDs = ids;
        DirtyCaller = dirtyCaller;
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
        int index = IndexOf(ID);

        if (index != -1)
        {
            if (validName != null)
            {
                Names[index] = validName;
            }
            else
            {
                RemoveAtIndex(index);
            }
        }
        else if (validName != null)
        {
            Add(ID, validName);
        }

        SetDirty();
    }

    int IndexOf(int ID)
    {
        for (int i = 0; i < Length; i++)
        {
            if (IDs[i] != ID) continue;
            
            return i;
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
     
    public SerializableNameData SerializeData() => new SerializableNameData(this);

    public void Update(int[] ids, string[] names, IPettableDirtyCaller dirtyCaller)
    {
        if (ids.Length != names.Length)
        {
            return;
        }

        List<string> newNames = new List<string>();

        for (int i = 0; i < names.Length; i++)
        {
            newNames.Add(MakeNameValid(names[i]) ?? string.Empty);
        }

        IDs = ids;
        Names = newNames.ToArray();
    }

    void SetDirty()
    {
        DirtyCaller?.DirtyName(this);
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
