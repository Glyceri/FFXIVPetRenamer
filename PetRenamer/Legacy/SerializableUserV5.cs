using Newtonsoft.Json;
using System;

// Keep save file size shorter ....
#pragma warning disable IDE0130 // Namespace does not match folder structure (This is to keep the save file shorter)
namespace PN.S;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[Obsolete("Use SerializableUserV6 instead")]
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
        ContentID             = contentId;
        Name                  = name;
        Homeworld             = homeworld;
        SerializableNameDatas = serializableNameDatas;
        SoftSkeletonData      = softSkeletonData;
    }
}
