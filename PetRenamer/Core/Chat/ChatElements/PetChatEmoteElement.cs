﻿using Dalamud.Game.Text;
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
internal unsafe class PetChatEmoteElement : RestrictedChatElement
{
    // 40 53 56 41 54 41 57 48 83 EC ?? 48 8B 02
    // Aparently an emote hook!
    // https://github.com/RokasKil/EmoteLog/blob/master/EmoteLog/Hooks/EmoteReaderHook.cs
    // DUDE THIS IS SICKO!
    // Thanks to Speedas in the Dalamud discord

    public PetChatEmoteElement() => RegisterChat(XivChatType.StandardEmote, XivChatType.CustomEmote);

    internal override void OnRestrictedChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!PluginLink.Configuration.displayCustomNames) return;

        BattleChara* bChara = PluginLink.CharacterManager->LookupBattleCharaByName(sender.ToString(), true);
        if (bChara == null) return;

        nint value = nint.Zero;

        var emoteTarget = bChara->Character.EmoteController.Target;
        if (emoteTarget.Type != 0 && emoteTarget.Type != 4) return;

        if (emoteTarget.Type == 4)
            foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
                if (user.ObjectID == emoteTarget.ObjectId)
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
                if (pet.ObjectID != emoteTarget.ObjectId && pet.Pet != value) continue;

                (string, string)[] replaceNames = new (string, string)[] { (pet.BaseNamePlural, pet.UsedName), (pet.BaseName, pet.UsedName) };
                StringUtils.instance.ReplaceSeString(ref message, ref replaceNames, !(PluginHandlers.ClientState.ClientLanguage == Dalamud.Game.ClientLanguage.Japanese));
                return;
            }
        }
        return;
    }
}
