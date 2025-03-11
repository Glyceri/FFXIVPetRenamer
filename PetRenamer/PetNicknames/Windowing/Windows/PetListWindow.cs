using ImGuiNET;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Base;
using System;
using System.Numerics;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Windowing.Enums;
using System.Collections.Generic;
using System.Linq;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Windows.PetList.Interfaces;
using PetRenamer.PetNicknames.Windowing.Components;
using Dalamud.Interface.Utility;
using PetRenamer.PetNicknames.Windowing.Components.Image;
using PetRenamer.PetNicknames.Windowing.Components.Labels;
using PetRenamer.PetNicknames.TranslatorSystem;
using Dalamud.Utility;
using Dalamud.Interface.ImGuiNotification;
using PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using Dalamud.Interface;
using PetRenamer.PetNicknames.Windowing.Windows.PetList;

namespace PetRenamer.PetNicknames.Windowing.Windows;

internal class PetListWindow : PetWindow
{
    readonly IPettableUserList UserList;
    readonly IPettableDatabase Database;
    readonly IPettableDatabase LegacyDatabase;
    readonly IPetServices PetServices;
    readonly IImageDatabase ImageDatabase;

    readonly IDataParser DataParser;
    readonly IDataWriter DataWriter;

    protected override Vector2 MinSize { get; } = new Vector2(400, 250);
    protected override Vector2 MaxSize { get; } = new Vector2(1600, 1500);
    protected override Vector2 DefaultSize { get; } = new Vector2(800, 500);
    protected override bool HasModeToggle { get; } = true;

    bool inUserMode = false;
    bool lastInUserMode = false;
    IPettableDatabaseEntry? ActiveEntry;
    IPettableUser? lastUser = null;

    string SearchText = string.Empty;
    string activeSearchText = string.Empty;

    bool isLocalEntry = false;

    readonly List<IPetListDrawable> petListDrawables = new List<IPetListDrawable>();

    bool importDisabled = false;

    float BarHeight => 30 * ImGuiHelpers.GlobalScaleSafe;

    public PetListWindow(WindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDatabase database, IPettableDatabase legacyDatabase, IImageDatabase imageDatabase, IDataParser dataParser, IDataWriter dataWriter) : base(windowHandler, dalamudServices, petServices.Configuration, "Pet List Window", ImGuiWindowFlags.None)
    {
        UserList = userList;
        Database = database;
        LegacyDatabase = legacyDatabase;
        PetServices = petServices;
        ImageDatabase = imageDatabase;
        DataParser = dataParser;
        DataWriter = dataWriter;
    }

    public override void OnOpen()
    {
        ClearSearchBar();
        SetUser(UserList.LocalPlayer?.DataBaseEntry);
    }

    DateTime lastTime = DateTime.Now;

    protected override void OnDraw()
    {
        DateTime now = DateTime.Now;

        TimeSpan deltaSpan = lastTime - now;
        lastTime = now;

        if (internalDisabledTimer >= 0)
        {
            internalDisabledTimer += deltaSpan.TotalSeconds;
            importDisabled = true;
        }
        else
        {
            importDisabled = false;
        }

        if (lastUser != UserList.LocalPlayer)
        {
            lastUser = UserList.LocalPlayer;
            SetUser(lastUser?.DataBaseEntry);
        }

        DrawHeader();
        DrawSearchbar();
        DrawList();
    }

