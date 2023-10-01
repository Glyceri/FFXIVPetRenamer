using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Core.Chat.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.PettableUserSystem.Pet;
using PetRenamer.Logging;

namespace PetRenamer.Core.Chat.ChatElements;

[Chat]
internal unsafe class PetChatEmoteElement : ChatElement
{
    internal override bool OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!PluginLink.Configuration.displayCustomNames) return false;
        if (type != XivChatType.StandardEmote && type != XivChatType.CustomEmote) return false;

        BattleChara* bChara = PluginLink.CharacterManager->LookupBattleCharaByName(sender.ToString(), true);
        if (bChara == null) return false;

        GameObjectID emoteTarget = bChara->Character.EmoteController.Target;
        if (emoteTarget.Type != 0 && emoteTarget.Type != 4) return false;

        if (emoteTarget.Type == 4)
            foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
                if (user.ObjectID == emoteTarget.ObjectID)
                    emoteTarget.ObjectID = user.Minion.ObjectID;
        
        foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
        {
            if (!user.HasAny) continue;

            foreach (PetBase pet in user.Pets)
            {
                if (!pet.Has) continue;
                // TODO: Make configuration better
                if (pet.ID < -1 && !PluginLink.Configuration.replaceEmotesBattlePets) continue;
                if (pet.ID > -1 && !PluginLink.Configuration.replaceEmotesOnMinions) continue;
                if (pet.ObjectID != emoteTarget.ObjectID) continue;

                (string, string)[] replaceNames = new (string, string)[] { (pet.BaseNamePlural, pet.CustomName), (pet.BaseName, pet.CustomName) };
                StringUtils.instance.ReplaceSeString(ref message, ref replaceNames);
                return true;
            }
        }
        return false;
    }
}
