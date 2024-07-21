using Dalamud.Utility;
using PetRenamer.PetNicknames.ColourProfiling;
using PetRenamer.PetNicknames.ColourProfiling.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing.Enums;
using PetRenamer.PetNicknames.ReadingAndParsing.Interfaces;
using PetRenamer.PetNicknames.WritingAndParsing.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetRenamer.PetNicknames.ReadingAndParsing;

internal class DataWriter : IDataWriter
{
    readonly IPettableUserList UserList;

    public DataWriter(in IPettableUserList userList)
    {
        UserList = userList;
    }

    public string WriteColourData(in IColourProfile colourProfile)
    {
        List<string> strings = new List<string>() { colourProfile.Name, colourProfile.Author };

        foreach (PetColour petColour in colourProfile.Colours)
        {
            strings.Add($"{petColour.Name}{PluginConstants.forbiddenCharacter}{petColour.Colour}");
        }

        string outcome = string.Join("\n", strings);

        outcome = Convert.ToBase64String(Encoding.Unicode.GetBytes(outcome));

        return outcome;
    }

    public string WriteData()
    {
        IPettableUser? localUser = UserList.LocalPlayer;
        if (localUser == null)
        {
            return string.Empty;
        }

        string header = ParseVersion.Version2.GetDescription();

        IPettableDatabaseEntry entry = localUser.DataBaseEntry;

        string userName = entry.Name;
        string homeworldID = entry.Homeworld.ToString();
        string contentID = entry.ContentID.ToString();
        string SoftSkeletons = $"[{entry.SoftSkeletons[0]},{entry.SoftSkeletons[1]},{entry.SoftSkeletons[2]},{entry.SoftSkeletons[3]},{entry.SoftSkeletons[4]}]";

        INamesDatabase database = entry.ActiveDatabase;
        int length = database.Length;

        List<string> petLines = new List<string>() { header, userName, homeworldID, contentID, SoftSkeletons };

        for (int i = 0; i < length; i++)
        {
            string name = database.Names[i];
            int id = database.IDs[i];

            if (id == 0) continue;
            if (name.IsNullOrWhitespace()) continue;

            string newLine = $"{id}{PluginConstants.forbiddenCharacter}{name}";
            petLines.Add(newLine);
        }

        string outcome = string.Join("\n", petLines);

        outcome = Convert.ToBase64String(Encoding.Unicode.GetBytes(outcome));

        return outcome;
    }
}
