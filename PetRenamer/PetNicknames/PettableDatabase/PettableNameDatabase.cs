using Dalamud.Utility;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PN.S;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableNameDatabase : INamesDatabase
{
    public int[] IDs { get; private set; }
    public string[] Names { get; private set; }
    public int Length { get => IDs.Length; }
    public Vector3?[] EdgeColours { get; private set; }
    public Vector3?[] TextColours { get; private set; }

    readonly IPettableDirtyCaller? DirtyCaller = null;

    public PettableNameDatabase(int[] ids, string[] names, Vector3?[] edgeColours, Vector3?[] textColours, IPettableDirtyCaller dirtyCaller)
    {
        Names = names;
        IDs = ids;
        EdgeColours = edgeColours;
        TextColours = textColours;
        DirtyCaller = dirtyCaller;
    }

    int GetIndex(int ID)
    {
        for (int i = 0; i < Length; i++)
        {
            if (IDs[i] != ID) continue;

            return i;
        }

        return -1;
    }

    public string? GetName(int ID)
    {
        int index = GetIndex(ID);
        if (index == -1) return null;
        
        return Names[index];
    }

    public Vector3? GetEdgeColour(int ID)
    {
        int index = GetIndex(ID);
        if (index == -1) return null;

        return EdgeColours[index];
    }

    public Vector3? GetTextColour(int ID)
    {
        int index = GetIndex(ID);
        if (index == -1) return null;

        return TextColours[index];
    }

    public void SetName(int ID, string? name, Vector3? edgeColour, Vector3? textColour)
    {
        if (ID == -1) return;

        string? validName = MakeNameValid(name);
        int index = IndexOf(ID);

        if (index != -1)
        {
            if (validName != null)
            {
                Names[index] = validName;
                EdgeColours[index] = edgeColour;
                TextColours[index] = textColour;
            }
            else
            {
                RemoveAtIndex(index);
            }
        }
        else if (validName != null)
        {
            Add(ID, validName, edgeColour, textColour);
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
        List<Vector3?> edgeColours = EdgeColours.ToList();
        List<Vector3?> textColours = TextColours.ToList();
        newIds.RemoveAt(index);
        newNames.RemoveAt(index);
        edgeColours.RemoveAt(index);
        textColours.RemoveAt(index);
        IDs = newIds.ToArray();
        Names = newNames.ToArray();
        EdgeColours = edgeColours.ToArray();
        TextColours = textColours.ToArray();
    }

    void Add(int id, string name, Vector3? edgeColour, Vector3? textColour)
    {
        List<int> newIds = IDs.ToList();
        List<string> newNames = Names.ToList();
        List<Vector3?> edgeColours = EdgeColours.ToList();
        List<Vector3?> textColours = TextColours.ToList();
        newIds.Add(id);
        newNames.Add(name);
        edgeColours.Add(edgeColour);
        textColours.Add(textColour);
        IDs = newIds.ToArray();
        Names = newNames.ToArray();
        EdgeColours = edgeColours.ToArray();
        TextColours = textColours.ToArray();
    }
     
    public SerializableNameDataV2 SerializeData() => new SerializableNameDataV2(this);

    public void Update(int[] ids, string[] names, Vector3?[] edgeColours, Vector3?[] textColours, IPettableDirtyCaller dirtyCaller)
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
        EdgeColours = edgeColours.ToArray();
        TextColours = textColours.ToArray();
    }

    void SetDirty()
    {
        DirtyCaller?.DirtyName(this);
    }

    string? MakeNameValid(string? name)
    {
        if (name.IsNullOrWhitespace()) return null;

        try
        {
            name = urlRegex.Replace(name, string.Empty);
        }
        catch { }

        if (name.Length > PluginConstants.ffxivNameSize)
        {
            name = name[..PluginConstants.ffxivNameSize];
        }

        name = name.Replace(PluginConstants.forbiddenCharacter.ToString(), string.Empty);

        name = name.Trim();
        if (name.IsNullOrWhitespace()) return null;

        return name;
    }

    static readonly Regex urlRegex = new Regex(
        @"\b(?:(?:https?|ftp):\/\/)?(?:(?:[a-z0-9\-]+\.)+[a-z]{2,}|localhost)(?::\d{1,5})?(?:\/[^\s]*)?\b",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );
}
