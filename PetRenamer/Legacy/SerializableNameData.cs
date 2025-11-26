using Newtonsoft.Json;
using System;

// Keep save file size shorter ....
#pragma warning disable IDE0130 // Namespace does not match folder structure (This is to keep the save file shorter)
namespace PN.S;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[Obsolete("Use SerializableNameDataV2 instead")]
[Serializable]
internal class SerializableNameData
{
    public readonly int[] IDS = Array.Empty<int>(); 
    public readonly string[] Names = Array.Empty<string>();
    [JsonIgnore]
    public readonly bool Faulty = false;

    [JsonConstructor]
    public SerializableNameData(int[] ids, string[] names)
    {
        if (ids.Length != names.Length)
        {
            Faulty = true;
            return;
        }
        IDS = ids;
        Names = names;
    }
}

