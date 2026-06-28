// ReSharper disable All
// like... this file is PURELY to test and is actually hot garbage...
#pragma warning disable

using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Utility;
using Dalamud.Plugin.Ipc;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using PetRenamer.PetNicknames.Hooking;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.IPC;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers;
using PetRenamer.PetNicknames.PettableUsers.Enums;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Components;
using PetRenamer.PetNicknames.Windowing.Components.Image;
using PetRenamer.PetNicknames.Windowing.Components.Labels;
using PetRenamer.PetNicknames.WritingAndParsing.Enums;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace PetRenamer.PetNicknames.Windowing.Windows;

internal class PetDevWindow : PetWindow
{
    readonly IPettableDatabase Database;
    readonly IPetServices      PetServices;
    readonly ISharingDictionary SharingDictionary;
    readonly IPronounHook PronounHook;

    protected override Vector2 MinSize { get; } = new Vector2(350, 136);
    protected override Vector2 MaxSize { get; } = new Vector2(2000, 2000);
    protected override Vector2 DefaultSize { get; } = new Vector2(800, 400);

    int currentActive = 0;
    List<DevStruct> devStructList = new List<DevStruct>();

    public PetDevWindow(WindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, IPettableDatabase database, ISharingDictionary sharingDictionary, IPronounHook pronounHook) 
        : base(windowHandler, dalamudServices, petServices, "Pet Dev Window")
    {
        Database    = database;
        PetServices = petServices;
        SharingDictionary = sharingDictionary;
        PronounHook = pronounHook;
        
        RespectCloseHotkey   = false;
        DisableWindowSounds  = true;
        DisableFadeInFadeOut = true;
        
        if (PetServices.Configuration.debugModeActive && PetServices.Configuration.openDebugWindowOnStart)
        {
            Open();
        }

        devStructList.Add(new DevStruct("User List",  DrawUserList));
        devStructList.Add(new DevStruct("Party",      DrawParty));
        devStructList.Add(new DevStruct("Cast",       DrawCasts));
        devStructList.Add(new DevStruct("Targeting",  DrawTargeting));
        devStructList.Add(new DevStruct("Sharing Dict",  DrawSharing));
        devStructList.Add(new DevStruct("IPC Tester", DrawIPCTester, OnIPCUpdate));
        devStructList.Add(new DevStruct("Database",   DrawDatabase));
        devStructList.Add(new DevStruct("Sheets",     DrawSheets));
        devStructList.Add(new DevStruct("Hover",      DrawHover));
        devStructList.Add(new DevStruct("Pronoun",    DrawPronoun));
        devStructList.Add(new DevStruct("Windowing",  DrawWindowing));
        devStructList.Add(new DevStruct("NameError",  DrawNameError));
        devStructList.Add(new DevStruct("Island",     DrawIsland));
        
        currentActive = PetServices.Configuration.lastDebugTab;
    }

    public override bool ShowQuickButtons
        => true;
    
    public override bool HasModeToggle
        => false;
    
    public override void OnOpen()
    {
        if (devStructList.Count <= currentActive)
        {
            currentActive = 0;
            
            return;
        }
        
        devStructList[currentActive].requestUpdate?.Invoke(true);
    }

    public override void OnClose()
    {
        for (int i = 0; i < devStructList.Count; i++)
        {
            devStructList[i].requestUpdate?.Invoke(false);
        }
    }

    protected override void OnDraw()
    {
        if (devStructList.Count == 0) return;

        ImGui.BeginTabBar("##DevTabBar");

        for (int i = 0; i < devStructList.Count; i++)
        {
            if (!ImGui.TabItemButton(devStructList[i].title)) continue;
            
            int lastActive = currentActive;
            
            if (lastActive == i) continue;
            
            currentActive = i;
            ClearSearchBar();
            
            PetServices.Configuration.lastDebugTab = i;
            PetServices.Configuration.Save();

            try
            {
                devStructList[lastActive].requestUpdate?.Invoke(false);
            }
            catch {}
            
            try
            {
                devStructList[currentActive].requestUpdate?.Invoke(true);
            }
            catch {}
        }
        
        try
        {
            devStructList[currentActive].onSelected?.Invoke();
        }
        catch {}

        ImGui.EndTabBar();
    }

