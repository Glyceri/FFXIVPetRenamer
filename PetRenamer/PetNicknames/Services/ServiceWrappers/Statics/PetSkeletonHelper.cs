using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;

internal static class PetSkeletonHelper
{
    public static bool IsInvalid(this PetSkeleton petSkeleton)
    {
        if (petSkeleton.SkeletonType == SkeletonType.Invalid)
        {
            return true;
        }

        if (petSkeleton.SkeletonId == 0)
        {
            return true;
        }

        return false;
    }

    public static void AsLegacy(this PetSkeleton petSkeleton, out int Id)
    {
        Id = (int)petSkeleton.SkeletonId;

        SkeletonType type = petSkeleton.SkeletonType;

        if (type == SkeletonType.BattlePet)
        {
            Id = -Id;
        }
        else if (type != SkeletonType.Minion)
        {
            Id = -1;
        }
    }

    public static void AsLegacyArray(this PetSkeleton[] petSkeletonArray, out int[] ids)
    {
        int arrayLength = petSkeletonArray.Length;

        ids             = new int[arrayLength];

        for (int i = 0; i < arrayLength; i++)
        {
            PetSkeleton skeleton = petSkeletonArray[i];

            ids[i]            = (int)skeleton.SkeletonId;
            SkeletonType type = skeleton.SkeletonType;

            if (type == SkeletonType.BattlePet)
            {
                ids[i] = -ids[i];
            }
            else if (type != SkeletonType.Minion)
            {
                ids[i] = -1;
            }
        }
    }

    public static void AsMappedArray(this PetSkeleton[] petSkeletonArray, out int[] ids, out int[] skeletonTypes)
    {
        int arrayLength = petSkeletonArray.Length;

        ids           = new int[arrayLength];
        skeletonTypes = new int[arrayLength];

        for (int i = 0; i < arrayLength; i++) 
        {
            PetSkeleton skeleton = petSkeletonArray[i];

            ids[i]           = (int)skeleton.SkeletonId;
            skeletonTypes[i] = (int)skeleton.SkeletonType;
        }
    }

    public static PetSkeleton[] AsPetSkeletons(int[] skeletons, int[] skeletonTypes)
    {
        int arrayLength = skeletons.Length;

        PetSkeleton[] newSkeletons = new PetSkeleton[arrayLength];

        for (int i = 0; i < arrayLength; i++)
        {
            newSkeletons[i] = new PetSkeleton((uint)skeletons[i], (SkeletonType)skeletonTypes[i]);
        }

        return newSkeletons;
    }

    public static PetSkeleton AsPetSkeleton(int skeletonId)
    {
        int remappedOldSkeletonId = skeletonId;

        if (remappedOldSkeletonId < 0)
        {
            remappedOldSkeletonId = -remappedOldSkeletonId;
        }

        uint newSkeletonId        = (uint)remappedOldSkeletonId;
        SkeletonType skeletonType = SkeletonType.Invalid;

        if (skeletonId < -1)
        {
            skeletonType = SkeletonType.BattlePet;
        }
        else if (skeletonId > -1)
        {
            skeletonType = SkeletonType.Minion;
        }

        return new PetSkeleton(newSkeletonId, skeletonType);
    }

    public static PetSkeleton[] AsPetSkeletons(this int[] skeletonArray)
    {
        int arrayLength = skeletonArray.Length;

        PetSkeleton[] newSkeletons = new PetSkeleton[arrayLength];        

        for (int i = 0; i < arrayLength; i++)
        {
            int oldSkeletonId         = skeletonArray[i];
            int remappedOldSkeletonId = oldSkeletonId;

            if (remappedOldSkeletonId < 0)
            {
                remappedOldSkeletonId = -remappedOldSkeletonId;
            }

            uint         skeletonId   = (uint)remappedOldSkeletonId;
            SkeletonType skeletonType = SkeletonType.Invalid;

            if (oldSkeletonId < -1)
            {
                skeletonType = SkeletonType.BattlePet;
            }
            else if (oldSkeletonId > -1)
            {
                skeletonType = SkeletonType.Minion;
            }

            newSkeletons[i] = new PetSkeleton(skeletonId, skeletonType);
        }

        return newSkeletons;
    }
}
