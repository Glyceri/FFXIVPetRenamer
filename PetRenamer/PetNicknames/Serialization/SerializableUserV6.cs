using Newtonsoft.Json;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using System;
using System.Collections.Generic;

// Keep save file size shorter ....
#pragma warning disable IDE0130 // Namespace does not match folder structure (This is to keep the save file shorter)
namespace PN.S;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[Serializable]
internal class SerializableUserV6
{
    public readonly ulong          ContentID;
    public readonly string         Name;
    public readonly ushort         Homeworld;
    public readonly int[]          SoftSkeletonData;
    public readonly int[]          SoftSkeletonTypes;

    public readonly SerializableNameDataV3[] SerializableNameDatas;

    [JsonConstructor]
    public SerializableUserV6(ulong contentId, string name, ushort homeworld, int[] softSkeletonData, int[] softSkeletonTypes, SerializableNameDataV3[] serializableNameDatas)
    {
        ContentID             = contentId;
        Name                  = name;
        Homeworld             = homeworld;
        SerializableNameDatas = serializableNameDatas;
        SoftSkeletonData      = softSkeletonData;
        SoftSkeletonTypes     = softSkeletonTypes;
    }

    public SerializableUserV6(in IPettableDatabaseEntry entry)
    {
        ContentID         = entry.ContentID;
        Name              = entry.Name;
        Homeworld         = entry.Homeworld;

        PetSkeletonHelper.AsMappedArray([.. entry.SoftSkeletons], out int[] ids, out int[] skeletonTypes);

        SoftSkeletonData  = ids;
        SoftSkeletonTypes = skeletonTypes;

        List<SerializableNameDataV3> list = [];

        foreach (INamesDatabase database in entry.AllDatabases)
        {
            list.Add(new SerializableNameDataV3(database));
        }

        SerializableNameDatas = [..list];
    }
}
