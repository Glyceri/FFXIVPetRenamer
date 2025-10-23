using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;

namespace PetRenamer.PetNicknames.Services.Interface;

internal interface IPetServices : IDisposable
{
    public IPetLog          PetLog          { get; }
    public IPetSheets       PetSheets       { get; }
    public IStringHelper    StringHelper    { get; }
    public IPetCastHelper   PetCastHelper   { get; }
    public IPetActionHelper PetActionHelper { get; }
    public Configuration    Configuration   { get; }
    public ITargetManager   TargetManager   { get; }
    public IPluginWatcher   PluginWatcher   { get; }
}
