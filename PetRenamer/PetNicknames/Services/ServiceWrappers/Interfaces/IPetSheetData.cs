using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IPetSheetData
{
    PetSkeleton Model         { get; }
    uint        Icon          { get; }

    string      Singular      { get; }

    sbyte       Pronoun       { get; }
    
    uint        RaceId        { get; }
    string?     RaceName      { get; }
    string?     BehaviourName { get; }

    string      ActionName    { get; }
    uint        ActionId      { get; }

    int         LegacyModelId { get; }

    bool        IsPet(string name);
    bool        IsAction(uint action);
}
