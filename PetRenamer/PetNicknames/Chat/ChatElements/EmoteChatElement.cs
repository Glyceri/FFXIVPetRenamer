using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using PetRenamer.PetNicknames.Chat.Interfaces;
using Dalamud.Game;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Chat.ChatElements;

internal unsafe class EmoteChatElement : IChatElement
{
    private readonly DalamudServices   DalamudServices;
    private readonly IPettableUserList UserList;
    private readonly IPetServices      PetServices;

    public EmoteChatElement(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList) 
    {
        DalamudServices = dalamudServices;
        UserList        = userList;
        PetServices     = petServices;
    }

    private bool LanguageIsJapanese
        => DalamudServices.ClientState.ClientLanguage == ClientLanguage.Japanese;

    public void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (type != XivChatType.StandardEmote)
        {
            return;
        }

        if (!PetServices.Configuration.showOnEmotes)
        {
            return;
        }

        IPettableUser? senderUser = UserList.GetUser(sender.TextValue);

        if (senderUser == null)
        {
            PetServices.PetLog.LogVerbose($"Sender User is NULL [{sender.TextValue}].");

            return;
        }

        IPettableEntity? target = senderUser.TargetManager?.GetLeadingTarget();

        if (target == null)
        {
            PetServices.PetLog.LogVerbose($"Target is NULL [{sender.TextValue}].");

            return;
        }

        if (target is not IPettablePet pet)
        {
            PetServices.PetLog.LogVerbose($"Target is NOT IPettablePet [{target.GetType().Name}].");

            return;
        }

        IPettableUser? petOwner = pet.Owner;

        if (petOwner == null)
        {
            PetServices.PetLog.LogVerbose($"Target doesnt have an owner {target.Address}.");

            return;
        }

        if (!petOwner.IsActive)
        {
            PetServices.PetLog.LogVerbose($"Pet Owner is NOT an active user.");

            return;
        }

        string? customName = pet.CustomName;

        if (customName == null)
        {
            PetServices.PetLog.LogVerbose($"Pet Owner is NOT an active user.");

            return;
        }

        IPetSheetData? data = pet.PetData;

        if (data == null)
        {
            PetServices.PetLog.LogVerbose($"Pet Data is NULL.");

            return;
        }

        PetServices.StringHelper.ReplaceSeString(ref message, customName, data, !LanguageIsJapanese);
    }
}

