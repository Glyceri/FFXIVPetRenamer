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
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable]
internal class NameChangeUpdatable : Updatable
{
    int lastID = -1;
    int lastBattleID = -1;
    int lastJob = -1;
    bool lastHasPetOut = false;
    bool lastPetBeenTrue = false;
    string lastName = null!;
    string lastBattleName = null!;

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

    List<FoundPlayerCharacter> characters = new List<FoundPlayerCharacter>();
    unsafe void FillUserList()
    {
        characters.Clear();
        for (int i = 0; i < 2000; i++)
        {
            GameObject* currentObject = GameObjectManager.GetGameObjectByIndex(i);
            if (currentObject == null) continue;

            Character* plCharacter = (Character*)currentObject;
            if (plCharacter == null) continue;

            string name = stringUtils.GetCharacterName(currentObject->Name);
            ushort homeWorld = plCharacter->HomeWorld;

            SerializableUser? correctUser = null;

            foreach (SerializableUser user in PluginLink.Configuration.serializableUsers!)
            {
                if (user == null) continue;
                if (user.username != name) continue;
                if (user.homeworld != homeWorld) continue;
                correctUser = user;
                break;
            }
            if (correctUser == null)
            {
                foreach (FoundPlayerCharacter character in characters)
                {
                    if (character.GetOwnID() == currentObject->OwnerID)
                    {
                        character.battlePetCharacter = plCharacter;
                        break;
                    }
                }
                continue;
            }

            FFCompanion* plCompanion = plCharacter->Companion.CompanionObject;
            characters.Add(new FoundPlayerCharacter()
            {
                selfGameObject = currentObject,
                playerCharacter = plCharacter,
                playerCompanion = plCompanion,
                associatedUser = correctUser,
                ownName = name,
                ownHomeWorld = homeWorld,
            });
        }
    }

    unsafe void LoopUserList()
    {
        Dalamud.Game.ClientState.Objects.SubKinds.PlayerCharacter playerChar = PluginHandlers.ClientState.LocalPlayer!;
        bool flickAtEnd = false;
        bool battlePetBeenActive = false;

        foreach (FoundPlayerCharacter character in characters)
        {
            bool isLocalPlayer = character.ownName == playerChar.Name.ToString() && character.ownHomeWorld == playerChar.HomeWorld.Id;

            bool hasCompanion = character.HasCompanion();
            bool hasBattlePet = character.HasBattlePet();

            int currentID = -1;
            string currentName = string.Empty;

            int currentIDBattlePet = -1;
            string currentNameBattlePet = string.Empty;

            int currentJob = character.GetPlayerJob();

            if (hasCompanion)
            {
                currentID = character.GetCompanionID();
                currentName = sheetUtils.GetPetName(currentID);
                SetName(character, currentID, ref currentName);
                ApplyName(character.GetCompanionName(), currentName);
            }

            if(hasBattlePet && character.BattlePetNamingAllowed()) 
            {
                battlePetBeenActive = true;
                currentID = character.GetBattlePetID();
                currentIDBattlePet = character.GetBattlePetModelID();
                currentNameBattlePet = sheetUtils.GetBattlePetName(currentIDBattlePet);
                SetName(character, currentID, ref currentNameBattlePet);
                ApplyName(character.GetBattlePetName(), currentNameBattlePet);
            }

            if (isLocalPlayer) 
                flickAtEnd = HasChanged(currentID, currentIDBattlePet, currentName, currentNameBattlePet, character.GetPlayerJob(), hasBattlePet);
        }

        if (!flickAtEnd && battlePetBeenActive == lastPetBeenTrue) return;

        lastPetBeenTrue = battlePetBeenActive;
        onCompanionChange?.Invoke(playerUtils.GetPlayerData(battlePetBeenActive), null);
    }

    unsafe void ApplyName(byte* namePtr, string newName)
    {
        byte[] outcome = new byte[PluginConstants.ffxivNameSize];
        Marshal.Copy((nint)Utf8String.FromString(newName)->StringPtr, outcome, 0, PluginConstants.ffxivNameSize);
        Marshal.Copy(outcome, 0, (nint)namePtr, PluginConstants.ffxivNameSize);
    }

    bool HasChanged(int currentID, int currentIDBattlePet, string currentName, string currentBattleName, byte currentJob, bool hasBattlePet)
    {
        if (currentID != lastID || lastBattleID != currentIDBattlePet || currentName != lastName || lastJob != currentJob || lastHasPetOut != hasBattlePet)
        {
            lastID = currentID;
            lastBattleID = currentIDBattlePet;
            lastName = currentName;
            lastBattleName = currentBattleName;
            lastJob = currentJob;
            lastHasPetOut = hasBattlePet;

            return true;
        }
        return false;
    }

    unsafe void SetName(FoundPlayerCharacter character, int id, ref string name)
    {
        SerializableNickname serializableNickname = nicknameUtils.GetNickname(character.associatedUser!, id);
        if (serializableNickname == null) return;
        if (serializableNickname.Name.Length == 0 || !PluginLink.Configuration.displayCustomNames || name.Length == 0) return;
        name = serializableNickname.Name;
    }

    public override unsafe void Update(Framework frameWork)
    {
        if (PluginHandlers.ClientState.LocalPlayer! == null) return;
        FillUserList();
        LoopUserList();
    }



    private unsafe class FoundPlayerCharacter
    {
        public GameObject* selfGameObject;
        public Character* playerCharacter;
        public Character* battlePetCharacter;
        public FFCompanion* playerCompanion;
        public SerializableUser? associatedUser;

        public string ownName = string.Empty;
        public uint ownHomeWorld = 0;

        public bool BattlePetNamingAllowed() => PluginConstants.allowedJobs.Contains(GetPlayerJob());
        public bool HasBattlePet() => battlePetCharacter != null;
        public bool HasCompanion() => playerCompanion != null;

        public byte* GetBattlePetName() => battlePetCharacter->GameObject.Name;
        public byte* GetCompanionName() => playerCompanion->Character.GameObject.Name;

        public int GetCompanionID() => playerCompanion->Character.CharacterData.ModelSkeletonId;
        public int GetBattlePetID() => PluginLink.Utils.Get<RemapUtils>().GetPetIDFromClass(GetPlayerJob());

        public int GetBattlePetModelID() => battlePetCharacter->CharacterData.ModelCharaId;

        public uint GetOwnID() => selfGameObject->ObjectID;
        public byte GetPlayerJob() => playerCharacter->CharacterData.ClassJob;
    }
}