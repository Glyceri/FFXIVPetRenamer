using Lumina.Data;
using System.Diagnostics.CodeAnalysis;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.LanguageBased;

internal class LanguageBasedValue<T>
{
    public T EnglishNameType            { get; init; }
    public T GermanNameType             { get; init; }
    public T FrenchNameType             { get; init; }
    public T JapaneseNameType           { get; init; }
    public T ChineseSimplifiedNameType  { get; init; }
    public T ChineseTraditionalNameType { get; init; }
    public T KoreanNameType             { get; init; }
    public T TaiwaneseNameType          { get; init; }
    
    public T GetValue(DalamudServices dalamudServices)
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
    
    [SetsRequiredMembers]
    public LanguageBasedValue(T defaultTValue)
    {
        EnglishNameType            = defaultTValue;
        GermanNameType             = defaultTValue;
        FrenchNameType             = defaultTValue;
        JapaneseNameType           = defaultTValue;
        ChineseSimplifiedNameType  = defaultTValue;
        ChineseTraditionalNameType = defaultTValue;
        KoreanNameType             = defaultTValue;
        TaiwaneseNameType          = defaultTValue;
        
    }
}