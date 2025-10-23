using Newtonsoft.Json;
using System;

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
}
