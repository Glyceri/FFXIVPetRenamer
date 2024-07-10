using Newtonsoft.Json;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Serialization;

[Serializable]
internal class SerializableUserV4
{
    public readonly ulong ContentID;
    public readonly string Name;
    public readonly ushort Homeworld;
    [JsonIgnore]
    public readonly bool OldUser;
    public readonly int[] SoftSkeletonData;

    public readonly SerializableNameData[] SerializableNameDatas;

    [JsonConstructor]
    public SerializableUserV4(ulong contentId, string name, ushort homeworld, int[] softSkeletonData, SerializableNameData[] serializableNameDatas)
    {
        ContentID = contentId;
        OldUser = name.Contains(PluginConstants.HalfWidthSpace);
        Name = name.Replace(PluginConstants.HalfWidthSpace, " ");
        Homeworld = homeworld;
        SerializableNameDatas = serializableNameDatas;
        SoftSkeletonData = softSkeletonData;
    }

    public SerializableUserV4(in IPettableDatabaseEntry entry)
    {
        ContentID = entry.ContentID;
        Name = entry.Name;
        Homeworld = entry.Homeworld;
        OldUser = entry.OldUser;
        SoftSkeletonData = entry.SoftSkeletons;

        List<SerializableNameData> list = new List<SerializableNameData>();
        foreach(INamesDatabase database in entry.AllDatabases)
        {
            list.Add(new SerializableNameData(database));
        }
        SerializableNameDatas = list.ToArray();

        if (entry.OldUser) Name = Name.Replace(" ", PluginConstants.HalfWidthSpace);
    }

}
