using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;

namespace PetRenamer.PetNicknames.Services.Interface;

internal interface IPetServices : IDisposable
{
    string               Version             { get; }
    IPetLog              PetLog              { get; }
    IPetSheets           PetSheets           { get; }
    IUserList            UserList            { get; }
    IStringHelper        StringHelper        { get; }
    IPetCastHelper       PetCastHelper       { get; }
    Configuration        Configuration       { get; }
    ITargetManager       TargetManager       { get; }
    IPluginWatcher       PluginWatcher       { get; }
    INotificationService NotificationService { get; }
    INameService         NameService         { get; }
    IHoverService        HoverService        { get; }
    IDirtyCaller         DirtyCaller         { get; }
    IDirtyListener       DirtyListener       { get; }
    IParty               Party               { get; }
}
