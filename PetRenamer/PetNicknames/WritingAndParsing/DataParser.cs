﻿using Dalamud.Game.ClientState.Objects.SubKinds;
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

    public DataParser(in DalamudServices dalamudServices, in IPettableUserList userList, in IPettableDatabase database, in ILegacyDatabase legacyDatabase)
    {
        DalamudServices = dalamudServices;
        UserList = userList;
        Database = database;
        LegacyDatabase = legacyDatabase;
    }

    public void ApplyParseData(IPlayerCharacter player, IDataParseResult result, bool isFromIPC)
    {
        ulong activeContentID = UserList.LocalPlayer?.ContentID ?? 0;

        if (player.GameObjectId == activeContentID && isFromIPC)
        {
            return;
        }

        if (result is InvalidParseResult invalidParseResult)
        {
            DalamudServices.PluginLog.Verbose(invalidParseResult.Reason);
            return;
        }

        if (result is IBaseParseResult version1ParseResult)
        {
            LegacyDatabase.ApplyParseResult(version1ParseResult, isFromIPC);
            return;
        }

        if (result is IModernParseResult version2ParseResult)
        {
            Database.ApplyParseResult(version2ParseResult, isFromIPC);
            return;
        }

        if (result is IClearParseResult clearParseResult)
        {
            Database.RemoveEntry(clearParseResult.ContentID);
            return;
        }
    }

    public IDataParseResult ParseData(string data) => InternalParseData(data);

    static IDataParseResult InternalParseData(string data)
    {
        if(!TryFromBase64(data, out byte[]? stringData))
        {
            return new InvalidParseResult("Incoming data could not become decoded Base64 data.");
        }

        if (!TryGetString(stringData, out string? value))
        {
            return new InvalidParseResult("Incoming parse data failed to become a string.");
        }

        if (value.IsNullOrWhitespace())
        {
            return new InvalidParseResult("Incoming parse data is empty.");
        }


        ParseVersion parseVersion = GetParseVersion(data);
        switch (parseVersion)
        {
            case ParseVersion.Invalid:
                return new InvalidParseResult("Invalid Parse Version.");
            case ParseVersion.Version1:
                return DataParserVersion1.Parse(data);
            case ParseVersion.Version2:
                return DataParserVersion2.Parse(data);
            default:
                return new InvalidParseResult("Invalid Parse Version.");
        }
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