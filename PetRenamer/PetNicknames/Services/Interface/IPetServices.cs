using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Update.Interfaces;
using System;

namespace PetRenamer.PetNicknames.Services.Interface;

internal interface IPetServices : IDisposable
{
    IPetLog          PetLog          { get; }
    IPetSheets       PetSheets       { get; }
    IStringHelper    StringHelper    { get; }
    IPetCastHelper   PetCastHelper   { get; }
    IPetActionHelper PetActionHelper { get; }
    Configuration    Configuration   { get; }
    ITargetManager   TargetManager   { get; }
    IPluginWatcher   PluginWatcher   { get; }
}
