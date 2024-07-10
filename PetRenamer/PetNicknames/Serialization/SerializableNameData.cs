using Newtonsoft.Json;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System;

namespace PetRenamer.PetNicknames.Serialization;

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

    public SerializableNameData(in INamesDatabase namesDatabase)
    {
        IDS = namesDatabase.IDs; 
        Names = namesDatabase.Names;
    }
}

