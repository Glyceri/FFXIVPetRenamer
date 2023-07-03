using System;

namespace PetRenamer.Commands.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PetCommandAttribute : Attribute
{
    public string command = string.Empty;
    public string description = string.Empty;
    public bool showInHelp = true;

    public PetCommandAttribute(string command, string description, bool showInHelp)
    {
        this.command = command;
        this.description = description;
        this.showInHelp = showInHelp;
    }
}
