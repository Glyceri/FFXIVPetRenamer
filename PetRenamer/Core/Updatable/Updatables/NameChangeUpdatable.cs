using Dalamud.Game;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using PetRenamer.Core.Serialization;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFCompanion = FFXIVClientStructs.FFXIV.Client.Game.Character.Companion;
using FFXIVClientStructs.FFXIV.Client.System.String;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable]
internal class NameChangeUpdatable : Updatable
{
    int lastID = -1;
    string lastName = null!;

    StringUtils stringUtils;
    NicknameUtils nicknameUtils;
    PlayerUtils playerUtils;
    SheetUtils sheetUtils;

    internal delegate void OnCompanionChange(PlayerData? playerData, SerializableNickname? serializableNickname);
    internal OnCompanionChange onCompanionChange = null!;

    public NameChangeUpdatable()
    {
        stringUtils = PluginLink.Utils.Get<StringUtils>();
        nicknameUtils = PluginLink.Utils.Get<NicknameUtils>();
        playerUtils = PluginLink.Utils.Get<PlayerUtils>();
        sheetUtils = PluginLink.Utils.Get<SheetUtils>();
    }

#pragma warning disable CS8601 // Possible null reference assignment. (It's legit impossible for it to be null intelliSense 😤😤😤) 
    public void RegisterMethod(OnCompanionChange companionChange)
    {
        if (onCompanionChange == null)
            onCompanionChange = (change, serializableNickname) => { };

        if (companionChange != null)
        {
            onCompanionChange -= companionChange;
            onCompanionChange += companionChange;
        }
    }
#pragma warning restore CS8601 // Possible null reference assignment.

    public override unsafe void Update(Framework frameWork)
    {
        if (PluginHandlers.ClientState.LocalPlayer! == null) return;
        for (int i = 0; i < 200; i++)
        {
            GameObject* me = GameObjectManager.GetGameObjectByIndex(i);
            if (me == null) continue;
            if (!me->IsCharacter()) continue;
            Character* playerCharacter = (Character*)me;
            if (playerCharacter == null) continue;
            FFCompanion* meCompanion = (FFCompanion*)me;
            if (meCompanion == null) continue;
            FFCompanion* playerCompanion = meCompanion->Character.Companion.CompanionObject;
            

            string objectName = stringUtils.FromBytes(stringUtils.GetBytes(me->Name)).Replace(((char)0).ToString(), "");
            ushort objectHomeworld = playerCharacter->HomeWorld;

            int currentID = -1;
            
            if(playerCompanion != null)
                currentID = playerCompanion->Character.CharacterData.ModelSkeletonId;

            foreach (SerializableUser user in PluginLink.Configuration.serializableUsers!)
            {
                if (user == null) continue;
                if (objectName != user.username) continue;
                if (objectHomeworld != user.homeworld) continue;

                string currentName;
                SerializableNickname serializableNickname = nicknameUtils.GetNickname(user, currentID);
                currentName = sheetUtils.GetPetName(currentID);
                if (PluginLink.Configuration.displayCustomNames && currentName.Length != 0 && serializableNickname != null)
                    currentName = serializableNickname.Name;

                if (i == 0 && (currentID != lastID || currentName != lastName))
                {
                    lastID = currentID;
                    lastName = currentName;

                    onCompanionChange?.Invoke(playerUtils.GetPlayerData(), serializableNickname);
                }

                if (playerCompanion == null) break;
                byte[] outcome = new byte[PluginConstants.ffxivNameSize];
                Marshal.Copy((nint)Utf8String.FromString(currentName)->StringPtr, outcome, 0, PluginConstants.ffxivNameSize);
                Marshal.Copy(outcome, 0, (nint)playerCompanion->Character.GameObject.Name, PluginConstants.ffxivNameSize);
            }
        }
    }
}
