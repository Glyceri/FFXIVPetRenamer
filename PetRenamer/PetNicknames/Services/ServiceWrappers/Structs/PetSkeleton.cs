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

    public PetSkeleton(SkeletonType skeletonType, int skeletonId)
        : this(skeletonType, (uint)skeletonId)
        { }

    public PetSkeleton(SkeletonType skeletonType, uint skeletonId)
        : this(skeletonType, skeletonId, [])
        { }
    
    public PetSkeleton(SkeletonType skeletonType, uint leadingSkeletonId, uint[] mirageSkeletonIds)
        : this(skeletonType, [leadingSkeletonId, ..mirageSkeletonIds])
        { }

    public PetSkeleton(SkeletonType skeletonType, uint[] skeletonIds)
    {
        if (skeletonIds.Length == 0)
        {
            throw new ArgumentException("skeletonIds must contain at least one ID.", nameof(skeletonIds));
        }
        
        LeadingSkeletonId = skeletonIds[0];
        SkeletonType      = skeletonType;
        MirageSkeletonIds = skeletonIds[1..];
    }
    
    public static PetSkeleton CreateInvalid()
        => new PetSkeleton(SkeletonType.Invalid, 0);

    public static bool operator ==(PetSkeleton left, PetSkeleton right)
        => ComparisonCheck(left, right);

    public static bool operator !=(PetSkeleton left, PetSkeleton right)
        => !ComparisonCheck(left, right);

    public bool Equals(PetSkeleton other)
        => this == other;

    public override bool Equals(object? obj)
        => obj is PetSkeleton other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(LeadingSkeletonId, SkeletonType);
    
    private static bool IsInMirage(PetSkeleton left, PetSkeleton right)
    {
        foreach (uint mirageId in left.MirageSkeletonIds)
        {
            if (right.LeadingSkeletonId != mirageId)
            {
                continue;
            }
            
            return true;
        }
        
        return false;
    }
    
    private static bool ComparisonCheck(PetSkeleton left, PetSkeleton right)
    {
        if (left.SkeletonType != right.SkeletonType)
        {
            return false;
        }
        
        if (left.LeadingSkeletonId == right.LeadingSkeletonId)
        {
            return true;
        }
        
        if (IsInMirage(left, right))
        {
            return true;
        }
        
        if (IsInMirage(right, left))
        {
            return true;
        }
        
        return false;
    }
    
    public override string ToString()
    {
        string newString = (SkeletonType.GetAttributeOfType<SkeletonTypeSymbolAttribute>()?.Symbol ?? $"{SkeletonType}:") + $" [{LeadingSkeletonId}]";
        
        if (MirageSkeletonIds.Length > 0)
        {
            newString += $", [{string.Join(", ", MirageSkeletonIds)}]";
        }
        
        return newString;
    }
}