    private void DrawIsland()
    {
        ImGui.Text("Last user contentId: " + PetServices.Configuration.LastIslandContentId);
        
        IPettableDatabaseEntry? entry = Database.GetEntryNoCreate(PetServices.Configuration.LastIslandContentId);
        
        if (entry != null)
        {
            DrawDatabaseUser(entry);
        }
        
        IPettableUser? islandUser = PetServices.UserList[IUserList.IslandIndex];
        
        if (islandUser == null)
        {
            ImGui.Text("No island user found!");
        }
        else
        {
            ImGui.Text("I think you're on the island for the user: " + islandUser.DataBaseEntry.Name + "@" + islandUser.DataBaseEntry.HomeworldName);

            NewDrawUser(islandUser);
        }
    }
    
    private void DrawNameError()
    {
        IPettableDatabaseEntry[] entries = Database.DatabaseEntries;

        foreach (IPettableDatabaseEntry entry in entries)
        {
            if (entry.ActiveDatabase.LatestError == NameDatabaseError.NoError)
            {
                continue;
            }
            
            ImGui.Text($"Error in: [{entry.Name}@{entry.HomeworldName}] [{entry.ActiveDatabase.LatestError}].");
        }
    }
    
    void DrawDatabase()
    {
        IPettableDatabaseEntry[] entries = Database.DatabaseEntries;

        foreach (IPettableDatabaseEntry entry in entries)
        {
            DrawDatabaseUser(entry);
        }
    }

    private void DrawWindowing()
    {
        ImGui.Text($"Focussed Window: {WindowHandler.FocussedWindow}");
    }
    
    private void DrawPronoun()
    {
        ImGui.Text("Current Pronoun: ");
        ImGui.SameLine();
        
        ImGui.TextColored(new Vector4(1, 0.5f, 1, 1), $"[{PronounHook.LastGottenPronoun}].");
        
        ImGui.Text($"Last Pronoun: ");
        ImGui.SameLine();
        
        ImGui.TextColored(new Vector4(1, 0.5f, 1, 1), $"[{PronounHook.PreviousLastGottenPronoun}].");
    }
    
    private void DrawHover()
    {
        ImGui.Text("Currently Hovered Pet: ");
        ImGui.SameLine();
        
        ImGui.TextColored(new Vector4(1, 0.5f, 1, 1), $"[{PetServices.HoverService.CurrentlyHoveredPet?.Model}, {PetServices.HoverService.CurrentlyHoveredPet?.Singular}].");
        
        ImGui.Text($"Currently Hovered Nametype: ");
        ImGui.SameLine();
        
        ImGui.TextColored(new Vector4(1, 0.5f, 1, 1), $"[{PetServices.HoverService.CurrentNameType}].");
    }
    
    private string SearchText = string.Empty;
    private string activeSearchText = string.Empty;
    
    private void ClearSearchBar()
    {
        SearchText       = string.Empty;
        activeSearchText = string.Empty;
    }
    
    private void DrawSearchbar()
    {
        ImGuiStylePtr style = ImGui.GetStyle();

        //using (ImRaii.PushStyle(ImGuiStyleVar.ItemSpacing, new Vector2(style.ItemSpacing.X, 0)));
        
        float buttSize = WindowHandler.BarHeight;
        bool  clicked  = false;

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - buttSize - style.FramePadding.X);

        using (ImRaii.PushStyle(ImGuiStyleVar.FramePadding, new Vector2(10, (buttSize - ImGui.GetTextLineHeight()) * 0.5f)))
        {
            if (ImGui.InputTextWithHint($"##InputText_{WindowHandler.InternalCounter}", ". . .", ref SearchText, 64))
            {
                clicked = true;
            }
        }

        SearchText = SearchText.Replace(Environment.NewLine, string.Empty);

        ImGui.SameLine();

        ImGui.PushFont(UiBuilder.IconFont);

        using (ImRaii.PushStyle(ImGuiStyleVar.FramePadding, new Vector2(style.FramePadding.X, (buttSize - ImGui.GetTextLineHeight()) * 0.3f)))
        {
            if (ImGui.Button($"{FontAwesomeIcon.Search.ToIconString()}##Search_{WindowHandler.InternalCounter}", new Vector2(buttSize, buttSize)))
            {
                clicked = true;
            }
        }

