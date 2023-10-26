using ImGuiNET;
using PetRenamer.Windows.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using CLE = PetRenamer.Windows.PetWindows.ChangeLogElement;
using CLS = PetRenamer.Windows.PetWindows.ChangeLogStruct;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
internal class ChangelogWindow : PetWindow
{
    // I use the following headers
    // Gameplay Additions/Changes
    // UI Additions/Changes
    // Saving Additions/Changes
    // Bugfixes Additions/Changes
    // Removed Additions/Changes
    // General

    Vector2 baseSize = new Vector2(500, 400);

    readonly Version CurrentVersion;

    readonly List<CLS> changeLogs = new List<CLS>()
    {
        { new CLS(new Version("0.1.0.0"), new CLE("General", "Complete plugin rewrite (now more stable)."), new CLE("UI", "Clear all Pet Names Option.", "Disable the Display of Custom Names Option.")) },
        { new CLS(new Version("0.2.0.0"), new CLE("General", "Added the /petname Command.", "Nicknames get Saved on a Per Pet Basis.")) },
        { new CLS(new Version("0.2.1.0"), new CLE("General", "Added support for sub commands. AKA: /minionname."), new CLE("UI", "Renamed All Instances of Pet to Minion.", "Added a Little Info Spot that Informs Users to Look Away or Resummon Their Pet to Apply a Nickname.")) },
        { new CLS(new Version("0.2.2.0"), new CLE("General", "Filters out Weird Characters to Prevent Crashing.")) },
        { new CLS(new Version("0.2.4.0"), new CLE("General", "Added the /petlist Command."), new CLE("Saving", "Save File Version Updated from Version 1 to Version 2.")) },
        { new CLS(new Version("0.3.1.1"), new CLE("UI", "New UI Styles Per UI Mode.", "Ability to Import and Export Pet Nicknames.", "Pet Nicknames Will Now Display on Other People.")) },
        { new CLS(new Version("0.3.1.3"), new CLE("Bugfixes", "Fixed a bug where logging in on any alt would result in the plugin not working.")) },
        { new CLS(new Version("0.4.0.0"), new CLE("General", "Support for renaming your Carbuncle and Faerie.", "Rewrote the naming process to be WAY more stable."), new CLE("UI",  "Added a new theme to indicate you are now renaming a Battle Pet.")) },
        { new CLS(new Version("0.4.0.2"), new CLE("General", "Added Support for Automaton Queen and Esteem.")) },
        { new CLS(new Version("0.4.1.0"), new CLE("General", "Added IPC Endpoints.", "Rewrote UTILS code to be MUCH faster."), new CLE("Saving", "Save File Version Updated from Version 2 to Version 3.")) },
        { new CLS(new Version("0.4.2.1"), new CLE("General", "Fixed an issue where the /petname command didn't properly detect you had the Esteem and Automaton Queen pets out.")) },
        { new CLS(new Version("0.4.2.2"), new CLE("General", "Added hooks in more places so now pet names show on target bar and focus bar and target of target bar.")) },
        { new CLS(new Version("0.4.2.3"), new CLE("General", "Fixed the healthbar issue and some pet naming issues."), new CLE("YES! This was the horrid update that broke all healthbars :)")) },
        { new CLS(new Version("0.4.3.0"), new CLE("General", "Emotes will now show Custom Petnames.", "The Party List will now show Custom Petnames.")) },
        { new CLS(new Version("0.4.5.1"), new CLE("General", "Tooltips will now show Custom Petnames.", "Fixed an issue where /petlist would cause errors.")) },
        { new CLS(new Version("1.0.0.0"), new CLE("General", "Updated version number.")) },
        { new CLS(new Version("1.1.0.0"), new CLE("General", "Improved performance."), new CLE("UI", "Plugin interface button added.")) },
        { new CLS(new Version("1.1.0.1"), new CLE("General", "Actually does the things mentioned in 1.1.0.0 (I pushed the wrong stuff).")) },
        { new CLS(new Version("1.1.0.2"), new CLE("General", "Fixed stuff not Disposing properly.")) },
        { new CLS(new Version("1.2.0.0"), new CLE("General", "The plugin is WAY more performant.")) },
        { new CLS(new Version("1.2.1.0"), new CLE("UI", "Every single UI thing changed.")) },
        { new CLS(new Version("1.2.1.1"), new CLE("General", "Fixed colours leaking.")) },
        { new CLS(new Version("1.2.2.0"), new CLE("General", "Tooltips should now show Custom Battle Pets.", "Cast bars should now show Custom Battle Pets.", "Flyout text should now show Custom Battle Pets.", "Batte Logs should now show Custom Battle Pets.")) },
        { new CLS(new Version("1.2.2.1"), new CLE("General", "Ninjas will no longer crash upon using ANY ability.")) },
        { new CLS(new Version("1.2.1.2"), new CLE("General", "Map tooltips should work even better.", "Names now show on more castbars.", "Context menu's should now ONLY show on pets.")) },
        { new CLS(new Version("1.2.3.0"), new CLE("General", "Empty names will no longer get exported."), new CLE("UI", "Remade the whole Config Window.","Added a new Quick Menu button.", "Pet list now has a user list mode.", "Export now has an advanced mode.", "The layout of some UI elements have been changed.", "The colours of some UI elements have been changed.", "Fixed a number of UI related issues/bugs.")) },
        { new CLS(new Version("1.2.3.2"), new CLE("General", "Added multiple configuration options.", "Multiple plugin support.")) },
        { new CLS(new Version("1.3.0.0"), new CLE("UI", "Images. Images Everywhere")) },
        { new CLS(new Version("1.3.1.1"), new CLE("General", "Fixed log spam.")) },
        { new CLS(new Version("1.4.0.2"), new CLE("General", "Removed map tooltips completely.", "Weird version bump. What I have internally been calling 1.4.0.1 was publicly known as 1.3.1.1.")) },
        { new CLS(new Version("1.4.1.0"), new CLE("General", "French emotes now work better.", "Fixed a lot of Text Replacement Bugs.", "Some windows wont randomly reset anymore. (Sorry, this includes everything but the pet list :( )", "Cleaned up a lot of the code.", "Increased performance in some areas."), new CLE("UI", "Added a theme editor.", "Better settings menu.", "Added a Changelog Window.", "Most UI is now Resizable."), new CLE("Saving", "Save File Version Updated from Version 6 to Version 7.")) },
        { new CLS(new Version("1.4.1.1"), new CLE("General", "Fixed Redraw not working.")) },
        { new CLS(new Version("1.4.1.2"), new CLE("General", "Fixed Map Tooltips not working.")) },
        { new CLS(new Version("1.4.1.3"), new CLE("General", "Fixed Emotes not working in certain conditions.")) },
        { new CLS(new Version("1.4.1.4"), new CLE("General", "Fixed wrong names displaying in certain situations when having used /petglamour. (/petglamour will very soon receive a full rework).")) },
        { new CLS(new Version("1.4.2.0"), new CLE("General", "Every different Battle Pet model can now be assigned a name. No more naming per Job.", "You can no longer see names on models that are Human. Sorry to those that enjoyed this feature, but it is problematic :("), new CLE("Saving", "Save File Version Updated from Version 7 to Version 8.")) },
        { new CLS(new Version("1.4.2.1"), new CLE("General", "Fixed a bug where Legacy Compatibily would only update your file by 1 step compared to all the steps.")) },
        { new CLS(new Version("1.4.2.2"), new CLE("General", "Custom names will show up on Esteem again.", "Added new IPC endpoints for way, WAY better integration.")) },
        { new CLS(new Version("1.4.2.3"), new CLE("General", "Fixed an issue where sometimes pet names would show as lower case variants.")) },
        { new CLS(new Version("1.4.2.4"), new CLE("General", "Fixed an issue where the pet search bar would crash your game.")) },
        { new CLS(new Version("1.4.2.5"), new CLE("General", "Fixed an issue that caused DelvUI nameplates to not show up with this plugin enabled.")) },
        { new CLS(new Version("1.4.3.0"), new CLE("General", "Added Wotsit support.", "Your xllog should no longer say my plugin failed to dispose hooks. (It has always properly disposed, but now the error should no longer show up.)", "Changed IPC endpoints so plugins to work properly.", "Fixed a bug where using [ ] in your petnames would cause weirdness... there's probably more weirdness that can appear, sorry for that.")) },
    };

