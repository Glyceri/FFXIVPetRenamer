using Dalamud.Interface.Utility;
using ImGuiNET;
using PetRenamer.PetNicknames.PettableUsers;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Base;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows;

internal class PetDevWindow : PetWindow
{
    readonly IPettableUserList UserList;

    protected override Vector2 MinSize { get; } = new Vector2(350, 136);
    protected override Vector2 MaxSize { get; } = new Vector2(2000, 2000);
    protected override Vector2 DefaultSize { get; } = new Vector2(800, 400);
    protected override bool HasModeToggle { get; } = true;

    float BarSize = 30 * ImGuiHelpers.GlobalScale;

    public PetDevWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration, in IPettableUserList userList) : base(windowHandler, dalamudServices, configuration, "Pet Dev Window", ImGuiWindowFlags.None)
    {
        UserList = userList;

        if (configuration.debugModeActive && configuration.openDebugWindowOnStart)
        {
            Open();
        }
    }

    protected override void OnDraw()
    {
        foreach (IPettableUser? user in UserList.PettableUsers)
        {
            if (user == null) continue;
            NewDrawUser(user);
        }
    }

    void NewDrawUser(IPettableUser user)
    {
        if (!ImGui.BeginTable($"##usersTable{WindowHandler.InternalCounter}", 5, ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingMask))
            return;

        ImGui.TableNextRow();

        ImGui.TableSetColumnIndex(0);
        ImGui.TextUnformatted($"{user.Name}");

        ImGui.TableSetColumnIndex(1);
        ImGui.TextUnformatted(user.DataBaseEntry.HomeworldName);

        ImGui.TableSetColumnIndex(2);
        ImGui.TextUnformatted(user.IsActive ? "O" : "X");

        ImGui.TableSetColumnIndex(3);
        ImGui.TextUnformatted(user.DataBaseEntry.ActiveDatabase.Length.ToString());

        foreach (IPettablePet pet in user.PettablePets)
        {
            ImGui.TableNextRow();

            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{pet.SkeletonID}");

            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted(pet.Name);

            ImGui.TableSetColumnIndex(2);
            ImGui.TextUnformatted(pet.PetData?.BaseSingular);

            ImGui.TableSetColumnIndex(3);
            ImGui.TextUnformatted(pet.PetData?.BasePlural);

            ImGui.TableSetColumnIndex(4);
            ImGui.TextUnformatted(pet.Index.ToString());
        }

        ImGui.EndTable();
    }
}
