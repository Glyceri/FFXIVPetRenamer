using Dalamud.Utility;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Attributes;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using System;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

public readonly struct PetSkeleton : IEquatable<PetSkeleton>
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

    public override string ToString()
        => (SkeletonType.GetAttributeOfType<SkeletonTypeSymbolAttribute>()?.Symbol ?? $"{SkeletonType}: ") + $"{SkeletonId}";
    
    public static PetSkeleton? Parse(string input)
    {
        if (input.IsNullOrWhitespace())
        {
            return CreateInvalid();
        }
        
        if (input.Length < 2)
        {
            return CreateInvalid();
        }
        
        char identifierChar = input[0];
        
        SkeletonType foundType = SkeletonType.Invalid;
        
        for (int i = 0; i < (int)SkeletonType.COUNT; i++)
        {
            SkeletonType skeletonType = (SkeletonType)i;
            
            char? symbol = skeletonType.GetAttributeOfType<SkeletonTypeSymbolAttribute>()?.SymbolChar;
            
            if (symbol == null)
            {
                continue;
            }
            
            if (symbol != identifierChar)
            {
                continue;
            }
            
            foundType = skeletonType;
           
            input = input.Remove(0, 1);
            
            break;
        }
        
        if (foundType == SkeletonType.Invalid)
        {
            return CreateInvalid();
        }
        
        if (!uint.TryParse(input, out uint value))
        {
            return CreateInvalid();
        } 
        
        return new PetSkeleton(value, foundType);
    }
}
