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
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetListWindow;

internal partial class PetListWindow : PetWindow
{
    bool inUserMode = false;
    bool lastInUserMode = false;
    IPettableDatabaseEntry? ActiveEntry;
    IPettableUser? lastUser = null;

    bool isLocalEntry = false;

    public const int ElementsPerList = 10;
    int currentIndex = 0;

    public override void OnOpen()
    {
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
        SetIndex(0);
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
        ScrollistContentNode.ChildNodes.Clear();

        SmallHeaderNode.NodeValue = Translator.GetLine("...");

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
        if (UserList.LocalPlayer != null)
        {
            UserListButton.SetText("My List");
        }
        else
        {
            UserListButton.SetText("Pet List");
        }

        IPettableDatabaseEntry[] entries = [..Database.DatabaseEntries, ..LegacyDatabase.DatabaseEntries];
        int length = entries.Length;

        Looper(length, (index) =>
        {
            IPettableDatabaseEntry entry = entries[index];
            if (!entry.IsActive && !entry.IsLegacy) return false;

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
        UserListButton.SetText("User List");

        if (ActiveEntry == null) return;

        INamesDatabase names = ActiveEntry.ActiveDatabase;
        List<int> validIDS = new List<int>();
        List<string> validNames = new List<string>();

        int length = names.Length;

        for (int i = 0; i < length; i++)
        {
            int id = names.IDs[i];
            if (PetWindowMode.Minion    == CurrentMode && id <= -1) continue;
            if (PetWindowMode.BattlePet == CurrentMode && id >= -1) continue;

            string cusomName = names.Names[i];

            validIDS.Add(id);
            validNames.Add(cusomName);
        }

        if (isLocalEntry)
        {
            if (PetWindowMode.BattlePet == CurrentMode)
            {
                List<IPetSheetData> data = PetServices.PetSheets.GetMissingPets(validIDS);
                foreach (IPetSheetData p in data)
                {
                    validIDS.Add(p.Model);
                    validNames.Add("");
                }
            }
        }

        int newLength = validIDS.Count;

        Looper(newLength, (index) =>
        {
            int id = validIDS[index];

            IPetSheetData? petData = PetServices.PetSheets.GetPet(id);
            if (petData == null) return false;

            string customName = validNames[index];

            PetListNode newPetListNode = new PetListNode(in DalamudServices, petData, customName, isLocalEntry);
            ScrollistContentNode.AppendChild(newPetListNode);
            newPetListNode.OnSave += (value) => OnSave(value, id);

            return true;
        });
    }

    void Looper(int length, Func<int, bool> onValidCallback)
    {
        OffsetHelper offsetHelper = new OffsetHelper(currentIndex);

        NextListNode.IsDisabled = true;

        for (int i = 0; i < length; i++)
        {
            OffsetResult result = offsetHelper.OffsetResult();
            if (result == OffsetResult.Early) continue;
            if (result == OffsetResult.Late)
            {
                NextListNode.IsDisabled = false;
                break;
            }

            if (!onValidCallback.Invoke(i)) continue;

            offsetHelper.IncrementValidOffset();
        }
    }

    void OnSave(string? newName, int skeleton) => DalamudServices.Framework.Run(() => ActiveEntry?.SetName(skeleton, newName ?? ""));
}
