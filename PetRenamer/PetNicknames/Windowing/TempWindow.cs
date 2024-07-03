using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using ImGuiNET;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.Windowing;

internal class TempWindow : Window
{
    IPettableUserList UserList { get; init; }
    IPettableDatabase Database { get; init; }

    public TempWindow(IPettableUserList userList, IPettableDatabase database) : base("Temp Window", ImGuiWindowFlags.None, true)
    {
        UserList = userList;
        Database = database;

        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = new System.Numerics.Vector2 (100, 100),
        };
    }

    public unsafe override void Draw()
    {
        if (ImGui.Button("Everone active"))
        {
            foreach (IPettableUser? item in UserList.pettableUsers)
            {
                if (item == null) continue;
                BattleChara* bchara = CharacterManager.Instance()->LookupBattleCharaByName(item.Name, true, (short)item.Homeworld);
                if (bchara == null) continue;
                item.DataBaseEntry.UpdateContentID(bchara->ContentId);
            }
        }

        ImGui.BeginTable("Pet Nicknames Table##1", 5, ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.ScrollY, ImGui.GetContentRegionAvail() * new System.Numerics.Vector2(1, 0.5f));

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
            ImGui.TableSetColumnIndex(3);
            if (item == null) ImGui.Text("_");
            else ImGui.Text(item.DataBaseEntry.IsActive.ToString());
            ImGui.TableSetColumnIndex(4);
            if (item == null) ImGui.Text("_");
            else ImGui.Text(item.PettablePets.Count.ToString());
        }

        ImGui.EndTable();

        ImGui.BeginTable("Pet Nicknames Table##2", 2, ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.ScrollY, ImGui.GetContentRegionAvail());

        foreach (IPettableDatabaseEntry? item in Database.DatabaseEntries)
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (item == null) ImGui.Text("NULL");
            else ImGui.Text(item.Name);
            ImGui.TableSetColumnIndex(1);
            if (item == null) ImGui.Text("_____");
            else ImGui.Text(item.ContentID.ToString());

            for(int i = 0; i < item.Length(); i++)
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                if (item == null) ImGui.Text("_");
                else ImGui.Text(item.ActiveDatabase.IDs[i].ToString());
                ImGui.TableSetColumnIndex(1);
                if (item == null) ImGui.Text("_");
                else ImGui.Text(item.ActiveDatabase.Names[i]);
            }
        }

        ImGui.EndTable();
    }
}
