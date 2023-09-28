using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Core.Chat.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Core.PettableUserSystem;

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

            if (user.HasBattlePet && PluginLink.Configuration.replaceEmotesBattlePets)
            {
                if (user.BattlePet->Character.GameObject.ObjectID == emoteTarget.ObjectID)
                {
                    baseName = user.BaseBattlePetName;
                    customName = user.BattlePetCustomName;
                }
            }
            if (user.HasCompanion && PluginLink.Configuration.replaceEmotesOnMinions)
            {
                if (user.Companion->Character.GameObject.ObjectID == emoteTarget.ObjectID)
                {
                    baseName = user.CompanionBaseName;
                    customName = user.CustomCompanionName;
                }
            }
        }

        StringUtils.instance.ReplaceSeString(ref message, baseName, customName);
    }
}
