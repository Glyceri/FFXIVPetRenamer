using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.PetNicknames.Chat.Interfaces;
using Dalamud.Game;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.Chat.ChatElements;

internal unsafe class EmoteChatElement : IChatElement
{
    DalamudServices DalamudServices { get; init; }
    IPettableUserList UserList { get; init; }
    IPetServices PetServices { get; init; }

    public EmoteChatElement(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList) 
    {
        DalamudServices = dalamudServices;
        UserList = userList;
        PetServices = petServices;
    }

    public void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (type != XivChatType.StandardEmote) return;

        BattleChara* bChara = CharacterManager.Instance()->LookupBattleCharaByName(sender.ToString(), true);
        if (bChara == null) return;

        nint value = nint.Zero;

        GameObjectId emoteTarget = bChara->Character.EmoteController.Target;
        if (emoteTarget.Type != 0 && emoteTarget.Type != 4) return;

        foreach(IPettableUser? user in UserList.pettableUsers)
        {
            if (user == null) continue;
            if (!user.IsActive) continue;
            IPettablePet? pet = user.GetPet(emoteTarget);
            if (pet == null) continue; 
            string? customName = pet.CustomName;
            if (customName == null) continue; 
            PetSheetData? data = pet.PetData;
            if (data == null) continue;
            PetSheetData pData = data.Value;
            PetServices.StringHelper.ReplaceSeString(ref message, customName, pData, !(DalamudServices.ClientState.ClientLanguage == ClientLanguage.Japanese));
            break;
        }
    }
}

