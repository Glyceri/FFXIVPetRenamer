using Newtonsoft.Json;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

// Keep save file size shorter ....
#pragma warning disable IDE0130 // Namespace does not match folder structure (This is to keep the save file shorter)
namespace PN.S;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[Serializable]
internal class SerializableUserV5
{
    public readonly ulong ContentID;
    public readonly string Name;
    public readonly ushort Homeworld;
    public readonly int[] SoftSkeletonData;

    public readonly SerializableNameDataV2[] SerializableNameDatas;

    [JsonConstructor]
    public SerializableUserV5(ulong contentId, string name, ushort homeworld, int[] softSkeletonData, SerializableNameDataV2[] serializableNameDatas)
    {
        ContentID = contentId;
        Name = name;
        Homeworld = homeworld;
        SerializableNameDatas = serializableNameDatas;
        SoftSkeletonData = softSkeletonData;
    }

    public SerializableUserV5(in IPettableDatabaseEntry entry)
    {
        ContentID = entry.ContentID;
        Name = entry.Name;
        Homeworld = entry.Homeworld;
        SoftSkeletonData = entry.SoftSkeletons.ToArray();

        List<SerializableNameDataV2> list = new List<SerializableNameDataV2>();
        foreach (INamesDatabase database in entry.AllDatabases)
        {
            list.Add(new SerializableNameDataV2(database));
        }
        SerializableNameDatas = list.ToArray();
    }
}
