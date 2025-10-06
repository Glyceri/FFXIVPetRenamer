using Dalamud.Utility;
using PetRenamer.PetNicknames.Commands.Commands.Base;
using PetRenamer.PetNicknames.Commands.Exceptions;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace PetRenamer.PetNicknames.Commands.Commands;

// This shit SUCKS
internal partial class PetnameCommand : Command
{
    private readonly IPetServices      PetServices;
    private readonly IPettableUserList UserList;
    private readonly IPettableDatabase Database;

    private IPettableDatabaseEntry? activeEntry;
    private IPetSheetData?          activeData;

    public PetnameCommand(DalamudServices dalamudServices, IWindowHandler windowHandler, IPetServices petServices, IPettableUserList userList, IPettableDatabase database) : base(dalamudServices, windowHandler) 
    {
        PetServices = petServices;
        UserList    = userList;
        Database    = database;
    }

    public override string CommandCode  { get; }    = "/petname";
    public override string Description  { get; }    = Translator.GetLine("Command.Petname");
    public override bool   ShowInHelp   { get; }    = true;

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

        if (!GetEntry(targetState))
        {
            ThrowTargetNotFoundException();
        }

        if (activeEntry == null)
        {
            ThrowNewNoPlayerFoundException();
        }

        if (activeData == null)
        {
            ThrowTargetNotFoundException();
        }

