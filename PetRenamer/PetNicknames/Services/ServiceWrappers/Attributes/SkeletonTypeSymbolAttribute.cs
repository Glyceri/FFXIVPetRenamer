using System;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class SkeletonTypeSymbolAttribute(char symbolIdentifier) : Attribute
{
    public readonly string Symbol     = symbolIdentifier.ToString();
    public readonly char   SymbolChar = symbolIdentifier;
}