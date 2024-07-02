using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem.Enums;
using PetRenamer.Core.Serialization;
using PetRenamer.Windows.Attributes;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
internal unsafe class DeveloperWindow : PetWindow
{

    public DeveloperWindow() : base("Dev Window Pet Renamer")
    {
        if(PluginLink.Configuration.debugMode && PluginLink.Configuration.autoOpenDebug) IsOpen = true;
    }

   
    public override void OnDraw()
    {
        if (Button("Add All"))
        {
            for (int i = 0; i < PluginHandlers.ObjectTable.Length - 1; i += 2)
            {
                IGameObject? gObj = PluginHandlers.ObjectTable[i];
                if (gObj == null) continue;
                if (gObj.ObjectKind != Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Player) continue;
                int minionTarget = i + 1;
                IGameObject? companion = PluginHandlers.ObjectTable[i];
                if (companion == null) continue;
                if (companion.ObjectKind != Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Companion) continue;
                Companion* cPointer = (Companion*)companion.Address;

                List<int> ids = new List<int>() { cPointer->Character.CharacterData.ModelCharaId };
                List<string> names = new List<string>() { "[TESTNAME]" };

                PluginLink.PettableUserHandler.DeclareUser(
                    new SerializableUserV3(
                        ids.ToArray(),
                        names.ToArray(),
                        gObj.Name.ToString(),
                        (ushort)PluginHandlers.ClientState.LocalPlayer!.HomeWorld.Id), UserDeclareType.Add, false, false);
            }
        }

        if (Button("Remove All"))
        {

        }
    }
}