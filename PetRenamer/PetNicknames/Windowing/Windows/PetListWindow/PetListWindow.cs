using Dalamud.Utility;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.ListNodes;
using PetRenamer.PetNicknames.Windowing.Enums;
using PetRenamer.PetNicknames.Windowing.Windows.PetListWindow.Enum;
using PetRenamer.PetNicknames.Windowing.Windows.PetListWindow.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetListWindow;

internal partial class PetListWindow : PetWindow
{
    bool inSearchMode = false;
    bool inUserMode = false;
    bool lastInUserMode = false;
    IPettableDatabaseEntry? ActiveEntry;
    IPettableUser? lastUser = null;

    bool isLocalEntry = false;

    public const int ElementsPerList = 10;
    int currentIndex = 0;

    public override void OnOpen()
    {
        SetSearchMode(false);
        SetUser(UserList.LocalPlayer?.DataBaseEntry);
    }

    public override void OnDraw()
    {
        BottomPortion.Style.Size = new Size(ContentSize.Width, ContentSize.Height - 100);
        ScrollListBaseNode.Style.Size = new Size(BottomPortion.Style.Size.Width - 120, BottomPortion.Style.Size.Height - 60);

        if (lastUser != UserList.LocalPlayer)
        {
            lastUser = UserList.LocalPlayer;
            SetUser(lastUser?.DataBaseEntry);
        }
    }

    public override void OnDirty()
    {
        if ((!ActiveEntry?.IsActive) ?? true)
        {
            ActiveEntry = UserList.LocalPlayer?.DataBaseEntry;
        }

        SetUser(ActiveEntry);
    }

    void HandleIncrement(int amount)
    {
        SetIndex(currentIndex + amount);
    }

    void ToggleSearchMode()
    {
        SetSearchMode(!inSearchMode);
    }

    void SetSearchMode(bool searchModeOn)
    {
        inSearchMode = searchModeOn;

        if (inSearchMode)
        {
            SmallHeaderNode.Style.IsVisible = false;
            SearchBarNode.Style.IsVisible = true;
        }
        else
        {
            SmallHeaderNode.Style.IsVisible = true;
            SearchBarNode.Style.IsVisible = false;
        }

        SearchBarNode.ClearSearchbar();
        SetIndex(0);
    }

    void SetIndex(int amount)
    {
        currentIndex = amount;
        PreviousListNode.IsDisabled = currentIndex == 0;
        NextListNode.IsDisabled = true;
        SetUser(ActiveEntry);
    }

    protected override void OnPetModeChanged(PetWindowMode mode)
    {
        if (inUserMode) return;
        SetSearchMode(false);
        SetIndex(0);
    }

    void ToggleUserMode()
    {
        SetSearchMode(false);

        inUserMode = !inUserMode;

        if (!inUserMode && UserList.LocalPlayer != null)
        {
            ActiveEntry = UserList.LocalPlayer?.DataBaseEntry;
        }
        SetUser(ActiveEntry);
    }

    public void SetUser(IPettableDatabaseEntry? entry)
    {
        isLocalEntry = HandleIfLocalEntry(entry);

        bool completeUserChange = ActiveEntry != entry;
        ActiveEntry = entry;

        if (lastInUserMode != inUserMode || completeUserChange)
        {
            lastInUserMode = inUserMode;
            SetIndex(0);
            return;
        }

        UserNode.SetUser(entry);
        for (int i = ScrollistContentNode.ChildNodes.Count - 1; i >= 0; i--)
        {
            ScrollistContentNode.RemoveChild(ScrollistContentNode.ChildNodes[i], true);
        }


        if (inUserMode) HandleUserMode();
        else HandlePetMode();
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
        HandleHeaderUsermode();

        IPettableDatabaseEntry[] entries = [.. Database.DatabaseEntries, .. LegacyDatabase.DatabaseEntries];
        int length = entries.Length;

        Looper(length, (index) =>
        {
            IPettableDatabaseEntry entry = entries[index.Item1];
            if (!entry.IsActive && !entry.IsLegacy) return false;

            if (inSearchMode)
            {
                if (!(SearchBarNode.Valid(entry.Name)
                 || SearchBarNode.Valid(entry.HomeworldName)
                 || SearchBarNode.Valid(entry.Version)
                 || SearchBarNode.Valid(entry.AddedOn))) return false;
            }

            if (!index.Item2)
            {
                return true;
            }

            bool isLocal = HandleIfLocalEntry(entry);

            UserListNode userNode = new UserListNode(in DalamudServices, in ImageDatabase, in entry, isLocal);
            ScrollistContentNode.AppendChild(userNode);

            userNode.OnView += (user) =>
            {
                inUserMode = false;
                SetUser(user);
            };

            return true;
        });
    }

