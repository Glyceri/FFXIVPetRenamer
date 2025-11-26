using Dalamud.Utility;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PN.S;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableNameDatabase : INamesDatabase
{
    public PetSkeleton[] IDs   { get; private set; }
    public string[]      Names { get; private set; }
    
    public Vector3?[] EdgeColours { get; private set; }
    public Vector3?[] TextColours { get; private set; }

    private readonly IPettableDirtyCaller? DirtyCaller = null;

    public PettableNameDatabase(PetSkeleton[] ids, string[] names, Vector3?[] edgeColours, Vector3?[] textColours, IPettableDirtyCaller dirtyCaller)
    {
        Names       = names;
        IDs         = ids;
        EdgeColours = edgeColours;
        TextColours = textColours;
        DirtyCaller = dirtyCaller;
    }

    public int Length 
        => IDs.Length; 

    private int GetIndex(PetSkeleton ID)
    {
        for (int i = 0; i < Length; i++)
        {
            if (IDs[i] != ID)
            {
                continue;
            }

            return i;
        }

        return -1;
    }

    public string? GetName(PetSkeleton ID)
    {
        int index = GetIndex(ID);

        if (index == -1)
        {
            return null;
        }
        
        return Names[index];
    }

    public Vector3? GetEdgeColour(PetSkeleton ID)
    {
        int index = GetIndex(ID);

        if (index == -1)
        {
            return null;
        }

        return EdgeColours[index];
    }

    public Vector3? GetTextColour(PetSkeleton ID)
    {
        int index = GetIndex(ID);

        if (index == -1)
        {
            return null;
        }

        return TextColours[index];
    }

    public void SetName(PetSkeleton ID, string? name, Vector3? edgeColour, Vector3? textColour)
    {
        if (ID.SkeletonType == SkeletonType.Invalid)
        {
            return;
        }

        string? validName = MakeNameValid(name);

        int index = IndexOf(ID);

        if (index != -1)
        {
            if (validName != null)
            {
                Names[index]       = validName;
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

    private int IndexOf(PetSkeleton ID)
    {
        for (int i = 0; i < Length; i++)
        {
            if (IDs[i] != ID)
            {
                continue;
            }
            
            return i;
        }

        return -1;
    }

    private void RemoveAtIndex(int index)
    {
        List<PetSkeleton> newIds      = [..IDs];
        List<string>      newNames    = [..Names];
        List<Vector3?>    edgeColours = [..EdgeColours];
        List<Vector3?>    textColours = [..TextColours];

        newIds.RemoveAt(index);
        newNames.RemoveAt(index);
        edgeColours.RemoveAt(index);
        textColours.RemoveAt(index);

        IDs         = [..newIds];
        Names       = [..newNames];
        EdgeColours = [..edgeColours];
        TextColours = [..textColours];
    }

    private void Add(PetSkeleton id, string name, Vector3? edgeColour, Vector3? textColour)
    {
        List<PetSkeleton> newIds      = [..IDs];
        List<string>      newNames    = [..Names];
        List<Vector3?>    edgeColours = [..EdgeColours];
        List<Vector3?>    textColours = [..TextColours];

        newIds.Add(id);
        newNames.Add(name);
        edgeColours.Add(edgeColour);
        textColours.Add(textColour);

        IDs         = [..newIds];
        Names       = [..newNames];
        EdgeColours = [..edgeColours];
        TextColours = [..textColours];
    }
     
    public SerializableNameDataV3 SerializeData() 
        => new SerializableNameDataV3(this);

    public void Update(PetSkeleton[] ids, string[] names, Vector3?[] edgeColours, Vector3?[] textColours, IPettableDirtyCaller dirtyCaller)
    {
        if (ids.Length != names.Length)
        {
            return;
        }

        List<string> newNames = new List<string>();

        for (int i = 0; i < names.Length; i++)
        {
            // I feel like this should actually remove the name, not add it back as empty :erm:
            newNames.Add(MakeNameValid(names[i]) ?? string.Empty);
        }

        IDs         = ids;
        Names       = [..newNames];
        EdgeColours = [..edgeColours];
        TextColours = [..textColours];
    }

    private void SetDirty()
    {
        DirtyCaller?.DirtyName(this);
    }

    private string? MakeNameValid(string? name)
    {
        if (name.IsNullOrWhitespace())
        {
            return null;
        }

        try
        {
            name = urlRegex.Replace(name, string.Empty);
        }
        catch { }

        if (name.Length > PluginConstants.ffxivNameSize)
        {
            name = name[..PluginConstants.ffxivNameSize];
        }

        name = name.CleanString(PluginConstants.forbiddenCharacter.ToString());

        name = name.Trim();

        if (name.IsNullOrWhitespace())
        {
            return null;
        }

        return name;
    }

    // wish I commented on what this was, because I have no idea
    //
    // nvm I remember c:
    // This is to dissallow urls or weird names
    private static readonly Regex urlRegex = new Regex(
        @"\b(?:(?:https?|ftp):\/\/)?(?:(?:[a-z0-9\-]+\.)+[a-z]{2,}|localhost)(?::\d{1,5})?(?:\/[^\s]*)?\b",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );
}
