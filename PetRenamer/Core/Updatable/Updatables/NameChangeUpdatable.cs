using Dalamud.Game;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using PetRenamer.Core.Serialization;
using System.Runtime.InteropServices;

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

    unsafe public override void Update(Framework frameWork)
    {
        if (!playerUtils.PlayerDataAvailable()) return;
        PlayerData? playerData = playerUtils.GetPlayerData();
        int currentID = -1;
        string currentName;

        if (playerData == null) return;
        if (playerData!.Value.companionData != null)
            currentID = playerData!.Value.companionData!.Value.currentModelID;
        SerializableNickname serializableNickname = nicknameUtils.GetLocalNickname(currentID);
        if (serializableNickname == null) currentName = sheetUtils.GetCurrentPetName();
        else currentName = serializableNickname.Name;

        if (currentID != lastID || currentName != lastName)
        {
            lastID = currentID;
            lastName = currentName;

            SerializableNickname nickname = nicknameUtils.GetLocalNickname(currentID);

            onCompanionChange?.Invoke(playerData, nickname);
        }

        if (!PluginLink.Configuration.displayCustomNames || currentName.Trim().Normalize().ToLower().Length == 0)
            currentName = sheetUtils.GetCurrentPetName();

        Marshal.Copy(stringUtils.GetBytes(currentName), 0, (nint)playerData!.Value.companionData!.Value.namePtr, PluginConstants.ffxivNameSize);
    }
}
