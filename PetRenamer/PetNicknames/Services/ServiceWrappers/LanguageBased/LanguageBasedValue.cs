using Lumina.Data;
using System.Diagnostics.CodeAnalysis;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.LanguageBased;

internal class LanguageBasedValue<T>
{
    public required T EnglishValue            { get; init; }
    public required T GermanValue             { get; init; }
    public required T FrenchValue             { get; init; }
    public required T JapaneseValue           { get; init; }
    public required T ChineseSimplifiedValue  { get; init; }
    public required T ChineseTraditionalValue { get; init; }
    public required T KoreanValue             { get; init; }
    public required T TaiwaneseValue          { get; init; }
    
    public T GetValue(DalamudServices dalamudServices)
        => dalamudServices.DataManager.GameData.Options.DefaultExcelLanguage switch
        {
            Language.Japanese           => JapaneseValue,
            Language.English            => EnglishValue,
            Language.French             => FrenchValue,
            Language.German             => GermanValue,
            Language.Korean             => KoreanValue,
            Language.TraditionalChinese => TaiwaneseValue,
            Language.ChineseSimplified  => ChineseSimplifiedValue, 
            Language.ChineseTraditional => ChineseTraditionalValue,
            _                           => EnglishValue,
        };
    
    [SetsRequiredMembers]
    public LanguageBasedValue(T defaultTValue)
    {
        EnglishValue            = defaultTValue;
        GermanValue             = defaultTValue;
        FrenchValue             = defaultTValue;
        JapaneseValue           = defaultTValue;
        ChineseSimplifiedValue  = defaultTValue;
        ChineseTraditionalValue = defaultTValue;
        KoreanValue             = defaultTValue;
        TaiwaneseValue          = defaultTValue;
        
    }
}