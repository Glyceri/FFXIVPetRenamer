using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using System.Diagnostics.CodeAnalysis;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.LanguageBased.Values;

internal class NameTypeValue : LanguageBasedValue<NameType>
{
    [SetsRequiredMembers]
    public NameTypeValue()
        : base(NameType.Raw) { }
}