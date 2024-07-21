using PetRenamer.PetNicknames.ColourProfiling.Interfaces;
using PetRenamer.PetNicknames.Serialization;
using PetRenamer.PetNicknames.Windowing.Base.Style;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.ColourProfiling;

internal class ColourProfileHandler : IColourProfileHandler
{
    readonly Configuration Configuration;
    readonly List<ColourProfile> ColourProfiles = new List<ColourProfile>();

    ColourProfile? activeProfile;

    public ColourProfileHandler(in Configuration configuration)
    {
        Configuration = configuration;

        if (Configuration.ColourProfiles != null)
        {
            foreach (SerializableColourProfile serializableColourProfile in Configuration.ColourProfiles)
            {
                ColourProfiles.Add(FromSerializable(serializableColourProfile));
            }
        }

        WindowStyles.DefaultColourProfile?.Activate();

        if (Configuration.ActiveProfile != null)
        {
            activeProfile = FromSerializable(Configuration.ActiveProfile);
            activeProfile?.Activate();
        }
    }

    ColourProfile FromSerializable(SerializableColourProfile serializableColourProfile)
    {
        List<PetColour> petColours = new List<PetColour>();

        int length = serializableColourProfile.ColourValues.Length;

        for (int i = 0; i < length; i++)
        {
            string name = serializableColourProfile.ColourNames[i];
            uint value = serializableColourProfile.ColourValues[i];

            petColours.Add(new PetColour(name, value));
        }

        return new ColourProfile(serializableColourProfile.Name, serializableColourProfile.Author, petColours);
    }

    SerializableColourProfile ToSerializable(ColourProfile colourProfile)
    {
        List<string> colourNames = new List<string>();
        List<uint> colourValues = new List<uint>();

        foreach (PetColour colour in colourProfile.Colours)
        {
            colourNames.Add(colour.Name);
            colourValues.Add(colour.Colour);
        }

        return new SerializableColourProfile(colourProfile.Name, colourProfile.Author, colourNames.ToArray(), colourValues.ToArray());
    }

    public void SetActiveProfile(SerializableColourProfile? profile)
    {
        if (profile == null)
        {
            activeProfile = null;
            return;
        }

        activeProfile = FromSerializable(profile);
    }

    public SerializableColourProfile? GetActiveProfile()
    {
        if (activeProfile == null) return null;

        return ToSerializable(activeProfile);
    }

    public SerializableColourProfile[] Serialize()
    {
        List<SerializableColourProfile> serializableColourProfiles = new List<SerializableColourProfile>();

        foreach (ColourProfile colourProfile in ColourProfiles)
        {
            serializableColourProfiles.Add(ToSerializable(colourProfile));
        }

        return serializableColourProfiles.ToArray();
    }

    public IColourProfile? GetActive()
    {
        return activeProfile;
    }
}
