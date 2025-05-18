using Dalamud.Utility;
using PetRenamer.PetNicknames.Commands.Commands.Base;
using PetRenamer.PetNicknames.Commands.Exceptions;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PetRenamer.PetNicknames.Commands.Commands;

internal partial class PetnameCommand : Command
{
    private readonly IPetServices      PetServices;
    private readonly IPettableUserList UserList;
    private readonly IPettableDatabase Database;

    public PetnameCommand(DalamudServices dalamudServices, IWindowHandler windowHandler, IPetServices petServices, IPettableUserList userList, IPettableDatabase database) : base(dalamudServices, windowHandler) 
    {
        PetServices = petServices;
        UserList    = userList;
        Database    = database;
    }

    public override string CommandCode { get; } = "/petname";
    public override string Description { get; } = Translator.GetLine("Command.Petname");
    public override bool   ShowInHelp { get; }  = true;

    private const string        CUSTOM_TAG          = "[custom]";
    private       string        lastCommandPart     = string.Empty;
    private       List<string>  matchedArguments    = [];
    private       int           customCounter       = 0;

    public override void OnCommand(string command, string args)
    {
        if (args.IsNullOrWhitespace())
        {
            WindowHandler.Open<PetRenameWindow>();
            return;
        }

        args += " ";

        ClearLast();
        PrepareArguments(ref args);

        string[] sanitizedArgs = GetArguments(args);

        try
        {
            if (sanitizedArgs.Length == 0)
            {
                ThrowArgumentException();
            }

            ProcessCommand(sanitizedArgs);
        }
        catch(PetNicknamesCommandArgumentException commandException)
        {
            if (!PetServices.Configuration.showCommandFeedback)
            {
                return;
            }

            DalamudServices.ChatGui.PrintError(commandException.Message);
        }
    }

    private void ProcessCommand(string[] commandArgs)
    {
        CommandState currentState = ParseCommandState(commandArgs);

        if (currentState == CommandState.INVALID)
        {
            throw new PetNicknamesCommandArgumentException($"'{lastCommandPart}' is invalid syntax. Use: set|clear|help");
        }
        else if (currentState == CommandState.Help)
        {
            PrintHelp();
            return;
        }

        TargetState targetState = ParseTargetState(commandArgs);

        if (targetState == TargetState.INVALID)
        {
            throw new PetNicknamesCommandArgumentException($"'{lastCommandPart}' is invalid syntax. Use: target|minion|pet|\"PETNAME\"|\"0000\"");
        }
        else if (targetState == TargetState.Custom)
        {
            customCounter++;
        }

        IPettableDatabaseEntry? targetedEntry = GetEntry(targetState);

        if (currentState == CommandState.Set)
        {
            HandleSet(commandArgs, targetState);
        }
        else if (currentState == CommandState.Clear)
        {
            HandleClear(commandArgs, targetState);
        }
    }

    private IPettableDatabaseEntry? GetEntry(TargetState targetState)
    {
        if (targetState == TargetState.Target)
        {
            IPettableEntity? target = PetServices.TargetManager.Target;

            if (target == null)
            {
                ThrowTargetNotFoundException();
            }

            IPettableUser? localUser = UserList.LocalPlayer;

            if (localUser == null)
            {
                ThrowTargetNotFoundException();
            }

            IPettablePet? pet = localUser!.GetPet(target!.Address);

            if (pet == null)
            {
                ThrowTargetNotFoundException();
            }

            return pet!.
        }

        return null;
    }

    private void HandleSet(string[] commandArgs, TargetState targetState)
    {
        
    }

    private void HandleClear(string[] commandArgs, TargetState targetState)
    {

    }

    private CommandState ParseCommandState(string[] commandArgs)
    {
        string part = GetCommandPart(commandArgs, 0);

        return part switch
        {
            "set"   => CommandState.Set,
            "clear" => CommandState.Clear,
            "help"  => CommandState.Help,
            _       => CommandState.INVALID,
        };
    }

    private TargetState ParseTargetState(string[] commandArgs)
    {
        string part = GetCommandPart(commandArgs, 1);

        return part switch
        {
            CUSTOM_TAG  => TargetState.Custom,
            "minion"    => TargetState.Minion,
            "pet"       => TargetState.BattlePet,
            "target"    => TargetState.Target,
            _           => TargetState.INVALID,
        };
    }

    private void PrintHelp()
    {
        DalamudServices.ChatGui.Print
        (
            "Pet Rename Help: \n" +
            "\n" +
            "A Pet Rename command is build as follows:\n" +
            "    /petname [action] [target selector]\n" +
            "    /petname [action] [target selector] [nickname]\n" +
            "\n" +
            "Example command: /petname set \"Hedgehoglet\" \"George\"\n" +
            "Or: /petname clear \"Hedgehoglet\"" +
            "\n" +
            "[action]\n" +
            "    set\n" +
            "        Specifies that you want to set the nickname to a new nickname.\n" +
            "    clear\n" +
            "        Clears the nickname of the specified target.\n" +
            "    help\n" +
            "        Displays help information.\n" +
            "\n" +
            "[target selector]\n" +
            "    \"PET_NAME\"\n" +
            "        Type the name of the pet (this is NOT case sensitive, but IS client language sensitive).\n" +
            "    0000\n" +
            "        The ID of a minion or battle pet. These can be found in the /petlist or in the games build in sheets. (Battle pets require the - marker)\n" +
            "    minion\n" +
            "        Your currently used minion.\n" +
            "    pet\n" +
            "        Your currently used pet (Think Carbuncle, Faerie, Esteem).\n" +
            "    target\n" +
            "        Your current target (Note you can only nickname your OWN pets and minions).\n" +
            "\n" +
            "[nickname]\n" +
            "    \"NICKNAME\"\n" +
            "        The new nickname the pet will adopt.\n"
        );
    }

    private void ClearLast()
    {
        lastCommandPart = string.Empty;
        matchedArguments.Clear();
        customCounter = 0;
    }

    private void PrepareArguments(ref string arguments)
    {
        // Finds arguments with a " in the param and sets them to CUSTOM_TAG, then stores them
        foreach (Match match in QuotationRegex().Matches(arguments))
        {
            string matchString = match.Value;

            matchedArguments.Add(matchString.Replace("\"", ""));

            arguments = arguments.Replace(matchString, CUSTOM_TAG);
        }
    }

    [GeneratedRegex("(\".+?\")")]
    private partial Regex QuotationRegex();

    private string[] GetArguments(string args)
    {
        args = args.ToLower();

        return args.Split(' ');
    }

    private void ThrowArgumentException()
    {
        throw new PetNicknamesCommandArgumentException("Missing arguments. Use \"/petname help\" for more information.");
    }

    private void ThrowTargetNotFoundException()
    {
        throw new PetNicknamesCommandArgumentException("Target could not be found.");
    }

    private string GetCommandPart(string[] commandArgs, int location)
    {
        if (commandArgs.Length <= location)
        {
            ThrowArgumentException();
        }

        lastCommandPart = commandArgs[location];

        return lastCommandPart;
    }

    private enum CommandState
    {
        INVALID,
        Set,
        Clear,
        Help
    }

    private enum TargetState
    { 
        INVALID,
        Minion,
        BattlePet,
        Target,
        Custom
    }
}
