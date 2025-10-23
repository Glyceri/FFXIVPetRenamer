using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IPetSheetData
{
    public PetSkeleton Model         { get;  }
    public uint        Icon          { get; }
    public sbyte       Pronoun       { get; }

    public string[]    Singular      { get; }
    public string[]    Plural        { get; }

    public string      BaseSingular  { get; }
    public string      BasePlural    { get;  }

    public uint        RaceID        { get; }
    public string?     RaceName      { get; }
    public string?     BehaviourName { get; }

    public string      ActionName    { get;  }
    public uint        ActionID      { get; }

    public int         LegacyModelID { get; }

    public bool        IsPet(string name);
    public bool        IsAction(string action);
    public bool        IsAction(uint action);
    public bool        Contains(string line);

    public string      LongestIdentifier();
}
