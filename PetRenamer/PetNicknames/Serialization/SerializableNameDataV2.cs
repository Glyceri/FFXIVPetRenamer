using Newtonsoft.Json;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System;
using System.Numerics;

// Keep save file size shorter ....
#pragma warning disable IDE0130 // Namespace does not match folder structure (This is to keep the save file shorter)
namespace PN.S;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[Serializable]
internal class SerializableNameDataV2
{
    public readonly int[] IDS = Array.Empty<int>(); 
    public readonly string[] Names = Array.Empty<string>();
    public readonly Vector3?[] EdgeColours = Array.Empty<Vector3?>();
    public readonly Vector3?[] TextColours = Array.Empty<Vector3?>();

    [JsonIgnore]
    public readonly bool Faulty = false;

    [JsonConstructor]
    public SerializableNameDataV2(int[] ids, string[] names, Vector3?[] edgeColours, Vector3?[] textColours)
    {
        if (ids.Length != names.Length)
        {
            Faulty = true;
            return;
        }
        IDS = ids;
        Names = names;
        EdgeColours = edgeColours;
        TextColours = textColours;
    }
#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
    public SerializableNameDataV2(SerializableNameData serializableNameData)
    {
        IDS = serializableNameData.IDS;
        Names = serializableNameData.Names;
        EdgeColours = new Vector3?[IDS.Length];
        TextColours = new Vector3?[IDS.Length];
    }
#pragma warning restore CS0618 // Type or member is obsolete

    public SerializableNameDataV2(in INamesDatabase namesDatabase)
    {
        IDS = namesDatabase.IDs; 
        Names = namesDatabase.Names;
        EdgeColours = namesDatabase.EdgeColours;
        TextColours = namesDatabase.TextColours;
    }
}


