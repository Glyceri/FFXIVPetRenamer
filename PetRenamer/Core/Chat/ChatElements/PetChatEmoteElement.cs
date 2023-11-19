using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Core.Chat.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.PettableUserSystem.Pet;
using PetRenamer.Utilization.UtilsModule;

namespace PetRenamer.Core.Chat.ChatElements;

[Chat]
internal unsafe class PetChatEmoteElement : ChatElement
{
    internal override void OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!PluginLink.Configuration.displayCustomNames) return;
        if (type != XivChatType.StandardEmote && type != XivChatType.CustomEmote) return;

        BattleChara* bChara = PluginLink.CharacterManager->LookupBattleCharaByName(sender.ToString(), true);
        if (bChara == null) return;

        nint value = nint.Zero;

        GameObjectID emoteTarget = bChara->Character.EmoteController.Target;
        if (emoteTarget.Type != 0 && emoteTarget.Type != 4) return;

        if (emoteTarget.Type == 4)
            foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
                if (user.ObjectID == emoteTarget.ObjectID)
                {
                    value = user.Minion.Pet;
                    break;
                }

        foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
        {
            if (!user.HasAny) continue;

            foreach (PetBase pet in user.Pets)
            {
                if (!pet.Has) continue;
                // TODO: Make configuration better
                if (pet.ID < -1 && !PluginLink.Configuration.replaceEmotesBattlePets) continue;
                if (pet.ID > -1 && !PluginLink.Configuration.replaceEmotesOnMinions) continue;
                if (pet.ObjectID != emoteTarget.ObjectID && pet.Pet != value) continue;

                (string, string)[] replaceNames = new (string, string)[] { (pet.BaseNamePlural, pet.UsedName), (pet.BaseName, pet.UsedName) };
                StringUtils.instance.ReplaceSeString(ref message, ref replaceNames, !(PluginHandlers.ClientState.ClientLanguage == Dalamud.ClientLanguage.Japanese));
                return;
            }
        }
        return;
    }
}
