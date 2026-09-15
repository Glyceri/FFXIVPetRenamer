using PetRenamer.PetNicknames.PettableUsers.Structs;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

internal readonly struct PetSheetData : IPetSheetData
{
    public PetSkeleton      Model         { get; }
    public uint             Icon          { get; }
    
    public string           Singular      { get; }

    public sbyte            Pronoun       { get; }
    
    public string           ActionName    { get; } = string.Empty;
    public ActionData       ActionData    { get; }

    public int              LegacyModelId { get; }

    public uint             RaceId        { get; } = 0;
    public string?          RaceName      { get; } = null;
    public string?          BehaviourName { get; } = null;

    public PetSheetData(PetSkeleton model, int legacyModelId, uint icon, string? raceName, uint raceId, string? behaviourName, sbyte pronoun, string singular, string actionName, ActionData actionData)
        : this(model, legacyModelId, icon, pronoun, singular, actionName, actionData)
    {
        RaceName      = raceName;
        BehaviourName = behaviourName;
        RaceId        = raceId;
    }

    private PetSheetData(PetSkeleton model, int legacyModelId, uint icon, sbyte pronoun, string singular, string actionName, ActionData actionData)
        : this(model, icon, pronoun, singular)
    {
        ActionData    = actionData;
        ActionName    = actionName;
        LegacyModelId = legacyModelId;
    }

    private PetSheetData(PetSkeleton model, uint icon, sbyte pronoun, string singular)
    {
        Model         = model;
        Icon          = icon;
        Pronoun       = pronoun;
        Singular      = singular;
    }

    public PetSheetData MakeSoft(IPetSheetData newData)
        => new PetSheetData(newData.Model, newData.LegacyModelId, newData.Icon, newData.Pronoun, Singular, ActionName, ActionData);

    public bool IsPet(string name)
        => Singular.InvariantEquals(name);

    public bool IsAction(ActionData action) 
        => (ActionData == action);
}
