using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;

namespace PetRenamer.PetNicknames.Services.Interface;

internal interface IPetServices : IDisposable
{
    IPetLog              PetLog              { get; }
    IPetSheets           PetSheets           { get; }
    IStringHelper        StringHelper        { get; }
    IPetCastHelper       PetCastHelper       { get; }
    Configuration        Configuration       { get; }
    ITargetManager       TargetManager       { get; }
    IPluginWatcher       PluginWatcher       { get; }
    INotificationService NotificationService { get; }
    INameService         NameService         { get; }
    IHoverService        HoverService        { get; }
    IParty               Party               { get; }
}
