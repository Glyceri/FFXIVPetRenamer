using Dalamud.Interface.Windowing;
using ImGuiNET;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.Windowing;

internal class TempWindow : Window
{
    IPettableUserList UserList { get; init; }

    public TempWindow(IPettableUserList userList) : base("Temp Window", ImGuiWindowFlags.None, true)
    {
        UserList = userList;

        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = new System.Numerics.Vector2 (100, 100),
        };
    }

    public unsafe override void Draw()
    {
        ImGui.BeginTable("Pet Nicknames Table##1", 3, ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.ScrollY, ImGui.GetContentRegionAvail());

        foreach (IPettableUser? item in UserList.pettableUsers) 
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (item == null) ImGui.Text("NULL");
            else ImGui.Text(item.Name);
            ImGui.TableSetColumnIndex(1);
            if (item == null) ImGui.Text("_____");
            else ImGui.Text(item.ContentID.ToString());
            ImGui.TableSetColumnIndex(2);
            if (item == null) ImGui.Text("?");
            else ImGui.Text(item.DataBaseEntry.Length().ToString());
        }

        ImGui.EndTable();
    }
}
