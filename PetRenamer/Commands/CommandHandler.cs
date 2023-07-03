using Dalamud.Game.Command;
using PetRenamer.Commands.Attributes;
using PetRenamer.Core.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PetRenamer.Commands
{
    internal class CommandHandler
    {
        List<PetCommand> commands = new List<PetCommand>();

        public CommandHandler()
        {
            Type petCommandType = typeof(PetCommand);
            Assembly petCommandAssembly = Assembly.GetAssembly(petCommandType)!;
            Type[] petCommandInheritedTypes = petCommandAssembly.GetTypes().Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.IsSubclassOf(typeof(PetCommand)) &&
                t.GetCustomAttribute<PetCommandAttribute>() != null)
            .ToArray();

            foreach (Type type in petCommandInheritedTypes)
            {
                PetCommand pCommand = (PetCommand)Activator.CreateInstance(type)!;
                PetCommandAttribute attribute = pCommand.GetType().GetCustomAttribute<PetCommandAttribute>()!;
                PluginHandlers.CommandManager.AddHandler(attribute.command, new CommandInfo(pCommand.OnCommand) { HelpMessage = attribute.description, ShowInHelp = attribute.showInHelp });
               commands.Add(pCommand);
            }
        }

        public void ClearAllCommands()
        {
            foreach(PetCommand command in commands)
            {
                PetCommandAttribute attribute = command.GetType().GetCustomAttribute<PetCommandAttribute>()!;
                PluginHandlers.CommandManager.RemoveHandler(attribute.command);
            }
            commands.Clear();
        }
    }
}
