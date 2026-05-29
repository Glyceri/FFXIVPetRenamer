using Dalamud.Game;
using Dalamud.Utility;
using Lumina.Excel.Sheets;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

internal struct PetSheetData : IPetSheetData
{
    public PetSkeleton  Model         { get; }
    public uint         Icon          { get; }
    
    public string       Singular  { get; }

    public sbyte        Pronoun       { get; }
    
    public string       ActionName    { get; } = string.Empty;
    public uint         ActionId      { get; } = 0;

    public int          LegacyModelId { get; }

    public uint         RaceId        { get; } = 0;
    public string?      RaceName      { get; } = null;
    public string?      BehaviourName { get; } = null;
    
    private static readonly string[] pronounList = ["er", "e", "es", "en"];

    public PetSheetData(PetSkeleton model, int legacyModelId, uint icon, string? raceName, uint raceId, string? behaviourName, sbyte pronoun, string singular, string actionName, uint actionId, DalamudServices services)
        : this(model, legacyModelId, icon, pronoun, singular, actionName, actionId, services)
    {
        RaceName      = raceName;
        BehaviourName = behaviourName;
        RaceId        = raceId;
    }

    public PetSheetData(PetSkeleton model, int legacyModelId, uint icon, sbyte pronoun, string singular, string actionName, uint actionId, DalamudServices services)
        : this(model, icon, pronoun, singular, services)
    {
        ActionId      = actionId;
        ActionName    = actionName;
        LegacyModelId = legacyModelId;
    }

    private PetSheetData(PetSkeleton model, uint icon, sbyte pronoun, string singular, DalamudServices services)
    {
        Model   = model;
        Icon    = icon;
        Pronoun = pronoun;

        ClientLanguage clientLanguage = services.ClientState.ClientLanguage;

        if (clientLanguage == ClientLanguage.German)
        {
            Singular = GermanReplace(singular, pronoun);
        }
        else
        {
            Singular = singular;
        }
    }
    
    public static PetSheetData? CreatePetSheetData(DalamudServices dalamudServices, Companion companion)
    {
        if (!companion.Model.IsValid)
        {
            return null;
        }

        ModelChara? model = companion.Model.ValueNullable;

        if (model == null)
        {
            return null;
        }

        uint        companionIndex = companion.RowId;
        int         modelId        = (int)model.Value.RowId;
        int         legacyModelId  = model.Value.Model;
        string      singular       = companion.Singular.ExtractText();
        string      plural         = companion.Plural.ExtractText();
        PetSkeleton petSkeleton    = new PetSkeleton((uint)modelId, SkeletonType.Minion);
        
        if (legacyModelId == 0)
        {
            return null;
        }
        
        if (singular.IsNullOrWhitespace() || plural.IsNullOrWhitespace())
        {
            return null;
        }

        singular = singular.ToTitleCase();
        
        uint   icon          = companion.Icon;
        sbyte  pronoun       = companion.Pronoun;
        uint   raceId        = companion.MinionRace.ValueNullable?.RowId ?? 0;
        string raceName      = companion.MinionRace.ValueNullable?.Name.ExtractText() ?? string.Empty;
        string behaviourName = companion.Behavior.ValueNullable?.Name.ExtractText() ?? string.Empty;
        
        return new PetSheetData(petSkeleton, legacyModelId, icon, raceName, raceId, behaviourName, pronoun, singular, singular, companionIndex, dalamudServices);
    }
    
    public static PetSheetData? CreatePetSheetData(IPetSheets petSheets, DalamudServices dalamudServices, Pet pet)
    {
        uint sheetSkeleton = pet.RowId;

        if (!PluginConstants.BattlePetRemap.TryGetValue(sheetSkeleton, out PetSkeleton skeleton))
        {
            return null;
        }

        if (!PluginConstants.PetIdToAction.TryGetValue(skeleton, out uint actionId))
        {
            return null;
        }

        if (!PluginConstants.BattlePetToBNpcName.TryGetValue(skeleton, out uint bnpcNameId))
        {
            return null;
        }

        BNpcName? bnpcName = petSheets.GetBNpcName(bnpcNameId);

        if (bnpcName == null)
        {
            return null;
        }
        
        Action? petAction = petSheets.GetAction(actionId);

        if (petAction == null)
        {
            return null;
        }

        ushort petIcon           = petAction.Value.Icon;
        string name              = bnpcName.Value.Singular.ExtractText().ToTitleCase();
        string actionName        = petAction.Value.Name.ExtractText();
        uint   actionRowId       = petAction.Value.RowId;
        
        return new PetSheetData(skeleton, -1, petIcon, bnpcName.Value.Pronoun, name, actionName, actionRowId, dalamudServices);
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
        => Singular.InvariantEquals(name);

    public readonly bool IsAction(uint action) 
        => (ActionId == action);
}