    void DrawHeader()
    {
        if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", new Vector2(250, 110) * ImGuiHelpers.GlobalScale))
        {
            PlayerImage.Draw(ActiveEntry, in ImageDatabase);
            ImGui.SameLine();

            if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", ImGui.GetContentRegionAvail()))
            {
                TextAligner.Align(TextAlignment.Left);

                float contentAvailableX = ImGui.GetContentRegionAvail().X;
                Vector2 barSize = new Vector2(contentAvailableX, BarHeight);

                if (ImGui.Button(ActiveEntry?.Name ?? Translator.GetLine("...") + $"##ToggleButtonButton_{WindowHandler.InternalCounter}", barSize))
                {
                    DalamudServices.Framework.Run(() => ToggleUserMode());
                }
                if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                {
                    if (!inUserMode)
                    {
                        ImGui.SetTooltip(Translator.GetLine("PetList.UserList"));
                    }
                    else
                    {
                        ImGui.SetTooltip(Translator.GetLine("PetList.Title"));
                    }
                }
                BasicLabel.Draw(ActiveEntry?.HomeworldName ?? Translator.GetLine("..."), barSize);
                BasicLabel.Draw(ActiveEntry?.ActiveDatabase.Length.ToString() ?? Translator.GetLine("..."), barSize);

                TextAligner.PopAlignment();
                Listbox.End();
            }

            Listbox.End();
        }

        ImGui.SameLine();

        if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", new Vector2(ImGui.GetContentRegionAvail().X, 110 * ImGuiHelpers.GlobalScale)))
        {
            if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", ImGui.GetContentRegionAvail()))
            {

                float contentAvailableX = ImGui.GetContentRegionAvail().X;
                Vector2 barSize = new Vector2(contentAvailableX, BarHeight);

                BasicLabel.Draw("You can export all your pet names to your clipboard and send those to a friend.", barSize);
                BasicLabel.Draw("A friend can import your code to see your names.", barSize);

                if (ImGui.Button($"Export to Clipboard##clipboardExport{WindowHandler.InternalCounter}", new Vector2(contentAvailableX / 2, barSize.Y)))
                {
                    string data = DataWriter.WriteData();
                    if (data.IsNullOrWhitespace())
                    {
                        DalamudServices.NotificationManager.AddNotification(new Notification()
                        {
                            Type = NotificationType.Warning,
                            Content = Translator.GetLine("ShareWindow.ExportError"),
                        });
                    }
                    else
                    {
                        DalamudServices.NotificationManager.AddNotification(new Notification()
                        {
                            Type = NotificationType.Success,
                            Content = Translator.GetLine("ShareWindow.ExportSuccess"),
                        });

                        ImGui.SetClipboardText(data);
                    }
                }
                if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                {
                    ImGui.SetTooltip(Translator.GetLine("ShareWindow.Export"));
                }

                ImGui.SameLine();

                ImGui.BeginDisabled(importDisabled);

                if (ImGui.Button($"Import from Clipboard##clipboardExport{WindowHandler.InternalCounter}", new Vector2(contentAvailableX / 2, barSize.Y)))
                {
                    StartDisabledTimer();

                    IDataParseResult parseResult = DataParser.ParseData(ImGui.GetClipboardText());

                    if (!DataParser.ApplyParseData(parseResult, false))
                    {
                        string error = string.Empty;
                        if (parseResult is InvalidParseResult invalidParseResult) error = invalidParseResult.Reason;

                        DalamudServices.NotificationManager.AddNotification(new Notification()
                        {
                            Type = NotificationType.Warning,
                            Content = string.Format(Translator.GetLine("ShareWindow.ImportError"), error)
                        });
                    }
                    else
                    {
                        string username = string.Empty;
                        if (parseResult is IBaseParseResult baseResult) username = baseResult.UserName;

                        StartDisabledTimer();

                        DalamudServices.NotificationManager.AddNotification(new Notification()
                        {
                            Type = NotificationType.Success,
                            Content = string.Format(Translator.GetLine("ShareWindow.ImportSuccess"), username)
                        });
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(Translator.GetLine("ShareWindow.Import"));
                }
                ImGui.EndDisabled();
                Listbox.End();
            }
            Listbox.End();
        }
    }

    void DrawSearchbar()
    {
        if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", new Vector2(ImGui.GetContentRegionAvail().X, 30 * ImGuiHelpers.GlobalScale)))
        {
            ImGuiStylePtr style = ImGui.GetStyle();
            float buttSize = ImGui.GetContentRegionAvail().Y;

            bool clicked = false;

            if (ImGui.InputTextMultiline($"##InputText_{WindowHandler.InternalCounter}", ref SearchText, 64, new Vector2(ImGui.GetContentRegionAvail().X - buttSize - style.FramePadding.X, buttSize), ImGuiInputTextFlags.CtrlEnterForNewLine | ImGuiInputTextFlags.EnterReturnsTrue))
            {
                clicked |= true;
            }

            SearchText = SearchText.Replace("\n", string.Empty);

            ImGui.SameLine();

            ImGui.PushFont(UiBuilder.IconFont);

            if (ImGui.Button($"{FontAwesomeIcon.Search.ToIconString()}##Search_{WindowHandler.InternalCounter}", new Vector2(buttSize, buttSize)))
            {
                clicked |= true;
            }

            ImGui.PopFont();

            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(Translator.GetLine("Search"));
            }

            if (clicked)
            {
                activeSearchText = SearchText;
                DalamudServices.Framework.Run(() => SetUser(ActiveEntry));
            }

            Listbox.End();
        }
    }

