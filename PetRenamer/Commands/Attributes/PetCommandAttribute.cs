using PetRenamer.Core.Attributes;
using System;

namespace PetRenamer.Commands.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PetCommandAttribute : SortableAttribute
{
    public string command = string.Empty;
    public string description = string.Empty;
    public bool showInHelp = true;
    public string[] extraCommands = Array.Empty<string>();

    public PetCommandAttribute(string command, string description, bool showInHelp, int order = 0) : base(order)
    {
        this.command = command;
        this.description = description;
        this.showInHelp = showInHelp;
    }

    public PetCommandAttribute(string command, string description, bool showInHelp, int order = 0, params string[] extraCommands) : base(order)
    {
        this.command = command;
        this.description = description;
        this.showInHelp = showInHelp;
        this.extraCommands = extraCommands;
    }
}
