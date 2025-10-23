using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using System;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

public readonly struct PetSkeleton
{
    public readonly uint         SkeletonId;
    public readonly SkeletonType SkeletonType;

    public PetSkeleton(int skeletonId, SkeletonType skeletonType)
        : this((uint)skeletonId, skeletonType) { }

    public PetSkeleton(uint skeletonId, SkeletonType skeletonType)
    {
        SkeletonId   = skeletonId;
        SkeletonType = skeletonType;
    }

    public static PetSkeleton CreateInvalid()
        => new PetSkeleton(0, SkeletonType.Invalid);

    public static bool operator ==(PetSkeleton left, PetSkeleton right)
        => left.SkeletonId == right.SkeletonId && left.SkeletonType == right.SkeletonType;

    public static bool operator !=(PetSkeleton left, PetSkeleton right)
        => left.SkeletonId != right.SkeletonId || left.SkeletonType != right.SkeletonType;

    public bool Equals(PetSkeleton other)
        => SkeletonId == other.SkeletonId && SkeletonType == other.SkeletonType;

    public override bool Equals(object? obj)
        => obj is PetSkeleton other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(SkeletonId, SkeletonType);
}
