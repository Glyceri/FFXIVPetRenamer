using System.Diagnostics.CodeAnalysis;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.LanguageBased.Values;

internal class EmptySpaceValue : LanguageBasedValue<bool>
{
    [SetsRequiredMembers]
    public EmptySpaceValue()
        : base(false) { }
}