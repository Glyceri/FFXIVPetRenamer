using Dalamud.Utility;
using PetRenamer.PetNicknames.Parsing.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing.Enums;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;
using PetRenamer.PetNicknames.WritingAndParsing.Enums;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using PetRenamer.PetNicknames.WritingAndParsing.ParserElements;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PetRenamer.PetNicknames.Parsing;

internal class DataParser : IDataParser
{
    readonly DalamudServices DalamudServices;
    readonly IPettableUserList UserList;
    readonly IPettableDatabase Database;
    readonly ILegacyDatabase LegacyDatabase;

    static readonly IDataParserElement DataParserVersion1 = new DataParserVersion1();
    static readonly IDataParserElement DataParserVersion2 = new DataParserVersion2();
    static readonly IDataParserElement DataParserVersion3 = new DataParserVersion3();

    public DataParser(DalamudServices dalamudServices, IPettableUserList userList, IPettableDatabase database, ILegacyDatabase legacyDatabase)
    {
        DalamudServices = dalamudServices;
        UserList = userList;
        Database = database;
        LegacyDatabase = legacyDatabase;
    }

    public unsafe bool ApplyParseData(IDataParseResult result, bool isFromIPC)
    {
        IPettableUser? localUser = UserList.LocalPlayer;

        if (result is InvalidParseResult invalidParseResult)
        {
            DalamudServices.PluginLog.Verbose(invalidParseResult.Reason);
            return false;
        }

        if (result is IClearParseResult clearParseResult)
        {
            if (clearParseResult.Name.IsNullOrWhitespace() || clearParseResult.Homeworld == 0) return false;

            IPettableDatabaseEntry? entry = Database.GetEntry(clearParseResult.Name,  clearParseResult.Homeworld, false);

            if (entry != null)
            {
                Database.RemoveEntry(entry);
            }

            return true;
        }

        if (result is IBaseParseResult baseParseResult)
        {
            if (localUser != null && isFromIPC)
            {
                if (localUser.Name == baseParseResult.UserName && localUser.Homeworld == baseParseResult.Homeworld)
                {
                    return false;
                }
            }

            if (baseParseResult is IModernParseResult version2ParseResult)
            {
                Database.ApplyParseResult(version2ParseResult, isFromIPC);
                return true;
            }

            LegacyDatabase.ApplyParseResult(baseParseResult, isFromIPC);
            return true;
        }

        return true;
    }

    public IDataParseResult ParseData(string data) => InternalParseData(data);

    static IDataParseResult InternalParseData(string data)
    {
        string incomingData = data;

        if(TryFromBase64(data, out byte[]? stringData))
        {
            if (TryGetString(stringData, out string? value))
            {
                incomingData = value;
            }
        }

        if (incomingData.IsNullOrWhitespace())
        {
            return new InvalidParseResult("Incoming parse data is empty.");
        }


        ParseVersion parseVersion = GetParseVersion(incomingData);
        return parseVersion switch
        {
            ParseVersion.Invalid => new InvalidParseResult("Data is not Pet Nicknames data."),
            ParseVersion.Version1 => DataParserVersion1.Parse(incomingData),
            ParseVersion.Version2 => DataParserVersion2.Parse(incomingData),
            ParseVersion.Version3 => DataParserVersion3.Parse(incomingData),
            _ => new InvalidParseResult("Invalid Parse Version."),
        };
    }

    static bool TryFromBase64(string s, [NotNullWhen(true)] out byte[]? data)
    {
        try
        {
            data = Convert.FromBase64String(s);
            return true;
        }
        catch 
        {
            data = null;
            return false;
        }
    }

    static bool TryGetString(byte[] data, [NotNullWhen(true)] out string? dataString)
    {
        try
        {
            dataString = Encoding.Unicode.GetString(data);
            return true;
        }
        catch
        {
            dataString = null;
            return false;
        }
    }

    static ParseVersion GetParseVersion(string data)
    {
        for(int i = (int)ParseVersion.Invalid; i < (int)ParseVersion.COUNT; i++)
        {
            ParseVersion currentVersion = (ParseVersion)i;

            string description = currentVersion.GetDescription();
            if (description.IsNullOrWhitespace()) continue;

            if (!data.StartsWith(description, StringComparison.InvariantCultureIgnoreCase)) continue;

            return currentVersion;
        }

        return ParseVersion.Invalid;
    }
}
