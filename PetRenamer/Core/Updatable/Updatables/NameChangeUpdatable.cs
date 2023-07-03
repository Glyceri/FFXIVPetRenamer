using Dalamud.Game;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using PetRenamer.Core.Serialization;
using System.Runtime.InteropServices;
using System.Xml.Linq;

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
        PlayerData? playerData = playerUtils.GetPlayerData();
        int currentID = -1;
        string currentName = null!;
        if (playerData == null) return;
        if (playerData!.Value.companionData != null)
            currentID = playerData!.Value.companionData!.Value.currentModelID;
        SerializableNickname serializableNickname = nicknameUtils.GetNickname(currentID);
        if (serializableNickname != null) currentName = serializableNickname.Name;

        if (currentID != lastID || currentName != lastName)
        {
            lastID = currentID;
            lastName = currentName;

            SerializableNickname nickname = nicknameUtils.GetNickname(currentID);

            onCompanionChange?.Invoke(playerData, nickname);

        }

        if (!PluginLink.Configuration.displayCustomNames || currentName.Trim().Normalize().ToLower().Length == 0)
            currentName = sheetUtils.GetCurrentPetName();

        Marshal.Copy(stringUtils.GetBytes(currentName), 0, (nint)playerData!.Value.companionData!.Value.companion->Character.GameObject.Name, PluginConstants.ffxivNameSize);
    }

    /*
    public override unsafe void Update(Framework frameWork)
    {
        GameObjectStruct* me = GameObjectManager.GetGameObjectByIndex(0);
        if (me == null) return;
        byte[] nameBytes = new byte[64];
        IntPtr intPtr = (nint)me->GetName();
        Globals.CurrentUserGender = me->Gender;
        Marshal.Copy(intPtr, nameBytes, 0, PluginConstants.ffxivNameSize);
        Globals.CurrentUserName = stringUtils.FromBytes(nameBytes);
        FFCompanion* meCompanion = (FFCompanion*)me;
        if (meCompanion == null) return;

        FFCompanion* playerCompanion = meCompanion->Character.Companion.CompanionObject;
        int lastID = Globals.CurrentID;
        Globals.CurrentID = -1;
        Globals.CurrentName = string.Empty;
        if (playerCompanion == null) return;



        int id = playerCompanion->Character.CharacterData.ModelSkeletonId;
        if (lastID != id) Globals.CurrentIDChanged = true;

        Globals.CurrentID = id;

        if (!PluginLink.Configuration.displayCustomNames) return;
        if (!nicknameUtils.Contains(Globals.CurrentID)) return;

        Globals.CurrentName = stringUtils.GetName(Globals.CurrentID);

        string usedName = Globals.CurrentName;

        byte* name = playerCompanion->Character.GameObject.GetName();
        Marshal.Copy(stringUtils.GetBytes(usedName), 0, (nint)name, PluginConstants.ffxivNameSize);


        if (Globals.RedrawPet)
        {
            playerCompanion->Character.GameObject.DisableDraw();
            playerCompanion->Character.GameObject.EnableDraw();
        }

        Globals.RedrawPet = false;
    }*/
}