        ImGui.PopFont();

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(Translator.GetLine("Search"));
        }

        if (!clicked)
        {
            return;
        }
        
        activeSearchText = SearchText;
    }
    
    private bool PassesSearch(IPetSheetData pet)
    {
        if (pet.Singular.Contains(activeSearchText, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }
        
        if (pet.Model.SkeletonId.ToString().Contains(activeSearchText, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }
        
        return false;
    }
    
    private void DrawSheets()
    {
        DrawSearchbar();
        
        if (!ImGui.BeginTable($"##petTable{WindowHandler.InternalCounter}", 4, ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingMask))
            return;
        
        foreach (IPetSheetData pet in PetServices.PetSheets.AllPets)
        {
            if (!activeSearchText.IsNullOrWhitespace())
            {
                if (!PassesSearch(pet))
                {
                    continue;
                }
            }
            
            ImGui.TableNextRow();
            
            ImGui.TableSetColumnIndex(0);
            ImGui.Text(pet.Model.ToString());
            
            ImGui.TableSetColumnIndex(1);
            ImGui.Text(pet.Singular);
            
            ImGui.TableSetColumnIndex(2);
            ImGui.Text(pet.ActionName);
            
            ImGui.TableSetColumnIndex(3);
            BoxedImage.DrawMinion(pet, DalamudServices, PetServices.Configuration, new Vector2(64, 64));
        }
        
        ImGui.EndTable();
    }
   
    
    void DrawDatabaseUser(IPettableDatabaseEntry user)
    {
        if (!ImGui.BeginTable($"##usersTable{WindowHandler.InternalCounter}", 6, ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingMask))
            return;

        ImGui.TableNextRow();

        ImGui.TableSetColumnIndex(0);
        ImGui.TextUnformatted("Is Active: " + (user.IsActive ? "O" : "X"));

        ImGui.TableSetColumnIndex(1);
        ImGui.TextUnformatted($"{user.Name}");

        ImGui.TableSetColumnIndex(2);
        ImGui.TextUnformatted(user.Homeworld.ToString());

        ImGui.TableSetColumnIndex(3);
        ImGui.TextUnformatted("IsIPC: " + (user.IsIpc ? "O" : "X"));

        ImGui.TableSetColumnIndex(4);
        ImGui.TextUnformatted("Found in Userlist: " + (PetServices.UserList.GetUserFromContentId(user.ContentId) != null ? "O" : "X"));

        ImGui.TableSetColumnIndex(5);
        ImGui.TextUnformatted("Entry Usage Count: " + user.EntryUsageCount.ToString());
        
        ImGui.EndTable();
    }

    ICallGateSubscriber<string>? getPlayerDataAll;
    ICallGateSubscriber<string, object>? onObjectChangeAll;
    ICallGateSubscriber<string, object>? setPlayerDataAll;
    ICallGateSubscriber<ulong, object>? clearPlayerData;

    string lastData = string.Empty;

    string targetMinionName = baseName;
    string targetBattlePetName = baseName;
    Vector3? targetEdgeColour = null;
    Vector3? targetTextColour = null;

    const string baseName = "[Test Name]";

    bool clicked = false;

    IGameObject? lastTarget = null;

    void SetBaseNames()
    {
        targetMinionName = baseName.ToString();
        targetBattlePetName = baseName.ToString();
        targetEdgeColour = null;
        targetTextColour = null;
    }
    
    void DrawSharing()
    {
        GameObjectId[]  shareObjectIds   = SharingDictionary.GetGameObjectIds();
        nint[]          shareAddresses   = SharingDictionary.GetAddresses();
        string[]        shareCustomNames = SharingDictionary.GetCustomNames();
        Vector3?[]      shareEdgeColours = SharingDictionary.GetEdgeColours();
        Vector3?[]      shareTextColours = SharingDictionary.GetTextColours();
        
        ImGui.Text("Sharing:");
        
        for (int i = 0; i < shareAddresses.Length; i++)
        {
            ImGui.Text($"[{i}] [(ID){shareObjectIds[i].Id}, (ADDRESS){shareAddresses[i]}, (CUSTOM NAME){shareCustomNames[i]}, (EDGE COLOUR){(shareEdgeColours[i]?.ToString() ?? "<null>")}, (TEXT COLOUR){(shareTextColours[i]?.ToString() ?? "<null>")}]");
        }
        
        
        ImGui.Separator();
        
        ImGui.Text("LEGACY");
        ImGui.Spacing();
        
        ulong[]  skeletons   = SharingDictionary.GetLegacySkeletons();
        string[] customNames = SharingDictionary.GetLegacyCustomNames();
        
        for (int i = 0; i < skeletons.Length; i++)
        {
            ImGui.BulletText($"{skeletons[i]} {customNames[i]}");
        }
    }
    
    private string GetPetText(IPettablePet? pet)
        => pet == null ? "No Pet" : $"{pet.SkeletonId}] [{pet.Address}";
    
    private string GetUserText(IPettableUser? user)
        => user == null ? "No Player" : $"{user.DataBaseEntry.Name}@{user.DataBaseEntry.HomeworldName}] [{user.PettablePets.Count}";
    
    private string GetTargetText(IPettableEntity? target)
    {
        if (target is IPettableUser user) return GetUserText(user);
        if (target is IPettablePet pet) return GetPetText(pet);
        
        return string.Empty;
    }
    
    bool HasTarget(IPettableUser user, out int targetCount, out int realTargetCount)
    {
        targetCount = 0;
        realTargetCount = 0;
        
        bool hasTarget = false;
        
        if (PetServices.TargetManager.GetLeadingTarget(user) != null)
        {
            if (showLeadingTargets) targetCount++;
            realTargetCount++;
            hasTarget = true;
        }
        if (PetServices.TargetManager.GetLeadingTargetOfLeadingTarget(user) != null)
        {
            if (showLeadingTargetOfLeadingTarget) targetCount++;
            realTargetCount++;
            hasTarget = true;
        }
        if (PetServices.TargetManager.GetSoftTarget(user) != null)
        {
            if (showSoftTargets) targetCount++;
            realTargetCount++;
            hasTarget = true;
        }
        if (PetServices.TargetManager.GetTarget(user) != null)
        {
            if (showTarget) targetCount++;
            realTargetCount++;
            hasTarget = true;
        }
        if (PetServices.TargetManager.GetSoftTargetOfLeadingTarget(user) != null)
        {
            if (showSoftTargetOfLeadingTarget) targetCount++;
            realTargetCount++;
            hasTarget = true;
        }
        if (PetServices.TargetManager.GetTargetOfLeadingTarget(user) != null)
        {
            if (showTargetOfLeadingTarget) targetCount++;
            realTargetCount++;
            hasTarget = true;
        }
        if (PetServices.TargetManager.GetTargetOfTarget(user) != null)
        {
            if (showTargetOfTarget) targetCount++;
            realTargetCount++;
            hasTarget = true;
        }
        if (PetServices.TargetManager.GetSoftTargetOfTarget(user) != null)
        {
            if (showSoftTargetOfTarget) targetCount++;
            realTargetCount++;
            hasTarget = true;
        }
        if (PetServices.TargetManager.GetTargetOfSoftTarget(user) != null)
        {
            if (showTargetOfSoftTarget) targetCount++;
            realTargetCount++;
            hasTarget = true;
        }
        if (PetServices.TargetManager.GetSoftTargetOfSoftTarget(user) != null)
        {
            if (showSoftTargetOfSoftTarget) targetCount++;
            realTargetCount++;
            hasTarget = true;
        }

        return hasTarget;
    }
    
    bool showLeadingTargets = true;
    bool showLeadingTargetOfLeadingTarget = true;
    bool showSoftTargets = true;
    bool showTarget = true;
    bool showSoftTargetOfLeadingTarget = true;
    bool showTargetOfLeadingTarget = true;
    bool showTargetOfTarget = true;
    bool showSoftTargetOfTarget = true;
    bool showTargetOfSoftTarget = true;
    bool showSoftTargetOfSoftTarget = true;
    
    private void DrawToggle(ref bool value, string title)
        => ImGui.Checkbox(title, ref value);
    
    private bool CanDrawDebugMenu()
    {
        #if DEBUG
        return true;
        #endif
        
        if (DalamudServices.Condition[ConditionFlag.BoundByDuty]) return false;
        if (DalamudServices.Condition[ConditionFlag.BoundByDuty56]) return false;
        if (DalamudServices.Condition[ConditionFlag.BoundByDuty95]) return false;
        if (DalamudServices.Condition[ConditionFlag.InCombat]) return false;
        if (DalamudServices.Condition[ConditionFlag.InDeepDungeon])return false;
        if (DalamudServices.Condition[ConditionFlag.InDuelingArea]) return false;
        if (DalamudServices.ClientState.IsPvP) return false;
        
        return true;
    }
    
    unsafe void DrawTargeting()
    {
        if (!CanDrawDebugMenu())
        {
            return;
        }
        
        DrawToggle(ref showLeadingTargets, nameof(showLeadingTargets));
        DrawToggle(ref showLeadingTargetOfLeadingTarget, nameof(showLeadingTargetOfLeadingTarget));
        DrawToggle(ref showSoftTargets, nameof(showSoftTargets));
        DrawToggle(ref showTarget, nameof(showTarget));
        DrawToggle(ref showSoftTargetOfLeadingTarget, nameof(showSoftTargetOfLeadingTarget));
        DrawToggle(ref showTargetOfLeadingTarget, nameof(showTargetOfLeadingTarget));
        DrawToggle(ref showTargetOfTarget, nameof(showTargetOfTarget));
        DrawToggle(ref showSoftTargetOfTarget, nameof(showSoftTargetOfTarget));
        DrawToggle(ref showTargetOfSoftTarget, nameof(showTargetOfSoftTarget));
        DrawToggle(ref showSoftTargetOfSoftTarget, nameof(showSoftTargetOfSoftTarget));
        
        ImGui.NewLine();
        ImGui.NewLine();
        
        foreach (IPettableUser? user in PetServices.UserList)
        {
            if (user == null) continue;
            
            if (!HasTarget(user, out int targetCount, out int realTargetCount)) continue;
            if (targetCount == 0) continue;
            
            ImGui.BulletText($"[{realTargetCount}]    [{GetUserText(user)}]");
            
            ImGui.Indent();
            
            if (showLeadingTargets)                 if (PetServices.TargetManager.GetLeadingTarget(user) != null)                   ImGui.BulletText($"[Leading Target]: [{GetTargetText(PetServices.TargetManager.GetLeadingTarget(user))}]");
            if (showLeadingTargetOfLeadingTarget)   if (PetServices.TargetManager.GetLeadingTargetOfLeadingTarget(user) != null)    ImGui.BulletText($"[Leading Target of Leading Target]: [{GetTargetText(PetServices.TargetManager.GetLeadingTargetOfLeadingTarget(user))}]");    
            if (showSoftTargets)                    if (PetServices.TargetManager.GetSoftTarget(user) != null)                      ImGui.BulletText($"[Soft Target]: [{GetTargetText(PetServices.TargetManager.GetSoftTarget(user))}]");
            if (showTarget)                         if (PetServices.TargetManager.GetTarget(user) != null)                          ImGui.BulletText($"[Target]: [{GetTargetText(PetServices.TargetManager.GetTarget(user))}]");    
            if (showSoftTargetOfLeadingTarget)      if (PetServices.TargetManager.GetSoftTargetOfLeadingTarget(user) != null)       ImGui.BulletText($"[Soft Target of Leading Target]: [{GetTargetText(PetServices.TargetManager.GetSoftTargetOfLeadingTarget(user))}]");
            if (showTargetOfLeadingTarget)          if (PetServices.TargetManager.GetTargetOfLeadingTarget(user) != null)           ImGui.BulletText($"[Target of Leading Target]: [{GetTargetText(PetServices.TargetManager.GetTargetOfLeadingTarget(user))}]");    
            if (showTargetOfTarget)                 if (PetServices.TargetManager.GetTargetOfTarget(user) != null)                  ImGui.BulletText($"[Target of Target]: [{GetTargetText(PetServices.TargetManager.GetTargetOfTarget(user))}]");
            if (showSoftTargetOfTarget)             if (PetServices.TargetManager.GetSoftTargetOfTarget(user) != null)              ImGui.BulletText($"[Soft Target of Target]: [{GetTargetText(PetServices.TargetManager.GetSoftTargetOfTarget(user))}]");
            if (showTargetOfSoftTarget)             if (PetServices.TargetManager.GetTargetOfSoftTarget(user) != null)              ImGui.BulletText($"[Target of Soft Target]: [{GetTargetText(PetServices.TargetManager.GetTargetOfSoftTarget(user))}]");
            if (showSoftTargetOfSoftTarget)         if (PetServices.TargetManager.GetSoftTargetOfSoftTarget(user) != null)          ImGui.BulletText($"[Soft Target of Soft Target]: [{GetTargetText(PetServices.TargetManager.GetSoftTargetOfSoftTarget(user))}]");
            
            ImGui.Unindent();
            
            ImGui.Spacing();
        }
    }
    
    unsafe void DrawCasts()
    {
        if (!CanDrawDebugMenu())
        {
            return;
        }
        
        foreach (IPettableUser? user in PetServices.UserList)
        {
            if (user == null)
            {
                continue;
            }
            
            if (user.CurrentCastId == 0)
            {
                continue;
            }
            
            float curCastTime   = user.BattleChara->CastInfo.CurrentCastTime;
            float totalCastTime = user.BattleChara->CastInfo.TotalCastTime;
            
            try
            {
                ImGui.BulletText($"[{GetUserText(user)}]      [{PetServices.PetSheets.GetAction(user.CurrentCastId)?.Name}] [{(int)((curCastTime / totalCastTime) * 100)}%]");
            }
            catch {}
        }
    }
    
    private void DrawPartyLeft()
    {
        int index = 0;
        
        ImGui.Text("Total Party Size: " + PetServices.Party.Length);
        
        foreach (IPettableUser? user in PetServices.Party)
        {
            if (user == null)
            {
                ImGui.BulletText($"[{index}] [{GetUserText(user)}]");
            }
            else
            {
                ImGui.BulletText($"[{index}] [{GetUserText(user)}]");
                
                ImGui.Indent();
                
                int petIndex = 0;
                
                foreach (IPettablePet pet in user.PettablePets)
                {
                    ImGui.BulletText($"[{petIndex}] [{GetPetText(pet)}]");
                    
                    petIndex++;
                }
                
                ImGui.Unindent();
            }
            
            index++;
        }
    }
    
    private unsafe void DrawPartyRight()
    {
        AgentHUD* agentChud = AgentHUD.Instance();
        
        int index = 0;
        
        foreach (HudPartyMember partyMember in agentChud->PartyMembers)
        {
            ImGui.BulletText($"[{index++}] [{partyMember.Name}, {partyMember.ContentId}]");
        }
    }
    
    void DrawParty()
    {
        if (ImGui.BeginListBox("###PET_NICKNAMES_DEV_PARTY_LISTBOX_LEFT", ImGui.GetContentRegionAvail() * new Vector2(0.5f, 1.0f)))
        {
            DrawPartyLeft();
            
            ImGui.EndListBox();
        }
        
        ImGui.SameLine();
        
        if (ImGui.BeginListBox("###PET_NICKNAMES_DEV_PARTY_LISTBOX_RIGHT", ImGui.GetContentRegionAvail()))
        {
            DrawPartyRight();
            
            ImGui.EndListBox();
        }
    }
    
    unsafe void DrawIPCTester()
    {
        Vector2 size = new Vector2(ImGui.GetContentRegionAvail().X, 30 * ImGuiHelpers.GlobalScale);

        if (LabledLabel.DrawButton("Recollect Data", "Click here##recollectButton", size))
        {
            GrabLastData();
        }

        if (LabledLabel.DrawButton("Clear Last Data", "Click here##clearButton", size))
        {
            lastData = string.Empty;
        }

        if (Listbox.Begin("##IPCBox", new Vector2(ImGui.GetContentRegionAvail().X, 200)))
        {
            string cleanedData = lastData;
            if (!cleanedData.IsNullOrWhitespace())
            {
                byte[] data = Convert.FromBase64String(cleanedData);
                cleanedData = Encoding.Unicode.GetString(data);
                ImGui.Text(cleanedData);
            }
            Listbox.End();
        }

        ImGui.NewLine();
        ImGui.NewLine();

        IGameObject? target = DalamudServices.TargetManager.Target;

        if (target?.Address != lastTarget?.Address)
        {
            lastTarget = target;
            SetBaseNames();
        }

        
        if (target != null)
        {
            if (target is IPlayerCharacter player)
            {
                bool hasTarget = true;
                
                IPettableUser? user = PetServices.UserList.GetUser(player!.Address, UserListFindType.PetMeansOwner);
                if (user != null)
                {
                    hasTarget = !user.DataBaseEntry.IsActive;
                }

                LabledLabel.Draw("Target Available", hasTarget ? "Yes" : "No", size);

                if (Listbox.Begin("##TargetBox", ImGui.GetContentRegionAvail()))
                {
                    Vector2 sizeIn = new Vector2(ImGui.GetContentRegionAvail().X, 30 * WindowHandler.GlobalScale);

                    LabledLabel.Draw("Target", player!.Name.TextValue, sizeIn);

                    BattleChara* bChara = (BattleChara*)player.Address;

                    if (LabledLabel.DrawButton("Clear Data", "Click here##clearIPCTarget", sizeIn))
                    {
                        ClearIPC(bChara->ObjectIndex);
                    }

                    if (hasTarget)
                    {
                        string startString = "[PetNicknames(2)]\n";
                        startString += bChara->NameString + "\n";
                        startString += bChara->HomeWorld + "\n";
                        startString += bChara->ContentId + "\n";
                        startString += "[-411,-417,-416,-415,-407]";


                        BattleChara* bPet = CharacterManager.Instance()->LookupPetByOwnerObject(bChara);
                        if (bPet != null)
                        {
                            RenameLabel.Draw($"Has Battle Pet [{bPet->NameString}]", true, ref targetBattlePetName, ref targetEdgeColour, ref targetTextColour, sizeIn, labelWidth: 300);
                            if (clicked)
                            {
                                int id = -bPet->Character.ModelContainer.ModelCharaId;
                                startString += $"\n{id}^{targetBattlePetName}";
                            }
                        }

                        Character* minion = &bChara->CompanionObject->Character;
                        if (minion != null)
                        {
                            RenameLabel.Draw($"Has Minion [{minion->NameString}]", true, ref targetMinionName, ref targetEdgeColour, ref targetTextColour, sizeIn, labelWidth: 300);

                            if (clicked)
                            {
                                int id = minion->ModelContainer.ModelCharaId;
                                startString += $"\n{id}^{targetMinionName}";
                            }
                        }

                        if (clicked)
                        {
                            DalamudServices.PluginLog.Debug(startString);
                            SendAll(startString);
                        }

                        clicked = LabledLabel.DrawButton("Apply Data", "Click here##applyDataIPC", sizeIn);
                    }

                    Listbox.End();
                }
            }
        }
    }

    void ClearIPC(ulong chara)
    {
        try
        {
            clearPlayerData?.InvokeAction(chara);
        }
        catch (Exception e) { DalamudServices.PluginLog.Debug(e.Message); }
    }

    void SendAll(string data)
    {
        try
        {
            setPlayerDataAll?.InvokeAction(data);
        }
        catch (Exception e) { DalamudServices.PluginLog.Debug(e.Message); }
    }

    void OnIPCUpdate(bool active)
    {
        if (active)
        {
            ActivateIPC();
        }
        else
        {
            DeactivateIPC();
        }
    }

    void ActivateIPC()
    {
        lastData = string.Empty;

        getPlayerDataAll = DalamudServices.DalamudPlugin.GetIpcSubscriber<string>("PetRenamer.GetPlayerData");
        onObjectChangeAll = DalamudServices.DalamudPlugin.GetIpcSubscriber<string, object>("PetRenamer.PlayerDataChanged");
        setPlayerDataAll = DalamudServices.DalamudPlugin.GetIpcSubscriber<string, object>("PetRenamer.SetPlayerData");
        clearPlayerData = DalamudServices.DalamudPlugin.GetIpcSubscriber<ulong, object>("PetRenamer.ClearPlayerData");

        onObjectChangeAll?.Unsubscribe(OnPlayerDataChanged);
        onObjectChangeAll?.Subscribe(OnPlayerDataChanged);

        GrabLastData();
        SetBaseNames();
    }

    void GrabLastData()
    {
        try
        {
            lastData = getPlayerDataAll?.InvokeFunc() ?? string.Empty;
        }
        catch { }
    }

    void DeactivateIPC()
    {
        lastData = string.Empty;

        onObjectChangeAll?.Unsubscribe(OnPlayerDataChanged);

        getPlayerDataAll = null;
        onObjectChangeAll = null;
        setPlayerDataAll = null;
        clearPlayerData = null;

        SetBaseNames();
    }

    void OnPlayerDataChanged(string data)
    {
        lastData = data;
    }

    void DrawUserList()
    {
        DrawBattlePetCount();
        
        foreach (IPettableUser? user in PetServices.UserList)
        {
            if (user == null) continue;
            NewDrawUser(user);
        }
    }

    unsafe void DrawBattlePetCount()
    {
        int battlePetCount = 0;

        for (int i = 0; i < 100; i++)
        {
            BattleChara* bChara = CharacterManager.Instance()->BattleCharas[i];
            if (bChara == null) continue;

            ObjectKind objKind = bChara->ObjectKind;
            if (objKind != ObjectKind.BattleNpc) continue;

            uint ownerID = bChara->OwnerId;

            if (ownerID == PluginConstants.InvalidId)
            {
                continue;
            }

            battlePetCount++;
        }

        int glyceriEstematedBattlePetCount = 0;

        for (int i = 0; i < 100; i++)
        {
            IPettableUser? user = PetServices.UserList[i];
            if (user == null) continue;

            foreach(IPettablePet? pet in user.PettablePets)
            {
                if (pet == null) continue;
                if (pet is not IPettableBattlePet) continue;

                glyceriEstematedBattlePetCount++;
            }
        }

        LabledLabel.Draw("Accurate Battle Pet Count: ", $"{battlePetCount}", WindowHandler.StretchingBar, labelWidth: 400);
        LabledLabel.Draw("My calculated Battle Pet Count (These should be equal): ", $"{glyceriEstematedBattlePetCount}", WindowHandler.StretchingBar, labelWidth: 400);
    }

    unsafe void NewDrawUser(IPettableUser user)
    {
        if (!ImGui.BeginTable($"##usersTable{WindowHandler.InternalCounter}", 4, ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingMask))
            return;

        ImGui.TableNextRow();

        ImGui.TableSetColumnIndex(0);
        ImGui.TextUnformatted($"{user.DataBaseEntry.Name}");

        ImGui.TableSetColumnIndex(1);
        ImGui.TextUnformatted(user.DataBaseEntry.HomeworldName);

        ImGui.TableSetColumnIndex(2);
        if (user.IsActive)
        {
            if (ImGui.Button($"O###DEBUG_DEACTIVATE_{user.DataBaseEntry.ContentId}"))
            {
                Database.GetEntry(user.DataBaseEntry.ContentId).Clear(ParseSource.Manual);
            }
            
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("DONT CLICK THIS! IT WILL DELETE THE USER'S ENTRY");
            }
        }
        else
        {
            if (ImGui.Button($"X###DEBUG_ACTIVATE_{user.DataBaseEntry.ContentId}"))
            {
                user.DataBaseEntry.UpdateContentId(user.DataBaseEntry.ContentId, true);
            }
        }
        
        ImGui.TableSetColumnIndex(3);
        ImGui.TextUnformatted(user.DataBaseEntry.ActiveDatabase.Length.ToString());

        foreach (IPettablePet pet in user.PettablePets)
        {
            ImGui.TableNextRow();

            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{pet.SkeletonId}");

            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted(((BattleChara*)pet.Address)->NameString);

            ImGui.TableSetColumnIndex(2);
            string?  customName = pet.Owner?.DataBaseEntry.GetName(pet.SkeletonId);
            Vector3? edgeColour = pet.Owner?.DataBaseEntry.GetEdgeColour(pet.SkeletonId);
            Vector3? textColour = pet.Owner?.DataBaseEntry.GetTextColour(pet.SkeletonId);
            SeString seString   = PetServices.StringHelper.WrapInColor(customName, edgeColour, textColour);
            ImGuiHelpers.SeStringWrapped(seString.EncodeWithNullTerminator());
            
            ImGui.TableSetColumnIndex(3);
            if (ImGui.Button($"Set Name###DEBUGSETNAME_{pet.SkeletonId}_{user.DataBaseEntry.ContentId}"))
            {
                WindowHandler.GetWindow<PetRenameWindow>()?.SetRenameWindow(pet.SkeletonId, user.DataBaseEntry);
            }
        }

        ImGui.EndTable();
    }

    protected override void OnDispose()
    {
        DeactivateIPC();
    }
}

readonly struct DevStruct
{
    public readonly string title;
    public readonly Action onSelected;
    public readonly Action<bool>? requestUpdate;

    public DevStruct(string title, Action onSelected, Action<bool>? requestUpdate = null)
    {
        this.title = title;
        this.onSelected = onSelected;
        this.requestUpdate = requestUpdate;
    }
}

#pragma warning enable
// ReSharper restore All