        if (currentState == CommandState.Set)
        {
            HandleSet(commandArgs, targetState);
        }
        else if (currentState == CommandState.Clear)
        {
            HandleClear(commandArgs, targetState);
        }
    }

    private bool GetEntry(TargetState targetState)
    {
        IPettableUser? localUser = UserList.LocalPlayer;

        if (localUser == null)
        {
            return false;
        }

        activeEntry = localUser!.DataBaseEntry;

        if (targetState == TargetState.Target)
        {
            IPettableEntity? target = PetServices.TargetManager.LeadingTarget;

            if (target == null)
            {
                return false;
            }

            IPettablePet? pet = localUser!.GetPet(target!.Address);

            if (pet == null)
            {
                return false;
            }

            activeData = pet!.PetData;

            return true;
        }
        else if (targetState == TargetState.Minion)
        {
            IPettablePet? pet = localUser!.GetYoungestPet(IPettableUser.PetFilter.Minion);

            if (pet == null)
            {
                return false;
            }

            activeData = pet!.PetData;

            return true;
        }
        else if (targetState == TargetState.BattlePet)
        {
            IPettablePet? pet = localUser!.GetYoungestPet(IPettableUser.PetFilter.BattlePet);

            if (pet == null)
            {
                return false;
            }

            activeData = pet!.PetData;

            return true;
        }
        else if (targetState == TargetState.Custom)
        {
            string? customName = GetCustomString();

            if (customName == null)
            {
                return false;
            }

            if (int.TryParse(customName, out int id))
            {
                IPetSheetData? idData = PetServices.PetSheets.GetPet(id);

                if (idData == null)
                {
                    return false;
                }

                activeData = idData;

                return true;
            }
            else
            {
                IPetSheetData? idData = PetServices.PetSheets.GetPetFromString(customName, localUser);

                if (idData == null)
                {
                    return false;
                }

                activeData = idData;

                return true;
            }
        }

        return false;
    }

    private void HandleSet(string[] commandArgs, TargetState targetState)
    {
        if (activeEntry == null)
        {
            ThrowNewErrorException();
        }

        if (activeData == null)
        {
            ThrowNewErrorException();
        }

        string? currentName         = activeEntry!.GetName(activeData!.Model);
        Vector3? currentEdgeColour  = activeEntry!.GetEdgeColour(activeData!.Model);
        Vector3? currentTextColour  = activeEntry!.GetTextColour(activeData!.Model);

        string? currentEdgeText = null;
        string? currentTextText = null;

        if (currentEdgeColour != null)
        {
            currentEdgeColour   = currentEdgeColour.Value * 255.0f;

            currentEdgeText     = PetServices.StringHelper.ToVector3String(currentEdgeColour.Value);
        }

        if (currentTextColour != null)
        {
            currentTextColour   = currentTextColour.Value * 255.0f;

            currentTextText     = PetServices.StringHelper.ToVector3String(currentTextColour.Value);
        }

        SetState nameSetState = ParseSetState(commandArgs, SetIndex.Name);
        SetState edgeSetState = ParseSetState(commandArgs, SetIndex.EdgeColour);
        SetState textSetState = ParseSetState(commandArgs, SetIndex.TextColour);

        currentName           = HandleSetStateText(currentName,     nameSetState);
        currentEdgeText       = HandleSetStateText(currentEdgeText, edgeSetState);
        currentTextText       = HandleSetStateText(currentTextText, textSetState);

        currentEdgeColour     = PetServices.StringHelper.ParseVector3(currentEdgeText);
        currentTextColour     = PetServices.StringHelper.ParseVector3(currentTextText);

        if (currentEdgeColour != null)
        {
            currentEdgeColour = currentEdgeColour.Value / 255.0f;   // divide by 255
        }

        if (currentTextColour != null)
        {
            currentTextColour = currentTextColour.Value / 255.0f;    // divide by 255
        }

        activeEntry!.ActiveDatabase.SetName(activeData!.Model, currentName, currentEdgeColour, currentTextColour);
    }

    private void HandleClear(string[] commandArgs, TargetState targetState)
    {
        if (activeEntry == null)
        {
            return;
        }

        if (activeData == null)
        {
            return;
        }

        activeEntry.SetName(activeData.Model, string.Empty, null, null);
    }

    private string? HandleSetStateText(string? text, SetState nameSetState)
    {
        string? current = text;

        switch (nameSetState)
        {
            case SetState.Keep:
            {
                // Keep it
                break;
            }

            case SetState.Null:
            {
                current = null;
                break;
            }

            case SetState.Custom:
            {
                current = GetCustomString();

                if (current == null)
                {
                    throw new PetNicknamesCommandArgumentException("You didn't provide a valid argument.");
                }

                if (current.IsNullOrWhitespace())
                {
                    throw new PetNicknamesCommandArgumentException("You didn't provide a valid argument.");
                }

                break;
            }

            case SetState.INVALID:
            default:
            {
                throw new PetNicknamesCommandArgumentException($"'{lastCommandPart}' is invalid syntax. Use: clear|null|keep|waive|\"NAME\"|<xxx,xxx,xxx>");
            }
        }

        return current;
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

    private SetState ParseSetState(string[] commandArgs, SetIndex atIndex)
    {
        int commandPart = atIndex switch
        {
            SetIndex.Name       => 2,
            SetIndex.EdgeColour => 3,
            SetIndex.TextColour => 4,
            _                   => 2
        };

        string part = GetCommandPart(commandArgs, commandPart, false, "keep");

        return part switch
        {
            CUSTOM_TAG  => SetState.Custom,
            "clear"     => SetState.Null,
            "null"      => SetState.Null,
            "keep"      => SetState.Keep,
            "waive"     => SetState.Keep,
            _           => SetState.INVALID,
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
            "    /petname [action] [target selector] [nickname] [edge colour]\n" +
            "    /petname [action] [target selector] [nickname] [edge colour] [text colour]\n" +
            "\n" +
            "Example command: /petname set \"Hedgehoglet\" \"George\"\n" +
            "Or: /petname clear \"Hedgehoglet\"\n" +
            "Or: /petname set minion \"George\" <100, 100, 100>\n" +
            "Or: /petname set target \"George\" keep <255, 255, 255>\n" +
            "Or: /petname set 2442 \"George\" clear keep\n" +
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
            "        The new nickname the pet will adopt.\n" +
            "    clear\n" +
            "        Clears the nickname of the specified target.\n" +
            "    null\n" +
            "        Clears the nickname of the specified target.\n" +
            "    keep\n" +
            "        Keeps the nickname of the specified target.\n" +
            "    waive\n" +
            "        Keeps the nickname of the specified target.\n" +
            "\n" +
            "[edge colour]\n" +
            "    <R, G, B>\n" +
            "        The new edge colour the pet will adopt.\n" +
            "    clear\n" +
            "        Clears the edge colour of the specified target.\n" +
            "    null\n" +
            "        Clears the edge colour of the specified target.\n" +
            "    keep\n" +
            "        Keeps the edge colour of the specified target.\n" +
            "    waive\n" +
            "        Keeps the edge colour of the specified target.\n" +
            "\n" +
            "[text colour]\n" +
            "    <R, G, B>\n" +
            "        The new text colour the pet will adopt.\n" +
            "    clear\n" +
            "        Clears the text colour of the specified target.\n" +
            "    null\n" +
            "        Clears the text colour of the specified target.\n" +
            "    keep\n" +
            "        Keeps the text colour of the specified target.\n" +
            "    waive\n" +
            "        Keeps the text colour of the specified target.\n"
        );
    }

    private void ClearLast()
    {
        lastCommandPart         = string.Empty;
        matchedArguments.Clear();
        customCounter           = 0;

        activeEntry             = null;
        activeData              = null;
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

    [GeneratedRegex("\"[^\"]*\"|<[^>]*>|-?\\d+")]
    private partial Regex QuotationRegex();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string[] GetArguments(string args)
    {
        return args.Split(' ');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowArgumentException()
    {
        throw new PetNicknamesCommandArgumentException("Missing arguments. Use \"/petname help\" for more information.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowTargetNotFoundException()
    {
        throw new PetNicknamesCommandArgumentException("Target could not be found.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowNewNoPlayerFoundException()
    {
        throw new PetNicknamesCommandArgumentException("Local player could not be found.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowNewErrorException()
    {
        throw new PetNicknamesCommandArgumentException("An unknown error occurred whilst parsing this command.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string GetCommandPart(string[] commandArgs, int location, bool throws = true, string filler = "keep")
    {
        if (location >= commandArgs.Length)
        {
            if (throws)
            {
                ThrowArgumentException();
            }
            else
            {
                lastCommandPart = filler;
            }
        }
        else
        {
            lastCommandPart = commandArgs[location];

            if (lastCommandPart.IsNullOrWhitespace() && !throws)
            {
                lastCommandPart = filler;
            }
        }

        return lastCommandPart;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string? GetCustomString(bool applyCounter = true)
    {
        string? customString = matchedArguments[customCounter];

        if (applyCounter)
        {
            customCounter++;
        }

        return customString;
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

    private enum SetState
    {
        INVALID,
        Keep,
        Null,
        Custom
    }

    private enum SetIndex
    {
        Name,
        EdgeColour,
        TextColour
    }
}
