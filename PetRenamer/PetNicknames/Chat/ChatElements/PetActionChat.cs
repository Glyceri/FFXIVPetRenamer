﻿using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.Chat.Base;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Chat.ChatElements;

internal class PetActionChat : RestrictedChatElement
{
    readonly IPetServices PetServices;
    readonly IPettableUserList UserList;

    public PetActionChat(in IPetServices petServices, in IPettableUserList userList)
    {
        PetServices = petServices;
        UserList = userList;

        RegisterChat(2105, 2106);
    }

    internal unsafe override void OnRestrictedChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!PetServices.PetActionHelper.LastValid) return;
        if (!PetServices.Configuration.showInBattleChat) return;

        BattleChara* owner = PetServices.PetActionHelper.LastUser;
        if (owner == null) return;

        IPettableUser? user = UserList.GetUser((nint)owner);
        if (user == null) return;
        if (!user.IsActive) return;

        IPettablePet? battlePet = user.GetYoungestPet(IPettableUser.PetFilter.BattlePet);
        if (battlePet == null) return;

        IPetSheetData? petData = battlePet.PetData;
        if (petData == null) return;

        string? customName = user.GetCustomName(petData);
        if (customName == null) return;

        PetServices.StringHelper.ReplaceSeString(ref message, customName, petData);
    }
}