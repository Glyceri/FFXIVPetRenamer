namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

using Lumina.Data;

internal class LanguageBasedFactory<T>
    where T : struct
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
    
    public LanguageBasedFactory()
        { }
}