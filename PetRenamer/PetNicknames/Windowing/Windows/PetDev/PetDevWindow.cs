using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using ImGuiNET;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Base;

using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetDev;

internal unsafe class PetDevWindow : PetWindow
{
    IpcProvider ipc;

    public PetDevWindow(DalamudServices dalamudServices, in IpcProvider ipcProvider) : base(dalamudServices, "PETDEV")
    { 
        ipc = ipcProvider;
    }

    protected override string ID { get; } = "Pet Dev";
    protected override Vector2 MinSize { get; } = new Vector2(100, 100);
    protected override Vector2 MaxSize { get; } = new Vector2(1500, 1500);
    protected override Vector2 DefaultSize { get; } = new Vector2(500, 500);
    protected override bool HasModeToggle { get; } = false;
    protected override string Title { get; } = "Pet Dev";

    int mode = 0;

    public override void OnDraw()
    {

    }

    public override void OnLateDraw()
    {
        ImGui.Text("This was PURELY for testing. I keep it in because I might need someone to use it.");
        ImGui.Text("Please leave unless you know what you are doing.");
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(5, 0));
        DoDraw();
        ImGui.PopStyleVar();
    }

    void DoDraw()
    {

        if (ImGui.Button("_1_##1"))
        {
            mode = 0;
        }
        ImGui.SameLine();
        if (ImGui.Button("_2_##2"))
        {
            mode = 1;
        }
        ImGui.SameLine();
        if (ImGui.Button("_3_##3"))
        {
            mode = 2;
        }
        ImGui.SameLine();
        if (ImGui.Button("_4_##4"))
        {
            mode = 3;
        }
        ImGui.SameLine();
        if (ImGui.Button("_5_##5"))
        {
            mode = 4;
        }

        if (mode == 0) RenderMode0();
        if (mode == 1) RenderMode1();
        if (mode == 2) RenderMode2();
        if (mode == 3) RenderMode3();
        if (mode == 4) RenderMode4();
    }

    string IPCData = "";

    void RenderMode0()
    {
        ImGui.Text("IPC TESTER!");

        if (ImGui.Button("Get IPC Data"))
        {
            IPCData = ipc.GetPlayerDataDetour();
        }

        IGameObject? target = DalamudServices.TargetManager.Target;

        if (target != null)
        {
            if (ImGui.Button("Clear IPC for target##clear_for_target"))
            {
                IPlayerCharacter? playerCharacter = target as IPlayerCharacter;
                if (playerCharacter != null)
                {
                    ipc.ClearIPCDataDetour(playerCharacter);
                }
            }

            if (ImGui.Button("Set IPC for target##set_for_target"))
            {
                IPlayerCharacter? playerCharacter = target as IPlayerCharacter;
                IPlayerCharacter pChara = (IPlayerCharacter)target;
                BattleChara* bChara = (BattleChara*)pChara.Address;


                string startString = "[PetNicknames(2)]\n";
                startString += bChara->NameString + "\n";
                startString += bChara->HomeWorld + "\n";
                startString += bChara->ContentId + "\n";
                startString += "[-411,-417,-416,-415,-407]";

                BattleChara* bPet = CharacterManager.Instance()->LookupPetByOwnerObject(bChara);
                if (bPet != null)
                {
                    int id = -bPet->Character.CharacterData.ModelCharaId;
                    startString += $"{id}^[Test Battle Pet IPC Name]\n";
                }
                Character* bMinion = &bChara->Character.CompanionObject->Character;
                if (bMinion != null)
                {
                    int id = bMinion->CharacterData.ModelCharaId;
                    startString += $"\n{id}^[Test Companion IPC Name]";
                }

                ipc.SetPlayerDataDetour(playerCharacter!, startString);
            }
        }

        if (!IPCData.IsNullOrWhitespace())
        {
            if (ImGui.Button("Clear IPC Data##clear_ipc"))
            {
                IPCData = string.Empty;
            }
            ImGui.Text(IPCData);
        }
    }

    void RenderMode1()
    {

    }

    void RenderMode2()
    {

    }

    void RenderMode3()
    {

    }

    void RenderMode4()
    {

    }
}


