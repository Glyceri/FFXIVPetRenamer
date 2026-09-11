using PetRenamer.PetNicknames.Services.ServiceWrappers.Attributes;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using System;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

internal readonly struct PetSkeleton : IEquatable<PetSkeleton>
{
    public readonly uint         LeadingSkeletonId;
    public readonly uint[]       MirageSkeletonIds;
    public readonly SkeletonType SkeletonType;

    public PetSkeleton(int skeletonId, SkeletonType skeletonType)
        : this((uint)skeletonId, skeletonType)
        { }

    public PetSkeleton(uint skeletonId, SkeletonType skeletonType)
        : this(skeletonId, skeletonType, [])
    { }
    
    public PetSkeleton(uint leadingSkeletonId, SkeletonType skeletonType, uint[] mirageSkeletonIds)
    {
        LeadingSkeletonId = leadingSkeletonId;
        MirageSkeletonIds = mirageSkeletonIds;
        SkeletonType      = skeletonType;
    }

    public static PetSkeleton CreateInvalid()
        => new PetSkeleton(0, SkeletonType.Invalid);

    public static bool operator ==(PetSkeleton left, PetSkeleton right)
        => left.LeadingSkeletonId == right.LeadingSkeletonId && left.SkeletonType == right.SkeletonType;

    public static bool operator !=(PetSkeleton left, PetSkeleton right)
        => left.LeadingSkeletonId != right.LeadingSkeletonId || left.SkeletonType != right.SkeletonType;

    public bool Equals(PetSkeleton other)
        => this == other;

    public override bool Equals(object? obj)
        => obj is PetSkeleton other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(LeadingSkeletonId, SkeletonType);

    public override string ToString()
        => (SkeletonType.GetAttributeOfType<SkeletonTypeSymbolAttribute>()?.Symbol ?? $"{SkeletonType}: ") + $"{LeadingSkeletonId}";
}
