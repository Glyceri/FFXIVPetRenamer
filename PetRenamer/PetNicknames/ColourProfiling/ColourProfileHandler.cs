using PetRenamer.PetNicknames.ColourProfiling.Interfaces;
using PetRenamer.PetNicknames.Serialization;
using PetRenamer.PetNicknames.Windowing.Base.Style;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows.ColourEditorWindow;
using System.Collections.Generic;
using Una.Drawing;

namespace PetRenamer.PetNicknames.ColourProfiling;

// My plogon is actually very well protected against people
// editing their save file and breaking stuff right.
// Not this time, not here.
// If you edit this and dont set the proper active colourprofile, you are cooking a bit.
internal class ColourProfileHandler : IColourProfileHandler
{
    IWindowHandler? WindowHandler;
    readonly Configuration Configuration;
    public List<IColourProfile> ColourProfiles { get; } = new List<IColourProfile>();

    int activeProfile;

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

        activeProfile = Configuration.ActiveProfile;
        Activate(GetActiveProfile());
    }

    IColourProfile FromSerializable(SerializableColourProfile serializableColourProfile)
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

    SerializableColourProfile ToSerializable(IColourProfile colourProfile)
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

    public int GetActiveAsSerialized()
    {
        return activeProfile;
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

    public IColourProfile GetActiveProfile()
    {
        if (activeProfile <= -1)
        {
            return WindowStyles.DefaultColourProfile;
        }

        if (activeProfile >= ColourProfiles.Count)
        {
            return WindowStyles.DefaultColourProfile;
        }

        return ColourProfiles[activeProfile];
    }

    public void RegisterWindowHandler(in IWindowHandler windowHandler)
    {
        WindowHandler = windowHandler;
    }

    public void AddColourProfile(IColourProfile colourProfile)
    {
        if (ColourProfiles.Contains(colourProfile)) return;

        ColourProfiles.Add(colourProfile);

        WindowHandler?.GetWindow<ColourEditorWindow>()?.OnPresetListChanged();
    }

    public void RemoveColourProfile(IColourProfile colourProfile)
    {
        int indexOf = ColourProfiles.IndexOf(colourProfile);
        if (indexOf == -1) return;

        if (indexOf == activeProfile)
        {
            SetActiveProfile(null);
        }
        else if (indexOf < activeProfile)
        {
            activeProfile--;
        }

        ColourProfiles.RemoveAt(indexOf);

        WindowHandler?.GetWindow<ColourEditorWindow>()?.OnPresetListChanged();
    }

    public void SetActiveProfile(IColourProfile? profile)
    {
        if (profile == null || profile == WindowStyles.DefaultColourProfile)
        {
            activeProfile = -1;
            Activate(WindowStyles.DefaultColourProfile);
        }
        else
        {
            int colourProfileCount = ColourProfiles.Count;

            for (int i = 0; i < colourProfileCount; i++)
            {
                IColourProfile colourProfile = ColourProfiles[i];
                if (colourProfile != profile) continue;

                activeProfile = i;
                Activate(colourProfile);

                break;
            }
        }
    }

    void Activate(IColourProfile colourProfile)
    {
        if (colourProfile != WindowStyles.DefaultColourProfile)
        {
            Activate(WindowStyles.DefaultColourProfile);
        }

        foreach (PetColour petColour in colourProfile.Colours)
        {
            Color.AssignByName(petColour.Name, petColour.Colour);
        }
    }

    public void Refresh()
    {
        Activate(GetActiveProfile());
    }
}
