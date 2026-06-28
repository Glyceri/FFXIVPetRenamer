using System.Diagnostics.CodeAnalysis;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.LanguageBased.Values;

internal class SummonLanguageValue : LanguageBasedValue<string>
{
    [SetsRequiredMembers]
    public SummonLanguageValue()
        : base(PluginConstants.EnglishSummonValue) { }
}