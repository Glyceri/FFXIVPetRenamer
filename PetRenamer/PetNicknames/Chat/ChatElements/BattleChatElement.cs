using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using PetRenamer.PetNicknames.Chat.Base;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Chat.ChatElements;

internal unsafe class BattleChatElement : RestrictedChatElement
{
    readonly IPettableUserList UserList;
    readonly IPetServices PetServices;

    public BattleChatElement(in IPetServices petServices, in IPettableUserList userList)
    {
        UserList = userList;
        PetServices = petServices;
        RegisterChat(2091, 2219, 16427);
    }

    internal override void OnRestrictedChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!PetServices.Configuration.showInBattleChat) return;

        BattleChara* dealer = PetServices.PetCastHelper.LastCastDealer;
        int lastCastID = PetServices.PetCastHelper.LastCastID;

        if (dealer == null) return;

        IPettableUser? user = UserList.GetUser((nint)dealer);
        if (user == null) return;
        if (!user.IsActive) return;

        IPetSheetData? petData;

        IPettablePet ? battlePet = UserList.GetPet((nint)dealer);

        if (battlePet == null) petData = PetServices.PetSheets.GetPetFromAction((uint)lastCastID, in user, true);
        else petData = battlePet.PetData;
        
        if (petData == null) return;

        string? customName = user.GetCustomName(petData);
        if (customName == null) return;

        PetServices.StringHelper.ReplaceSeString(ref message, customName, petData);
    }
}
