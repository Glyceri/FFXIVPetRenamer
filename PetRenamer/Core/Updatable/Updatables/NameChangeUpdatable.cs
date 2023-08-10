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
using System.Linq;
using System;
using Dalamud.Logging;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable]
internal class NameChangeUpdatable : Updatable
{
    int lastID = -1;
    int lastBattleID = -1;
    int lastJob = -1;
    int lastBattleSkeletonID = -1;
    bool lastHasPetOut = false;
    bool lastPetBeenTrue = false;
    string lastName = null!;
    string lastBattleName = null!;

    internal delegate void OnCompanionChange(PlayerData? playerData, SerializableNickname? serializableNickname);
    internal OnCompanionChange onCompanionChange = null!;

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


    unsafe void FillUserList()
    {
        PluginLink.IpcStorage.characters.Clear();
        /*
        foreach (SerializableUserV2 user in PluginLink.Configuration.serializableUsersV2!)
        {
            BattleChara* battleChara = PluginLink.CharacterManager->LookupBattleCharaByName(user.username, true, (short)user.homeworld);
            if (battleChara == null) continue;
            FFCompanion* companion = battleChara->Character.Companion.CompanionObject;
            BattleChara* pet =  PluginLink.CharacterManager->LookupPetByOwnerObject(battleChara);
            PluginLog.Log("---------------------------------");
            Utf8String str = new Utf8String();
            if (battleChara != null)
            {
                str.SetString(battleChara->Character.GameObject.Name);
                PluginLog.Log("Player Name: " + str.ToString());
            }
            if (pet != null)
            {
                str.SetString(pet->Character.GameObject.Name);
                PluginLog.Log("Pet Name: " + str.ToString());
            }
            if (companion != null)
            {
                str.SetString(companion->Character.GameObject.Name);
                PluginLog.Log("Minion Name: " + str.ToString());
            }
        }

        return;*/
        for (int i = 0; i < 2000; i++)
        {
            GameObject* currentObject = GameObjectManager.GetGameObjectByIndex(i);
            if (currentObject == null) continue;
            if (!currentObject->IsCharacter()) continue;
            Character* plCharacter = (Character*)currentObject;
            if (plCharacter == null) continue;

            string name = StringUtils.instance.GetCharacterName(currentObject->Name);
            ushort homeWorld = plCharacter->HomeWorld;

            SerializableUserV2? correctUser = null;

            foreach (SerializableUserV2 user in PluginLink.Configuration.serializableUsersV2!)
            {
                if (user == null) continue;
                if (user.username != name) continue;
                if (user.homeworld != homeWorld) continue;
                correctUser = user;
                break;
            }
            if (correctUser == null)
            {
                foreach (FoundPlayerCharacter character in PluginLink.IpcStorage.characters)
                {
                    if (character.GetOwnID() == currentObject->OwnerID)
                    {
                        if (RemapUtils.instance.battlePetRemap.ContainsKey(plCharacter->CharacterData.ModelCharaId))
                        {
                            character.battlePetCharacter = plCharacter;
                            break;
                        }
                    }
                }
                continue;
            }

            FFCompanion* plCompanion = plCharacter->Companion.CompanionObject;
            PluginLink.IpcStorage.characters.Add(new FoundPlayerCharacter()
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
        if (playerChar == null) return;
        bool flickAtEnd = false;
        bool battlePetBeenActive = false;

        foreach (FoundPlayerCharacter character in PluginLink.IpcStorage.characters)
        {
            bool isLocalPlayer = character.ownName == playerChar.Name.ToString() && character.ownHomeWorld == playerChar.HomeWorld.Id;

            bool hasCompanion = character.HasCompanion();
            bool hasBattlePet = character.HasBattlePet();

            int currentJob = character.GetPlayerJob();

            int currentID = -1;
            int currentIDBattlePet = -1;
            int currentbattID = -1;
            string currentName = string.Empty;
            string currentNameBattlePet = string.Empty;

            if (hasCompanion)
            {
                currentID = character.GetCompanionID();
                currentName = SheetUtils.instance.GetPetName(currentID);
                SetName(character, currentID, ref currentName);
                ApplyName(character.GetCompanionName(), currentName);
            }

            if (hasBattlePet && character.BattlePetNamingAllowed())
            {
                battlePetBeenActive = true;
                currentbattID = character.GetBattlePetID();
                currentIDBattlePet = character.GetBattlePetModelID();
                currentNameBattlePet = SheetUtils.instance.GetBattlePetName(currentIDBattlePet);
                SetName(character, currentbattID, ref currentNameBattlePet);
                ApplyName(character.GetBattlePetName(), currentNameBattlePet);
            }

            if (isLocalPlayer)
                flickAtEnd = HasChanged(currentID, currentIDBattlePet, currentbattID, currentName, currentNameBattlePet, character.GetPlayerJob(), hasBattlePet);
        }

        if (!flickAtEnd && battlePetBeenActive == lastPetBeenTrue) return;

        lastPetBeenTrue = battlePetBeenActive;
        onCompanionChange?.Invoke(PlayerUtils.instance.GetPlayerData(battlePetBeenActive), null);


    }

    unsafe void ApplyName(byte* namePtr, string newName)
    {
        if (PluginLink.Configuration.useNewNameSystem) return;
        byte[] outcome = new byte[PluginConstants.ffxivNameSize];
        Marshal.Copy((nint)Utf8String.FromString(newName)->StringPtr, outcome, 0, PluginConstants.ffxivNameSize);
        Marshal.Copy(outcome, 0, (nint)namePtr, PluginConstants.ffxivNameSize);
    }

    bool HasChanged(int currentID, int currentIDBattlePet, int currentBattleSkeletonID, string currentName, string currentBattleName, byte currentJob, bool hasBattlePet)
    {
        if (currentID != lastID || lastBattleID != currentIDBattlePet || currentName != lastName || currentBattleName != lastBattleName || lastJob != currentJob || lastHasPetOut != hasBattlePet)
        {
            lastID = currentID;
            lastBattleID = currentIDBattlePet;
            lastName = currentName;
            lastBattleName = currentBattleName;
            lastJob = currentJob;
            lastHasPetOut = hasBattlePet;
            lastBattleSkeletonID = currentBattleSkeletonID;

            string localCurrentName = currentName;
            string localCurrentBattleName = currentBattleName;
            if (localCurrentName == SheetUtils.instance.GetCurrentPetName()) localCurrentName = string.Empty;
            if (localCurrentBattleName == RemapUtils.instance.PetIDToName(RemapUtils.instance.GetPetIDFromClass(currentJob))) localCurrentBattleName = string.Empty;

            IpcProvider.ChangedPetNickname(new NicknameData(currentID, localCurrentName, RemapUtils.instance.GetPetIDFromClass(currentJob), localCurrentBattleName));
            return true;
        }
        return false;
    }

    unsafe void SetName(FoundPlayerCharacter character, int id, ref string name)
    {
        SerializableNickname serializableNickname = NicknameUtils.instance.GetNicknameV2(character.associatedUser!, id);
        if (serializableNickname == null) return;
        if (serializableNickname.Name.Length == 0 || !PluginLink.Configuration.displayCustomNames || name.Length == 0) return;
        name = serializableNickname.Name;
    }


    public override unsafe void Update(Framework frameWork)
    {
        if (PluginHandlers.ClientState.LocalPlayer! == null) return;
        lock (PluginLink.IpcStorage.characters)
        {
            FillUserList();
            LoopUserList();
        }
    }
}

public unsafe class FoundPlayerCharacter
{
    public GameObject* selfGameObject;
    public Character* playerCharacter;
    public Character* battlePetCharacter;
    public FFCompanion* playerCompanion;
    public SerializableUserV2? associatedUser;

    public string ownName = string.Empty;
    public uint ownHomeWorld = 0;

    public bool BattlePetNamingAllowed() => PluginConstants.allowedJobs.Contains(GetPlayerJob());
    public bool HasBattlePet() => battlePetCharacter != null;
    public bool HasCompanion() => playerCompanion != null;

    public byte* GetBattlePetName() => battlePetCharacter->GameObject.Name;
    public byte* GetCompanionName() => playerCompanion->Character.GameObject.Name;

    public int GetCompanionID() => playerCompanion->Character.CharacterData.ModelSkeletonId;
    public int GetBattlePetID() => RemapUtils.instance.GetPetIDFromClass(GetPlayerJob());

    public int GetBattlePetModelID() => battlePetCharacter->CharacterData.ModelCharaId;

    public uint GetOwnID() => selfGameObject->ObjectID;
    public uint GetBattlePetObjectID() => battlePetCharacter->GameObject.ObjectID;
    public byte GetPlayerJob() => playerCharacter->CharacterData.ClassJob;
}