using Newtonsoft.Json;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;
using System.Numerics;

// Keep save file size shorter ....
#pragma warning disable IDE0130 // Namespace does not match folder structure (This is to keep the save file shorter)
namespace PN.S;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[Serializable]
internal class SerializableNameDataV3
{
    public readonly int[]      Ids           = [];
    public readonly int[]      SkeletonTypes = [];
    public readonly string[]   Names         = [];
    public readonly Vector3?[] EdgeColours   = [];
    public readonly Vector3?[] TextColours   = [];

    [JsonConstructor]
    public SerializableNameDataV3(int[] ids, int[] skeletonTypes, string[] names, Vector3?[] edgeColours, Vector3?[] textColours)
    {
        Ids           = ids;
        SkeletonTypes = skeletonTypes;
        Names         = names;
        EdgeColours   = edgeColours;
        TextColours   = textColours;
    }
#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
    public SerializableNameDataV3(SerializableNameData serializableNameData)
    {
        PetSkeleton[] petSkeletons = PetSkeletonHelper.AsPetSkeletons(serializableNameData.IDS);

        int arrayLength = petSkeletons.Length;

        PetSkeletonHelper.AsMappedArray(petSkeletons, out int[] newIds, out int[] newSkeletonTypes);

        Ids             = newIds;
        SkeletonTypes   = newSkeletonTypes;
        Names           = serializableNameData.Names;
        EdgeColours     = new Vector3?[arrayLength];
        TextColours     = new Vector3?[arrayLength];
    }
#pragma warning restore CS0618 // Type or member is obsolete

    public SerializableNameDataV3(in INamesDatabase namesDatabase)
    {
        PetSkeletonHelper.AsMappedArray(namesDatabase.IDs, out int[] newIds, out int[] newSkeletonTypes);

        Ids             = newIds;
        SkeletonTypes   = newSkeletonTypes;
        Names           = namesDatabase.Names;
        EdgeColours     = namesDatabase.EdgeColours;
        TextColours     = namesDatabase.TextColours;
    }

#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
    public SerializableNameDataV3(SerializableNameDataV2 oldName)
    {
        PetSkeleton[] newSkeletons = PetSkeletonHelper.AsPetSkeletons(oldName.IDS);

        PetSkeletonHelper.AsMappedArray(newSkeletons, out int[] newIds, out int[] newSkeletonTypes);

        Ids             = newIds;
        SkeletonTypes   = newSkeletonTypes;
        Names           = oldName.Names;
        EdgeColours     = oldName.EdgeColours;
        TextColours     = oldName.TextColours;
    }
#pragma warning restore CS0618 // Type or member is obsolete
}


