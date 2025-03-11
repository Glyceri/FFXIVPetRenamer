using Newtonsoft.Json;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

// Keep save file size shorter ....
#pragma warning disable IDE0130 // Namespace does not match folder structure (This is to keep the save file shorter)
namespace PN.S;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[Obsolete("Use SerializableUserV5 instead")]
[Serializable]
internal class SerializableUserV4
{
    public readonly ulong ContentID;
    public readonly string Name;
    public readonly ushort Homeworld;
    public readonly int[] SoftSkeletonData;

    public readonly SerializableNameData[] SerializableNameDatas;

    [JsonConstructor]
    public SerializableUserV4(ulong contentId, string name, ushort homeworld, int[] softSkeletonData, SerializableNameData[] serializableNameDatas)
    {
        ContentID = contentId;
        Name = name;
        Homeworld = homeworld;
        SerializableNameDatas = serializableNameDatas;
        SoftSkeletonData = softSkeletonData;
    }

    public SerializableUserV4(in IPettableDatabaseEntry entry)
    {
        ContentID = entry.ContentID;
        Name = entry.Name;
        Homeworld = entry.Homeworld;
        SoftSkeletonData = entry.SoftSkeletons.ToArray();

        List<SerializableNameData> list = new List<SerializableNameData>();
        foreach (INamesDatabase database in entry.AllDatabases)
        {
            list.Add(new SerializableNameData(database));
        }
        SerializableNameDatas = list.ToArray();
    }
}
