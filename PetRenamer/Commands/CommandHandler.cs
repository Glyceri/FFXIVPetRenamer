﻿using Dalamud.Game.Command;
using PetRenamer.Commands.Attributes;
using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Handlers;
using System.Collections.Generic;
using System.Reflection;

namespace PetRenamer.Commands;

internal class CommandHandler : RegistryBase<PetCommand, PetCommandAttribute>
{
    internal List<PetCommand> commands => elements;

    protected override void OnElementCreation(PetCommand element)
    {
        PetCommandAttribute attribute = element.GetType().GetCustomAttribute<PetCommandAttribute>()!;
        PluginHandlers.CommandManager.AddHandler(attribute.command, new CommandInfo(element.OnCommand) { HelpMessage = attribute.description, ShowInHelp = attribute.showInHelp });
        foreach(string extraCommand in attribute.extraCommands)
            PluginHandlers.CommandManager.AddHandler(extraCommand, new CommandInfo(element.OnCommand) { HelpMessage = attribute.description, ShowInHelp = false });
    }

    protected override void OnElementDestroyed(PetCommand element)
    {
        PetCommandAttribute attribute = element.GetType().GetCustomAttribute<PetCommandAttribute>()!;
        PluginHandlers.CommandManager.RemoveHandler(attribute.command);
        foreach (string extraCommand in attribute.extraCommands)
            PluginHandlers.CommandManager.RemoveHandler(extraCommand);
    }

    public void ClearAllCommands() => ClearAllElements();
}
