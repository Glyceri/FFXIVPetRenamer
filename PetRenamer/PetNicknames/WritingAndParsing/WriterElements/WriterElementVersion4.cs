using Dalamud.Utility;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PetRenamer.PetNicknames.WritingAndParsing.WriterElements;

internal class WriterElementVersion4 : IDataWriterElement
{
    private string GetStringFromPetSkeleton(PetSkeleton petSkeleton)
        => $"{petSkeleton.SkeletonId}{PluginConstants.forbiddenCharacter}{(int)petSkeleton.SkeletonType}";

    public ParseVersion WriteVersion
        => ParseVersion.Version4;

    public string WriteData(IPettableUser forUser)
    {
        string header = ParseVersion.Version4.GetDescription();

        IPettableDatabaseEntry entry = forUser.DataBaseEntry;

        string userName      = entry.Name;
        string homeworldId   = entry.Homeworld.ToString();
        string contentId     = entry.ContentId.ToString();
        string softSkeletons = $"[{GetStringFromPetSkeleton(entry.SoftSkeletons[0])},{GetStringFromPetSkeleton(entry.SoftSkeletons[1])},{GetStringFromPetSkeleton(entry.SoftSkeletons[2])},{GetStringFromPetSkeleton(entry.SoftSkeletons[3])},{GetStringFromPetSkeleton(entry.SoftSkeletons[4])}]";

        INamesDatabase  database = entry.ActiveDatabase;
        int             length   = database.Length;
        List<string>    petLines = [header, userName, homeworldId, contentId, softSkeletons];

        for (int i = 0; i < length; i++)
        {
            string      name       = database.Names[i];
            PetSkeleton id         = database.Ids[i];
            string      edgeColour = database.EdgeColours[i]?.ToString("G", CultureInfo.InvariantCulture) ?? "null";
            string      textColour = database.TextColours[i]?.ToString("G", CultureInfo.InvariantCulture) ?? "null";

            if (id.SkeletonType == SkeletonType.Invalid)
            {
                continue;
            }

            if (name.IsNullOrWhitespace())
            {
                continue;
            }

            string newLine = $"{GetStringFromPetSkeleton(id)}{PluginConstants.forbiddenCharacter}{name}{PluginConstants.forbiddenCharacter}{edgeColour}{PluginConstants.forbiddenCharacter}{textColour}";

            petLines.Add(newLine);
        }

        string outcome;
            
        outcome = string.Join(Environment.NewLine, petLines);
        outcome = Convert.ToBase64String(Encoding.Unicode.GetBytes(outcome));

        return outcome;
    }
}