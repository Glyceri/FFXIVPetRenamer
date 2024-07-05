﻿namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IPetSheetData
{
    int Model { get;  }
    uint Icon { get; }
    sbyte Pronoun { get; }

    string[] Singular { get; }
    public string[] Plural { get; }

    string BaseSingular { get; }
    string BasePlural { get;  }

    string ActionName { get;  }
    uint ActionID { get; }

    int LegacyModelID { get; }

    bool IsPet(string name);
    bool IsAction(string action);
    bool Contains(string line);

    string LongestIdentifier();
}
