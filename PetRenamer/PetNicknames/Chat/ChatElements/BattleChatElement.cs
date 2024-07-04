using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using PetRenamer.PetNicknames.Chat.Base;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.Chat.ChatElements;

internal unsafe class BattleChatElement : RestrictedChatElement
{
    IPettableUserList UserList { get; init; }
    IPetServices PetServices { get; init; }

    public BattleChatElement(IPetServices petServices, IPettableUserList userList)
    {
        UserList = userList;
        PetServices = petServices;
        RegisterChat(2091, 2219, 16427);
    }

    internal override void OnRestrictedChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        string stringMessage = message.TextValue;
        BattleChara* target = PetServices.PetCastHelper.LastCastTarget;
        BattleChara* dealer = PetServices.PetCastHelper.LastCastDealer;
        int lastCastID = PetServices.PetCastHelper.LastCastID;

        if (dealer == null) return;

        IPettableUser? user = UserList.GetUser((nint)dealer);
        if (user == null) return;
        if (!user.IsActive) return;

        IPettableDatabaseEntry databaseEntry = user.DataBaseEntry;
        INamesDatabase nameDataBase = databaseEntry.ActiveDatabase;
        for (int i = 0; i < nameDataBase.IDs.Length; i++)
        {
            int id = nameDataBase.IDs[i];
            if (id > -1) continue;

            PetSheetData? sheetData = PetServices.PetSheets.GetPet(id);
            if (sheetData == null) continue;
            if (!sheetData.Value.Contains(stringMessage)) continue;

            int softSkeletonID = PetServices.PetSheets.ToSoftSkeleton(id, databaseEntry.SoftSkeletons);
            string? customName = nameDataBase.GetName(softSkeletonID);
            if (customName == null) break;

            PetServices.StringHelper.ReplaceSeString(ref message, customName, sheetData.Value);
            break;
        }
    }
}