    unsafe void DrawList()
    {
        if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", ImGui.GetContentRegionAvail()))
        {
            foreach (PetListPet pet in petListDrawables.Where(v => v is PetListPet))
            {
                if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", new Vector2(ImGui.GetContentRegionAvail().X, 110 * ImGuiHelpers.GlobalScale)))
                {
                    float size = ImGui.GetContentRegionAvail().Y;
                    BoxedImage.DrawMinion(in pet.PetSheetData, in DalamudServices, in Configuration, new Vector2(size, size));

                    if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", ImGui.GetContentRegionAvail()))
                    {
                        if (isLocalEntry) {

                            if (RenameLabel.Draw($"Nickname:##NicknameInput_{WindowHandler.InternalCounter}", pet.CustomName == pet.TempName, ref pet.TempName, ref pet.EdgeColour, ref pet.TextColour, new Vector2(ImGui.GetContentRegionAvail().X, BarHeight)))
                            {
                                OnSave(pet.TempName, pet.PetSheetData.Model, pet.EdgeColour, pet.TextColour);
                            }
                        }
                        else
                        {
                            LabledLabel.Draw("Nickname:", pet.CustomName ?? string.Empty, new Vector2(ImGui.GetContentRegionAvail().X, BarHeight));
                        }
                        LabledLabel.Draw("Pet:", pet.PetSheetData.BaseSingular, new Vector2(ImGui.GetContentRegionAvail().X, BarHeight));
                        LabledLabel.Draw("ID:", pet.PetSheetData.Model.ToString(), new Vector2(ImGui.GetContentRegionAvail().X, BarHeight));

                        Listbox.End();
                    }
                    Listbox.End();
                }
            }

            foreach (PetListUser user in petListDrawables.Where(v => v is PetListUser))
            {
                if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", new Vector2(ImGui.GetContentRegionAvail().X, 110 * ImGuiHelpers.GlobalScale)))
                {
                    float size = ImGui.GetContentRegionAvail().Y;

                    PlayerImage.Draw(user.Entry, in ImageDatabase);
                    ImGui.SameLine();

                    if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", ImGui.GetContentRegionAvail()))
                    {
                        if (user.Entry.ContentID == UserList.LocalPlayer?.ContentID)
                        {
                            if (LabledLabel.DrawButton("Username:", user.Entry.Name, new Vector2(ImGui.GetContentRegionAvail().X, BarHeight)))
                            {
                                DalamudServices.Framework.Run(() =>
                                {
                                    ToggleUserMode();
                                    SetUser(user.Entry);
                                });
                            }
                        }
                        else
                        {
                            bool isLegacy = user.Entry.IsLegacy;
                            bool isIPC = user.Entry.IsIPC;

                            bool isSpecial = isLegacy || isIPC;

                            int buttonCount = isSpecial ? 2 : 1;

                            float buttonSize = BarHeight;

                            ImGuiStylePtr style = ImGui.GetStyle();

                            if (LabledLabel.DrawButton("Username:", user.Entry.Name, new Vector2(ImGui.GetContentRegionAvail().X - (buttonSize * buttonCount) - ((style.ItemSpacing.X * (buttonCount + 1))), BarHeight)))
                            {
                                DalamudServices.Framework.Run(() =>
                                {
                                    ToggleUserMode();
                                    SetUser(user.Entry);
                                });
                            }

                            if (isSpecial)
                            {
                                ImGui.SameLine();

                                Vector4* colour = ImGui.GetStyleColorVec4(ImGuiCol.ButtonActive);

                                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, *colour);
                                ImGui.PushStyleColor(ImGuiCol.Button, *colour);
                                ImGui.PushStyleColor(ImGuiCol.ButtonActive, *colour);

                                ImGui.PushFont(UiBuilder.IconFont);
                                ImGui.Button($"{FontAwesomeIcon.Exclamation.ToIconString()}##Exlemation_{WindowHandler.InternalCounter}", new Vector2(buttonSize, buttonSize));
                                ImGui.PopFont();
                                if (ImGui.IsItemHovered())
                                {
                                    if (isLegacy)
                                    {
                                        ImGui.SetTooltip(Translator.GetLine("UserListElement.WarningOldUser"));
                                    }
                                    else if (isIPC)
                                    {
                                        ImGui.SetTooltip(Translator.GetLine("UserListElement.WarningIPC"));
                                    }
                                }

                                ImGui.PopStyleColor(3);

                            }

                            ImGui.SameLine();

                            if (EraserButton.Draw(new Vector2(buttonSize, buttonSize), Translator.GetLine("ClearButton.Label"), Translator.GetLine("PetRenameNode.Clear")))
                            {
                                DalamudServices.Framework.Run(() => user.Entry.Clear(false));
                            }
                        }
                        LabledLabel.Draw("Homeworld:", user.Entry.HomeworldName, new Vector2(ImGui.GetContentRegionAvail().X, BarHeight));
                        LabledLabel.Draw("Pet Count:", user.Entry.ActiveDatabase.Length.ToString(), new Vector2(ImGui.GetContentRegionAvail().X, BarHeight));

                        Listbox.End();
                    }
                    Listbox.End();
                }
            }

            Listbox.End();
        }
    }

    void ClearSearchBar()
    {
        SearchText = string.Empty;
        activeSearchText = string.Empty;
    }

    double internalDisabledTimer = 0;

    void StartDisabledTimer()
    {
        internalDisabledTimer = 4;
    }

    protected override void OnDirty()
    {
        if ((!ActiveEntry?.IsActive) ?? true)
        {
            ActiveEntry = UserList.LocalPlayer?.DataBaseEntry;
        }

        DalamudServices.Framework.Run(() => SetUser(ActiveEntry));
    }

    protected override void OnModeChange()
    {
        if (inUserMode) return;
        ClearSearchBar();
        DalamudServices.Framework.Run(() => SetUser(ActiveEntry));
    }

    void ToggleUserMode()
    {
        inUserMode = !inUserMode;

        if (!inUserMode && UserList.LocalPlayer != null)
        {
            ActiveEntry = UserList.LocalPlayer?.DataBaseEntry;
        }

        SetUser(ActiveEntry);
    }

    public void SetUser(IPettableDatabaseEntry? entry)
    {
        bool completeUserChange = ActiveEntry != entry;

        isLocalEntry = HandleIfLocalEntry(entry);

        ActiveEntry = entry;

        if (lastInUserMode != inUserMode || completeUserChange)
        {
            lastInUserMode = inUserMode;
            ClearSearchBar();
        }

        ClearList();

        if (inUserMode) HandleUserMode();
        else HandlePetMode();
    }

    void ClearList()
    {
        foreach (IPetListDrawable drawable in petListDrawables)
        {
            drawable?.Dispose();
        }
        petListDrawables.Clear();
    }

    bool HandleIfLocalEntry(IPettableDatabaseEntry? entry)
    {
        if (UserList.LocalPlayer != null && entry != null)
        {
            return UserList.LocalPlayer.ContentID == entry.ContentID;
        }
        else
        {
            return false;
        }
    }

    void HandleUserMode()
    {
        IPettableDatabaseEntry[] entries = [.. Database.DatabaseEntries, .. LegacyDatabase.DatabaseEntries];
        int length = entries.Length;
        for (int i = 0; i < length; i++)
        {
            IPettableDatabaseEntry entry = entries[i];

            if (!entry.IsActive && !entry.IsLegacy) continue;

            if (!(Valid(entry.Name) || Valid(entry.HomeworldName) || Valid(entry.ContentID.ToString()))) continue;

            if (entry.ActiveDatabase.Length == 0 && !Configuration.debugModeActive) continue;

            petListDrawables.Add(new PetListUser(in DalamudServices, in entry));
        }
    }

    void HandlePetMode()
    {
        if (ActiveEntry == null) return;

        INamesDatabase names = ActiveEntry.ActiveDatabase;

        List<int> validIDS = names.IDs.ToList();
        List<string> validNames = names.Names.ToList();
        List<Vector3?> validEdgeColours = names.EdgeColours.ToList();
        List<Vector3?> validTextColours = names.TextColours.ToList();

        if (isLocalEntry && PetWindowMode.BattlePet == CurrentMode)
        {
            List<IPetSheetData> data = PetServices.PetSheets.GetMissingPets(validIDS);
            foreach (IPetSheetData p in data)
            {
                validIDS.Add(p.Model);
                validNames.Add("");
                validEdgeColours.Add(null);
                validTextColours.Add(null);
            }
        }

        int newLength = validIDS.Count;

        for (int i = 0; i < newLength; i++)
        {
            int ID = validIDS[i];

            if (PetWindowMode.Minion == CurrentMode && ID <= -1) continue;
            if (PetWindowMode.BattlePet == CurrentMode && ID >= -1) continue;

            IPetSheetData? petData = PetServices.PetSheets.GetPet(ID);
            if (petData == null) continue;

            string name = validNames[i];
            Vector3? edgeColour = validEdgeColours[i];
            Vector3? textColour = validTextColours[i];

            if (!(Valid(name) || Valid(ID.ToString()) || Valid(petData.BaseSingular))) continue;

            petListDrawables.Add(new PetListPet(in DalamudServices, in petData, name, edgeColour, textColour));
        }
    }

    public bool Valid(string input)
    {
        if (activeSearchText.IsNullOrWhitespace()) return true;

        return input.Contains(activeSearchText, StringComparison.InvariantCultureIgnoreCase);
    }

    void OnSave(string? newName, int skeleton, Vector3? edgeColour, Vector3? textColour)
    {
        DalamudServices.Framework.Run(() => ActiveEntry?.SetName(skeleton, newName ?? "", edgeColour, textColour));
    }
}
