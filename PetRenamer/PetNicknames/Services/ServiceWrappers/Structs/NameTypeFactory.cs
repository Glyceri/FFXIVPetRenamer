using Lumina.Data;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

internal readonly struct NameTypeFactory
{
    public NameType EnglishNameType            { get; init; } = NameType.Raw;
    public NameType GermanNameType             { get; init; } = NameType.Raw;
    public NameType FrenchNameType             { get; init; } = NameType.Raw;
    public NameType JapaneseNameType           { get; init; } = NameType.Raw;
    public NameType ChineseSimplifiedNameType  { get; init; } = NameType.Raw;
    public NameType ChineseTraditionalNameType { get; init; } = NameType.Raw;
    public NameType KoreanNameType             { get; init; } = NameType.Raw;
    public NameType TaiwaneseNameType          { get; init; } = NameType.Raw;
    
    public NameType GetNameType(DalamudServices dalamudServices)
        => dalamudServices.DataManager.GameData.Options.DefaultExcelLanguage switch
        {
            Language.Japanese           => JapaneseNameType,
            Language.English            => EnglishNameType,
            Language.French             => FrenchNameType,
            Language.German             => GermanNameType,
            Language.Korean             => KoreanNameType,
            Language.TraditionalChinese => TaiwaneseNameType,
            Language.ChineseSimplified  => ChineseSimplifiedNameType,
            Language.ChineseTraditional => ChineseTraditionalNameType,
            _                           => EnglishNameType,
        };
    
    public NameTypeFactory()
        { }
}