    const string starter = "  ◉   ";
    float currentHeight = 0;
    bool canDraw = true;
    readonly Dictionary<string, (bool, float)> toggles = new Dictionary<string, (bool, float)>();
    string currentTitle = string.Empty;

    public ChangelogWindow() : base("Pet Nicknames Changelogs") 
    {
        CurrentVersion = Assembly.GetAssembly(typeof(PetRenamerPlugin))!.GetName().Version!;
        changeLogs.Sort((cl1, cl2) => cl1.version.CompareTo(cl2.version));
        changeLogs.Reverse();

        Size = baseSize;
        SizeCondition = ImGuiCond.FirstUseEver;
        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = baseSize,
            MaximumSize = new Vector2(9999, 9999)
        };
    }

    public override void OnDraw()
    {
        foreach(CLS cls in changeLogs)
        {
            if(BeginElementBox(cls.version.ToString(), CurrentVersion.ToString() == cls.version.ToString()))
            {
                DrawCLSElement(cls);
                EndElementBox();
            }
        }
    }

    void DrawCLSElement(CLS element)
    {
        if (!canDraw) return;
        foreach(CLE cle in element.changeLogElements)
        {
            if (cle.Header != string.Empty)
            {
                NewLabel(cle.Header + $"##{internalCounter++}", new Vector2(ContentAvailableX, BarSize));
                currentHeight += BarSize + ItemSpacingY;
            }
            foreach(string str in cle.ChangeLogStrings)
            {
                ImGui.TextWrapped(starter + str);
                currentHeight += ImGui.CalcTextSize(starter + str, true, ImGui.GetItemRectSize().X).Y + ItemSpacingY;
            }
        }
    }


    bool BeginElementBox(string title, bool forceOpen = false)
    {
        currentTitle = title;
        if (!toggles.ContainsKey(title))
            toggles.Add(title, (forceOpen ? forceOpen : false, 0));
        (bool, float) curToggle = toggles[title];
        currentHeight = curToggle.Item2;
        bool outcome = BeginListBox($"##{title}[version]{internalCounter++}", new Vector2(ContentAvailableX, currentHeight));
        if (!outcome) return false;
        if (!forceOpen)
        {
            if (Button(toggles[title].Item1 ? "v" : ">", new Vector2(BarSize, BarSize)))
                curToggle.Item1 ^= true;
            canDraw = toggles[title].Item1;
            SameLine();
        }
        NewLabel(title + $"##title{internalCounter++}", new Vector2(ContentAvailableX, BarSize));
        currentHeight = BarSize + (ItemSpacingY * 2);
        toggles[title] = curToggle;
        return outcome;
    }

    void EndElementBox()
    {
        ImGui.EndListBox();
        canDraw = true;
        toggles[currentTitle] = (toggles[currentTitle].Item1, currentHeight);
        currentHeight = 0;
    }
}

public struct ChangeLogStruct
{
    public readonly Version version;
    public readonly CLE[] changeLogElements;
    public ChangeLogStruct(Version version, params CLE[] values)
    {
        this.version = version;
        changeLogElements = values;
    }
}

public struct ChangeLogElement
{
    public readonly string Header;
    public readonly string[] ChangeLogStrings;

    public ChangeLogElement(string Header, params string[] ChangeLogStrings)
    {
        this.Header = Header;
        this.ChangeLogStrings = ChangeLogStrings;
    }
}
