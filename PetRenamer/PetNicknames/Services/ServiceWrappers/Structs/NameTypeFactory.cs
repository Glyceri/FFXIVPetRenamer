using Dalamud.Game;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

internal readonly unsafe struct NameTypeFactory
{
    public required NameType EnglishNameType  { get; init; }
    public required NameType GermanNameType   { get; init; }
    public required NameType FrenchNameType   { get; init; }
    public required NameType JapaneseNameType { get; init; }
    
    public static implicit operator NameType(NameTypeFactory nameTypeFactory) 
        => (ClientLanguage)Framework.Instance()->ClientLanguage switch
    {
        ClientLanguage.English  => nameTypeFactory.EnglishNameType,
        ClientLanguage.German   => nameTypeFactory.GermanNameType,
        ClientLanguage.French   => nameTypeFactory.FrenchNameType,
        ClientLanguage.Japanese => nameTypeFactory.JapaneseNameType,
        _                       => nameTypeFactory.EnglishNameType,
    };
}