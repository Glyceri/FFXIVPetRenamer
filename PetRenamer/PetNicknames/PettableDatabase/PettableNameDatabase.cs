using Dalamud.Utility;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableNameDatabase : INamesDatabase
{
    private static readonly Regex urlRegex = new Regex(@"\b(?:(?:https?|ftp):\/\/)?(?:(?:[a-z0-9\-]+\.)+[a-z]{2,}|localhost)(?::\d{1,5})?(?:\/[^\s]*)?\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    public PetSkeleton[] Ids         { get; private set; }
    public string[]      Names       { get; private set; }
    public Vector3?[]    EdgeColours { get; private set; }
    public Vector3?[]    TextColours { get; private set; }

    private readonly IPetServices PetServices;

    public PettableNameDatabase(IPetServices petServices, PetSkeleton[] ids, string[] names, Vector3?[] edgeColours, Vector3?[] textColours)
    {
        Names       = names;
        Ids         = ids;
        EdgeColours = edgeColours;
        TextColours = textColours;
        PetServices = petServices;
    }

    public int Length 
        => Ids.Length; 

    private int GetIndex(PetSkeleton id)
    {
        for (int i = 0; i < Length; i++)
        {
            if (Ids[i] != id)
            {
                continue;
            }

            return i;
        }

        return -1;
    }

    private T? GetElement<T>(T[] array, PetSkeleton id)
    {
        int index = GetIndex(id);

        if (index == -1)
        {
            return default;
        }
        
        return array[index];
    }
    
    public string? GetName(PetSkeleton id)
        => GetElement(Names, id);

    public Vector3? GetEdgeColour(PetSkeleton id)
        => GetElement(EdgeColours, id);

    public Vector3? GetTextColour(PetSkeleton id)
        => GetElement(TextColours, id);

    public void SetName(PetSkeleton id, string? name, Vector3? edgeColour, Vector3? textColour)
    {
        if (id.SkeletonType == SkeletonType.Invalid)
        {
            return;
        }

        string? validName = MakeNameValid(name);

        int index = GetIndex(id);

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
            Add(id, validName, edgeColour, textColour);
        }

        SetDirty();
    }

    private void RemoveAtIndex(int index)
    {
        List<PetSkeleton> newIds      = [.. Ids];
        List<string>      newNames    = [.. Names];
        List<Vector3?>    edgeColours = [.. EdgeColours];
        List<Vector3?>    textColours = [.. TextColours];

        newIds.RemoveAt(index);
        newNames.RemoveAt(index);
        edgeColours.RemoveAt(index);
        textColours.RemoveAt(index);

        Ids         = [.. newIds];
        Names       = [.. newNames];
        EdgeColours = [.. edgeColours];
        TextColours = [.. textColours];
    }

    private void Add(PetSkeleton id, string name, Vector3? edgeColour, Vector3? textColour)
    {
        List<PetSkeleton> newIds      = [.. Ids];
        List<string>      newNames    = [.. Names];
        List<Vector3?>    edgeColours = [.. EdgeColours];
        List<Vector3?>    textColours = [.. TextColours];

        newIds.Add(id);
        newNames.Add(name);
        edgeColours.Add(edgeColour);
        textColours.Add(textColour);

        Ids         = [.. newIds];
        Names       = [.. newNames];
        EdgeColours = [.. edgeColours];
        TextColours = [.. textColours];
    }
    
    public void Update(PetSkeleton[] ids, string[] names, Vector3?[] edgeColours, Vector3?[] textColours)
    {
        if (ids.Length != names.Length)
        {
            return;
        }

        List<string> newNames = [];

        for (int i = 0; i < names.Length; i++)
        {
            newNames.Add(MakeNameValid(names[i]) ?? string.Empty);
        }

        Ids         = ids;
        Names       = [.. newNames];
        EdgeColours = [.. edgeColours];
        TextColours = [.. textColours];
    }

    private void SetDirty()
    {
        PetServices.DirtyCaller.DirtyName(this);
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
}
