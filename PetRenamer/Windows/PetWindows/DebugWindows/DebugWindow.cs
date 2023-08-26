using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using CSGameObject = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;
using CSCharacter = FFXIVClientStructs.FFXIV.Client.Game.Character.Companion;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using System.Buffers.Text;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.Windows.PetWindows;
#if DEBUG
[PersistentPetWindow]
#endif
public class DebugWindow : PetWindow
{
    public DebugWindow() : base(
        "Debug Window PetRenamer",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoFocusOnAppearing)
    {
        Size = new Vector2(232, 225);
        SizeCondition = ImGuiCond.Always;

        IsOpen = true;
    }

    public unsafe override void OnDraw()
    {
        if (Button("Add user")) 
        {
            if (PluginHandlers.TargetManager.Target == null) return;
            Character chara = (Character)PluginHandlers.TargetManager.Target;
            if (chara == null) return;
            CSGameObject* gObj = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObjectManager.GetGameObjectByIndex(chara.ObjectIndex + 1);
            if (gObj == null) return;
            CSCharacter* companion = (CSCharacter*)gObj;
            if(companion == null) return;

            ConfigurationUtils.instance.AddNewUserV2(
                new SerializableUserV2(new SerializableNickname[5]
                {
                    new SerializableNickname( companion->Character.CharacterData.ModelSkeletonId, "Minion test!"),
                    new SerializableNickname(-2, "Test1"), 
                    new SerializableNickname(-3, "Test2"), 
                    new SerializableNickname(-4, "Test3"), 
                    new SerializableNickname(-5, "Test4") }, 
                    chara.Name.ToString(),
                (ushort)PluginHandlers.ClientState.LocalPlayer!.HomeWorld.Id));
        }

        if (Button("Add All users"))
        {
            for (int i = 2; i < 200; i += 2)
            {
                CSGameObject* gObj = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObjectManager.GetGameObjectByIndex(i);
                if (gObj == null) continue;
                CSGameObject* gObj2 = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObjectManager.GetGameObjectByIndex(i + 1);
                if (gObj2 == null) continue;
                CSCharacter* companion = (CSCharacter*)gObj2;
                if (companion == null) continue;
                ConfigurationUtils.instance.AddNewUserV2(
                   new SerializableUserV2(new SerializableNickname[5]
                   {
                    new SerializableNickname( companion->Character.CharacterData.ModelSkeletonId, "Minion test [ALL]!"),
                    new SerializableNickname(-2, "Test1"),
                    new SerializableNickname(-3, "Test2"),
                    new SerializableNickname(-4, "Test3"),
                    new SerializableNickname(-5, "Test4") },
                       Marshal.PtrToStringUTF8((IntPtr)gObj->Name) ?? string.Empty,
                   (ushort)PluginHandlers.ClientState.LocalPlayer!.HomeWorld.Id), true);
            }
        }

        if (Button("Remove all test users"))
        {
            List<SerializableUserV2> users = PluginLink.Configuration.serializableUsersV2!.ToList();

            for(int i = users.Count - 1; i >= 0; i--)
            {
                foreach (SerializableNickname nickname in users[i].nicknames)
                {
                    if (nickname.Name == "Minion test [ALL]!" || nickname.Name == "Minion test!")
                    {
                        users.Remove(users[i]);
                        break;
                    }
                }
            }
            PluginLink.Configuration.serializableUsersV2 = users.ToArray();
        }
    }
}