    void HandlePetMode()
    {
        HandleHeaderPetmode();

        if (ActiveEntry == null) return;

        INamesDatabase names = ActiveEntry.ActiveDatabase;

        List<int> validIDS = names.IDs.ToList();
        List<string> validNames = names.Names.ToList();

        if (isLocalEntry && PetWindowMode.BattlePet == CurrentMode)
        {
            List<IPetSheetData> data = PetServices.PetSheets.GetMissingPets(validIDS);
            foreach (IPetSheetData p in data)
            {
                validIDS.Add(p.Model);
                validNames.Add("");
            }
        }

        int newLength = validIDS.Count;

        Looper(newLength, (index) =>
        {
            int id = validIDS[index.Item1];

            if (PetWindowMode.Minion == CurrentMode && id <= -1) return false;
            if (PetWindowMode.BattlePet == CurrentMode && id >= -1) return false;

            IPetSheetData? petData = PetServices.PetSheets.GetPet(id);
            if (petData == null) return false;

            string customName = validNames[index.Item1];

            if (inSearchMode)
            {
                if (!(SearchBarNode.Valid(petData.BaseSingular)
                 || SearchBarNode.Valid(petData.Model.ToString())
                 || SearchBarNode.Valid(customName))) return false;
            }

            if (!index.Item2)
            {
                return true;
            }

            PetListNode newPetListNode = new PetListNode(in DalamudServices, petData, customName, isLocalEntry);
            ScrollistContentNode.AppendChild(newPetListNode);
            newPetListNode.OnSave += (value) => OnSave(value, id);

            return true;
        });
    }

    void Looper(int length, Func<(int, bool), bool> onValidCallback)
    {
        OffsetHelper offsetHelper = new OffsetHelper(currentIndex);

        NextListNode.IsDisabled = true;

        for (int i = 0; i < length; i++)
        {
            bool valid = true;

            OffsetResult result = offsetHelper.OffsetResult();
            if (result == OffsetResult.Early) valid = false;
            if (result == OffsetResult.Late)
            {
                NextListNode.IsDisabled = false;
                break;
            }

            if (!onValidCallback.Invoke((i, valid))) continue;

            offsetHelper.IncrementValidOffset();
        }
    }

    void HandleHeaderPetmode()
    {
        UserListButton.SetText(Translator.GetLine("PetList.UserList"));

        if (ActiveEntry == UserList.LocalPlayer?.DataBaseEntry)
        {
            if (CurrentMode == PetWindowMode.Minion)
            {
                SmallHeaderNode.NodeValue = Translator.GetLine("PetListWindow.ListHeaderPersonalMinion");
            }
            else
            {
                SmallHeaderNode.NodeValue = Translator.GetLine("PetListWindow.ListHeaderPersonalBattlePet");
            }
        }
        else
        {
            if (CurrentMode == PetWindowMode.Minion)
            {
                SmallHeaderNode.NodeValue = string.Format(Translator.GetLine("PetListWindow.ListHeaderOtherMinion"), ActiveEntry?.Name);
            }
            else
            {
                SmallHeaderNode.NodeValue = string.Format(Translator.GetLine("PetListWindow.ListHeaderOtherBattlePet"), ActiveEntry?.Name);
            }
        }
    }

    void HandleHeaderUsermode()
    {
        if (UserList.LocalPlayer != null)
        {
            UserListButton.SetText(Translator.GetLine("PetList.MyList"));
        }
        else
        {
            UserListButton.SetText(Translator.GetLine("PetList.Title"));
        }

        SmallHeaderNode.NodeValue = Translator.GetLine("PetList.UserList");
    }

    void OnSave(string? newName, int skeleton) => DalamudServices.Framework.Run(() => ActiveEntry?.SetName(skeleton, newName ?? ""));
}
