using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Core.Chat.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.PettableUserSystem.Pet;

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

        GameObjectID emoteTarget = bChara->Character.EmoteController.Target;
        if (emoteTarget.Type != 0 && emoteTarget.Type != 4) return;

        if(emoteTarget.Type == 4)
            emoteTarget.ObjectID = bChara->Character.CompanionObject->Character.GameObject.ObjectID;

        string baseName = string.Empty;
        string customName = string.Empty;

        foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
        {
            if (!user.HasAny) continue;

            foreach(PetBase pet in user.PetDatas)
            {
                if (!pet.Has) continue;
                if (pet.ID < -1 && !PluginLink.Configuration.replaceEmotesBattlePets) continue;
                if (pet.ID > -1 && !PluginLink.Configuration.replaceEmotesOnMinions) continue;
                if (pet.ObjectID == emoteTarget.ObjectID)
                {
                    baseName = pet.BaseName;
                    customName = pet.CustomName;
                    break;
                }
            }
        }

        StringUtils.instance.ReplaceSeString(ref message, baseName, customName);
    }
}
