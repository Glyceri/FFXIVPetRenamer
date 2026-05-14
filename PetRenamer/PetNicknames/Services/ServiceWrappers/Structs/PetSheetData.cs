using Dalamud.Game;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

internal struct PetSheetData : IPetSheetData
{
    public PetSkeleton  Model         { get; }
    public uint         Icon          { get; }
    
    public string       BaseSingular  { get; }
    public string       BasePlural    { get; }

    public sbyte        Pronoun       { get; }
    
    public string       ActionName    { get; } = string.Empty;
    public uint         ActionId      { get; } = 0;

    public int          LegacyModelId { get; }

    public uint         RaceId        { get; } = 0;
    public string?      RaceName      { get; } = null;
    public string?      BehaviourName { get; } = null;
    
    private static readonly string[] pronounList = ["er", "e", "es", "en"];

    public PetSheetData(PetSkeleton model, int legacyModelId, uint icon, string? raceName, uint raceId, string? behaviourName, sbyte pronoun, string singular, string plural, string actionName, uint actionId, DalamudServices services)
        : this(model, legacyModelId, icon, pronoun, singular, plural, actionName, actionId, services)
    {
        RaceName      = raceName;
        BehaviourName = behaviourName;
        RaceId        = raceId;
    }

    public PetSheetData(PetSkeleton model, int legacyModelId, uint icon, sbyte pronoun, string singular, string plural, string actionName, uint actionId, DalamudServices services)
        : this(model, icon, pronoun, singular, plural, services)
    {
        ActionId      = actionId;
        ActionName    = actionName;
        LegacyModelId = legacyModelId;
    }

    public PetSheetData(PetSkeleton model, uint icon, sbyte pronoun, string singular, string plural, DalamudServices services)
    {
        Model   = model;
        Icon    = icon;
        Pronoun = pronoun;

        ClientLanguage clientLanguage = services.ClientState.ClientLanguage;

        if (clientLanguage == ClientLanguage.German)
        {
            BaseSingular = GermanReplace(singular, pronoun);
            BasePlural   = GermanReplace(plural, pronoun);
        }
        else
        {
            BaseSingular = singular;
            BasePlural   = plural;
        }
    }
    
    private readonly string GermanReplace(string baseString, sbyte pronoun)
    {
        if (pronoun < 0 || pronoun >= pronounList.Length)
        {
            return baseString;
        }

        baseString = baseString.InvariantReplace("[p]", string.Empty);
        baseString = baseString.InvariantReplace("[a]", pronounList[pronoun]);   

        return baseString;
    }

    public readonly bool IsPet(string name)
        => BaseSingular.InvariantEquals(name);

    public readonly string LongestIdentifier()
    {
        string curIdentifier = BaseSingular;

        if (curIdentifier.Length < BasePlural.Length)
        {
            curIdentifier = BasePlural;
        }

        if (curIdentifier.Length < ActionName.Length)
        {
            curIdentifier = ActionName;
        }

        return curIdentifier;
    }

    public readonly bool IsAction(uint action) 
        => (ActionId == action);
}
