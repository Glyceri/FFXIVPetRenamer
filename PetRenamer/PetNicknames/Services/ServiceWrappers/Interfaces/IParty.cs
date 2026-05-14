using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IParty : IDisposable, IEnumerable<IPettableUser?>
{
    const int MaxPartyLength = 8;
    
    int Length { get; }
    
    IPettableUser? this[int index] { get; }
}