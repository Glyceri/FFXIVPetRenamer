using Dalamud.Bindings.ImGui;
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
using PetRenamer.PetNicknames.Windowing.Components.Image;
using PetRenamer.PetNicknames.Windowing.Components.Labels;
using PetRenamer.PetNicknames.TranslatorSystem;
using Dalamud.Utility;
using Dalamud.Interface.ImGuiNotification;
using PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using Dalamud.Interface;
using PetRenamer.PetNicknames.Windowing.Windows.PetList;
using PetRenamer.PetNicknames.WritingAndParsing.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using Dalamud.Interface.Utility.Raii;

namespace PetRenamer.PetNicknames.Windowing.Windows;

internal class PetListWindow : PetWindow
{
    private readonly IPettableUserList UserList;
    private readonly IPettableDatabase Database;
    private readonly IPettableDatabase LegacyDatabase;
    private readonly IPetServices      PetServices;
    private readonly IImageDatabase    ImageDatabase;

    private readonly IDataParser DataParser;
    private readonly IDataWriter DataWriter;

    private bool inUserMode     = false;
    private bool lastInUserMode = false;

    private IPettableDatabaseEntry? ActiveEntry;
    private IPettableUser?          lastUser;

    private string SearchText       = string.Empty;
    private string activeSearchText = string.Empty;

    private bool isLocalEntry = false;

    private readonly List<IPetListDrawable> petListDrawables = [];

    private bool importDisabled = false;

    private double internalDisabledTimer = 0;
    private DateTime lastTime = DateTime.Now;

    public PetListWindow(WindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDatabase database, IPettableDatabase legacyDatabase, IImageDatabase imageDatabase, IDataParser dataParser, IDataWriter dataWriter) : base(windowHandler, dalamudServices, petServices.Configuration, "Pet List Window", ImGuiWindowFlags.None)
    {
        UserList        = userList;
        Database        = database;
        LegacyDatabase  = legacyDatabase;
        PetServices     = petServices;
        ImageDatabase   = imageDatabase;
        DataParser      = dataParser;
        DataWriter      = dataWriter;
    }

    protected override Vector2 MinSize
        => new Vector2(400, 250);

    protected override Vector2 MaxSize
        => new Vector2(1600, 1500);

    protected override Vector2 DefaultSize
        => new Vector2(800, 500);

    protected override bool HasModeToggle
        => true;

    public override void OnOpen()
    {
        ClearSearchBar();

        SetUser(UserList.LocalPlayer?.DataBaseEntry);
    }

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

