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
    int lastJob = -1;
    bool lastHasPetOut = false;
    bool lastPetBeenTrue = false;
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
        GameObject* selfObject = GameObjectManager.GetGameObjectByIndex(0);
        if(selfObject == null) return;

        bool hasPetOut = false;
        bool petBeenTrue = false;
        bool flickAtEnd = false;
        
        for (int i = 0; i < 2000; i++)
        {
            bool currentIsPet = false;
            GameObject* me = GameObjectManager.GetGameObjectByIndex(i);
            if (me == null) continue;
            if (!me->IsCharacter()) continue;
            Character* playerCharacter = (Character*)me;
            if (playerCharacter == null) continue;
            FFCompanion* playerCompanion = playerCharacter->Companion.CompanionObject;

            if (playerCharacter->GameObject.OwnerID == selfObject->ObjectID)
            {
                currentIsPet = true;
                hasPetOut = true;
                petBeenTrue = true;
                Utf8String petString = new Utf8String();
                petString.SetString(playerCharacter->GameObject.Name);
            }
            string objectName;
            ushort objectHomeworld;
            if (!currentIsPet) {
                objectName = stringUtils.FromBytes(stringUtils.GetBytes(me->Name)).Replace(((char)0).ToString(), "");
                objectHomeworld = playerCharacter->HomeWorld;
            }
            else
            {
                objectName = stringUtils.FromBytes(stringUtils.GetBytes(selfObject->Name)).Replace(((char)0).ToString(), "");
                objectHomeworld = ((Character*)selfObject)->HomeWorld;
            }

            int currentID = -1;
            int currentJob = ((Character*)selfObject)->CharacterData.ClassJob;

            if (playerCompanion != null)
                currentID = playerCompanion->Character.CharacterData.ModelSkeletonId;

            if (currentIsPet)
            {
                if (currentJob == 26 || currentJob == 27)
                    currentID = -2;
                else if(currentJob == 28)
                    currentID = -3;
            }

            foreach (SerializableUser user in PluginLink.Configuration.serializableUsers!)
            {
                if (user == null) continue;
                if (objectName != user.username) continue;
                if (objectHomeworld != user.homeworld) continue;

                string currentName;
                SerializableNickname serializableNickname = nicknameUtils.GetNickname(user, currentID);
                if (currentIsPet)
                    currentName = sheetUtils.GetBattlePetName(playerCharacter->CharacterData.ModelCharaId);
                else
                    currentName = sheetUtils.GetPetName(currentID);
                if (PluginLink.Configuration.displayCustomNames && currentName.Length != 0 && serializableNickname != null)
                    currentName = serializableNickname.Name;

                if (i == 0 && (currentID != lastID || currentName != lastName || lastJob != currentJob || lastHasPetOut != hasPetOut))
                {
                    lastID = currentID;
                    lastName = currentName;
                    lastJob = currentJob;
                    lastHasPetOut = hasPetOut;

                    flickAtEnd = true;
                }

                if (playerCompanion != null)
                {
                    byte[] outcome = new byte[PluginConstants.ffxivNameSize];
                    Marshal.Copy((nint)Utf8String.FromString(currentName)->StringPtr, outcome, 0, PluginConstants.ffxivNameSize);
                    Marshal.Copy(outcome, 0, (nint)playerCompanion->Character.GameObject.Name, PluginConstants.ffxivNameSize);
                }

                if(currentIsPet && playerCharacter != null)
                {
                    byte[] outcome = new byte[PluginConstants.ffxivNameSize];
                    Marshal.Copy((nint)Utf8String.FromString(currentName)->StringPtr, outcome, 0, PluginConstants.ffxivNameSize);
                    Marshal.Copy(outcome, 0, (nint)playerCharacter->GameObject.Name, PluginConstants.ffxivNameSize);
                }
            }
        }
        if (flickAtEnd || lastPetBeenTrue != petBeenTrue)
        {
            lastPetBeenTrue = petBeenTrue;
            onCompanionChange?.Invoke(playerUtils.GetPlayerData(petBeenTrue), null);
        }
    }
}
