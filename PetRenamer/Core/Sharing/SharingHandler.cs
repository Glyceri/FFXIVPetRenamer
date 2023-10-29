using Dalamud.Logging;
using ImGuiNET;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetRenamer.Core.Sharing;

public static class SharingHandler
{
    readonly static List<bool> containsList = new List<bool>();
    static PettableUser lastSet = null!;

    public static bool HasSetup(PettableUser user) => lastSet == user;
    public static bool GetContainsList(int index) => containsList[index];
    public static void SetContainsList(int index, bool value) => containsList[index] = value;
    public static int ContainsListLenght() => containsList.Count;
    public static void ToggleAll(bool value)
    {
        for(int i = 0; i < containsList.Count; i++)
            containsList[i] = value;
    }

    public static void ClearList()
    {
        lastSet = null!;
        containsList!.Clear();
    }

    public static void SetupList(PettableUser user)
    {
        if (user == null) return;
        lastSet = user;

        containsList.Clear();
        for (int i = 0; i < user.SerializableUser.length; i++)
            containsList.Add(true);
    }

    public static bool Export(PettableUser user)
    {
        if (user == null) return false;
        if (lastSet != user) SetupList(user);
        try
        {
            string exportString = string.Concat("[PetExport]\n", user.UserName.ToString(), "\n", user.Homeworld.ToString(), "\n");
            for (int i = 0; i < user.SerializableUser.length; i++)
                if (user.SerializableUser[i].RawName != string.Empty && containsList[i])
                    exportString += $"{user.SerializableUser[i].ID}{PluginConstants.forbiddenCharacter}{user.SerializableUser[i].RawName}\n";
            string convertedString = Convert.ToBase64String(Encoding.Unicode.GetBytes(exportString));
            ImGui.SetClipboardText(convertedString);
            return true;
        }
        catch (Exception e) { PetLog.Log($"Export Error occured: {e}"); }
        return false;
    }
}
