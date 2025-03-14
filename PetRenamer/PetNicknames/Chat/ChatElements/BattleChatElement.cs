using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using PetRenamer.PetNicknames.Chat.Base;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Numerics;

namespace PetRenamer.PetNicknames.Chat.ChatElements;

internal unsafe class BattleChatElement : RestrictedChatElement
{
    readonly IPettableUserList UserList;
    readonly IPetServices PetServices;

    public BattleChatElement(IPetServices petServices, IPettableUserList userList)
    {
        UserList = userList;
        PetServices = petServices;
        RegisterChat(2091, 2219, 16427, 4139, 18475);
    }

    internal override void OnRestrictedChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!PetServices.Configuration.showInBattleChat) return;

        int lastCastID = PetServices.PetCastHelper.LastCastID;

        IPettableUser? user = UserList.GetUser(PetServices.PetCastHelper.LastCastDealer);
        if (user == null) return;
        if (!user.IsActive) return;

        IPetSheetData? petData;

        IPettablePet? battlePet = UserList.GetPet(PetServices.PetCastHelper.LastCastDealer);

        if (battlePet == null) petData = PetServices.PetSheets.GetPetFromAction((uint)lastCastID, in user, true);
        else petData = battlePet.PetData;
        
        if (petData == null) return;

        string? customName = user.GetCustomName(petData);
        if (customName == null) return;

        PetServices.StringHelper.ReplaceSeString(ref message, customName, petData, true);
    }
}