    private void DrawHeader()
    {
        if (Listbox.Begin($"##ListboxHolder_{WindowHandler.InternalCounter}", new Vector2(250, 110) * WindowHandler.GlobalScale))
        {
            PlayerImage.Draw(ActiveEntry, in ImageDatabase);

            ImGui.SameLine();

            if (Listbox.Begin($"##ListboxNametags_{WindowHandler.InternalCounter}", ImGui.GetContentRegionAvail()))
            {
                TextAligner.Align(TextAlignment.Left);

                if (ImGui.Button(ActiveEntry?.Name ?? Translator.GetLine("...") + $"##ToggleButtonButton_{WindowHandler.InternalCounter}", WindowHandler.StretchingBar))
                {
                    _ = DalamudServices.Framework.Run(ToggleUserMode);
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

                BasicLabel.Draw(ActiveEntry?.HomeworldName ?? Translator.GetLine("..."), WindowHandler.StretchingBar);
                BasicLabel.Draw(ActiveEntry?.ActiveDatabase.Length.ToString() ?? Translator.GetLine("..."), WindowHandler.StretchingBar);

                TextAligner.PopAlignment();

                Listbox.End();
            }

            Listbox.End();
        }

        ImGui.SameLine();

        if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", new Vector2(ImGui.GetContentRegionAvail().X, 110 * WindowHandler.GlobalScale)))
        {
            if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", ImGui.GetContentRegionAvail()))
            {

                float contentAvailableX = ImGui.GetContentRegionAvail().X;
                Vector2 barSize = WindowHandler.StretchingBar;

                BasicLabel.Draw("You can export all your pet names to your clipboard and send those to a friend.", barSize);
                BasicLabel.Draw("A friend can import your code to see your names.", barSize);

                if (ImGui.Button($"Export to Clipboard##clipboardExport{WindowHandler.InternalCounter}", new Vector2(contentAvailableX / 2, barSize.Y)))
                {
                    string data = DataWriter.WriteData();

                    if (data.IsNullOrWhitespace())
                    {
                        _ = PetServices.NotificationService.ShowNotification(NotificationType.Warning, Translator.GetLine("ShareWindow.ExportError"));
                    }
                    else
                    {
                        _ = PetServices.NotificationService.ShowNotification(NotificationType.Success, Translator.GetLine("ShareWindow.ExportSuccess"));

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

                    if (!DataParser.ApplyParseData(parseResult, ParseSource.Manual))
                    {
                        string error = string.Empty;

                        if (parseResult is InvalidParseResult invalidParseResult)
                        {
                            error = invalidParseResult.Reason;
                        }

                        _ = PetServices.NotificationService.ShowNotification(NotificationType.Warning, string.Format(Translator.GetLine("ShareWindow.ImportError"), error));
                    }
                    else
                    {
                        string username = string.Empty;

                        if (parseResult is IBaseParseResult baseResult)
                        {
                            username = baseResult.UserName;
                        }

                        StartDisabledTimer();

                        _ = PetServices.NotificationService.ShowNotification(NotificationType.Success, string.Format(Translator.GetLine("ShareWindow.ImportSuccess"), username));
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

    private void DrawSearchbar()
    {
        ImGuiStylePtr style = ImGui.GetStyle();

        using (ImRaii.PushStyle(ImGuiStyleVar.ItemSpacing, new Vector2(style.ItemSpacing.X, 0)))
        {
            float buttSize = WindowHandler.BarHeight;

            bool clicked = false;

            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - buttSize - style.FramePadding.X);

            using (ImRaii.PushStyle(ImGuiStyleVar.FramePadding, new Vector2(10, (buttSize - ImGui.GetTextLineHeight()) * 0.5f)))
            {
                if (ImGui.InputTextWithHint($"##InputText_{WindowHandler.InternalCounter}", ". . .", ref SearchText, 64))
                {
                    clicked |= true;
                }
            }

            SearchText = SearchText.Replace(Environment.NewLine, string.Empty);

            ImGui.SameLine();

            ImGui.PushFont(UiBuilder.IconFont);

            using (ImRaii.PushStyle(ImGuiStyleVar.FramePadding, new Vector2(style.FramePadding.X, (buttSize - ImGui.GetTextLineHeight()) * 0.3f)))
            {
                if (ImGui.Button($"{FontAwesomeIcon.Search.ToIconString()}##Search_{WindowHandler.InternalCounter}", new Vector2(buttSize, buttSize)))
                {
                    clicked |= true;
                }
            }

            ImGui.PopFont();

            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(Translator.GetLine("Search"));
            }

            if (clicked)
            {
                activeSearchText = SearchText;

                _ = DalamudServices.Framework.Run(() =>
                {
                    SetUser(ActiveEntry);
                });
            }
        }
    }

    private unsafe void DrawList()
    {
        if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", ImGui.GetContentRegionAvail()))
        {
            foreach (PetListPet pet in petListDrawables.Where(v => v is PetListPet))
            {
                if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", new Vector2(ImGui.GetContentRegionAvail().X, 110 * WindowHandler.GlobalScale)))
                {
                    float size = ImGui.GetContentRegionAvail().Y;
                    BoxedImage.DrawMinion(in pet.PetSheetData, in DalamudServices, in Configuration, new Vector2(size, size));

                    if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", ImGui.GetContentRegionAvail()))
                    {
                        if (isLocalEntry) {

                            if (RenameLabel.Draw($"Nickname:##NicknameInput_{WindowHandler.InternalCounter}", pet.CustomName == pet.TempName, ref pet.TempName, ref pet.EdgeColour, ref pet.TextColour, WindowHandler.StretchingBar))
                            {
                                OnSave(pet.TempName, pet.PetSheetData.Model, pet.EdgeColour, pet.TextColour);
                            }
                        }
                        else
                        {
                            LabledLabel.Draw("Nickname:", pet.CustomName ?? string.Empty, WindowHandler.StretchingBar);
                        }

                        LabledLabel.Draw("Pet:", pet.PetSheetData.BaseSingular, WindowHandler.StretchingBar);
                        LabledLabel.Draw("ID:", pet.PetSheetData.Model.SkeletonId.ToString(), WindowHandler.StretchingBar);

                        Listbox.End();
                    }

                    Listbox.End();
                }
            }

            foreach (PetListUser user in petListDrawables.Where(v => v is PetListUser))
            {
                if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", new Vector2(ImGui.GetContentRegionAvail().X, 110 * WindowHandler.GlobalScale)))
                {
                    float size = ImGui.GetContentRegionAvail().Y;

                    PlayerImage.Draw(user.Entry, in ImageDatabase);

                    ImGui.SameLine();

                    if (Listbox.Begin($"##Listbox_{WindowHandler.InternalCounter}", ImGui.GetContentRegionAvail()))
                    {
                        if (user.Entry.ContentID == UserList.LocalPlayer?.ContentID)
                        {
                            if (LabledLabel.DrawButton("Username:", user.Entry.Name, WindowHandler.StretchingBar))
                            {
                                _ = DalamudServices.Framework.Run(() =>
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

                            float buttonSize = WindowHandler.BarHeight;

                            ImGuiStylePtr style = ImGui.GetStyle();

                            if (LabledLabel.DrawButton("Username:", user.Entry.Name, new Vector2(ImGui.GetContentRegionAvail().X - (buttonSize * buttonCount) - ((style.ItemSpacing.X * (buttonCount + 1))), WindowHandler.BarHeight)))
                            {
                                _ = DalamudServices.Framework.Run(() =>
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
                                _ = ImGui.Button($"{FontAwesomeIcon.Exclamation.ToIconString()}##Exlemation_{WindowHandler.InternalCounter}", new Vector2(buttonSize, buttonSize));
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
                                _ = DalamudServices.Framework.Run(() =>
                                {
                                    user.Entry.Clear(ParseSource.Manual);
                                });
                            }
                        }

                        LabledLabel.Draw("Homeworld:", user.Entry.HomeworldName, WindowHandler.StretchingBar);
                        LabledLabel.Draw("Pet Count:", user.Entry.ActiveDatabase.Length.ToString(), WindowHandler.StretchingBar);

                        Listbox.End();
                    }

                    Listbox.End();
                }
            }

            Listbox.End();
        }
    }

    private void ClearSearchBar()
    {
        SearchText       = string.Empty;
        activeSearchText = string.Empty;
    }

    private void StartDisabledTimer()
    {
        internalDisabledTimer = 4;
    }

    protected override void OnDirty()
    {
        if ((!ActiveEntry?.IsActive) ?? true)
        {
            ActiveEntry = UserList.LocalPlayer?.DataBaseEntry;
        }

        _ = DalamudServices.Framework.Run(() =>
        {
            SetUser(ActiveEntry);
        });
    }

    protected override void OnModeChange()
    {
        if (inUserMode)
        {
            return;
        }

        ClearSearchBar();

        _ = DalamudServices.Framework.Run(() =>
        {
            SetUser(ActiveEntry);
        });
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
        ActiveEntry  = entry;

        if (lastInUserMode != inUserMode || completeUserChange)
        {
            lastInUserMode = inUserMode;

            ClearSearchBar();
        }

        ClearList();

        if (inUserMode)
        {
            HandleUserMode();
        }
        else
        {
            HandlePetMode();
        }
    }

    private void ClearList()
    {
        petListDrawables.Clear();
    }

    private bool HandleIfLocalEntry(IPettableDatabaseEntry? entry)
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

    private void HandleUserMode()
    {
        IPettableDatabaseEntry[] entries = [.. Database.DatabaseEntries, .. LegacyDatabase.DatabaseEntries];

        int length = entries.Length;

        for (int i = 0; i < length; i++)
        {
            IPettableDatabaseEntry entry = entries[i];

            if (!entry.IsActive && !entry.IsLegacy)
            {
                continue;
            }

            if (!(Valid(entry.Name) || Valid(entry.HomeworldName) || Valid(entry.ContentID.ToString())))
            {
                continue;
            }

            if (entry.ActiveDatabase.Length == 0 && !Configuration.debugModeActive)
            {
                continue;
            }

            petListDrawables.Add(new PetListUser(in DalamudServices, in entry));
        }
    }

    private void HandlePetMode()
    {
        if (ActiveEntry == null)
        {
            return;
        }

        INamesDatabase names = ActiveEntry.ActiveDatabase;

        List<PetSkeleton> validIDS         = [..names.IDs];
        List<string>      validNames       = [..names.Names];
        List<Vector3?>    validEdgeColours = [..names.EdgeColours];
        List<Vector3?>    validTextColours = [..names.TextColours];

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
            PetSkeleton ID = validIDS[i];

            if (PetWindowMode.Minion == CurrentMode && ID.SkeletonType != SkeletonType.Minion)
            {
                continue;
            }

            if (PetWindowMode.BattlePet == CurrentMode && ID.SkeletonType != SkeletonType.BattlePet)
            {
                continue;
            }

            IPetSheetData? petData = PetServices.PetSheets.GetPet(ID);

            if (petData == null)
            {
                continue;
            }

            string   name       = validNames[i];
            Vector3? edgeColour = validEdgeColours[i];
            Vector3? textColour = validTextColours[i];

            if (!(Valid(name) || Valid(ID.SkeletonId.ToString()) || Valid(petData.BaseSingular)))
            {
                continue;
            }

            petListDrawables.Add(new PetListPet(in DalamudServices, in petData, name, edgeColour, textColour));
        }
    }

    private bool Valid(string input)
    {
        if (activeSearchText.IsNullOrWhitespace())
        {
            return true;
        }

        return input.Contains(activeSearchText, StringComparison.InvariantCultureIgnoreCase);
    }

    private void OnSave(string? newName, PetSkeleton skeleton, Vector3? edgeColour, Vector3? textColour)
    {
        _ = DalamudServices.Framework.Run(() =>
        {
            ActiveEntry?.SetName(skeleton, newName ?? string.Empty, edgeColour, textColour);
        });
    }
}
