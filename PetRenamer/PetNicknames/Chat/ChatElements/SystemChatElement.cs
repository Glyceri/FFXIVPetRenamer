using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using Dalamud.Utility;
using PetRenamer.PetNicknames.Chat.Base;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;

namespace PetRenamer.PetNicknames.Chat.ChatElements;

internal class SystemChatElement : LogChatElement
{
    private readonly IPetServices PetServices;
    private readonly IPronounHook PronounHook;

    public SystemChatElement(DalamudServices dalamudServices, IPetServices petServices, IPronounHook pronounHook)
        : base(dalamudServices)
    {
        PetServices     = petServices;
        PronounHook     = pronounHook;
        
        Register(new LogChatMessage(640,  XivChatType.SystemMessage, XivChatRelationKind.LocalPlayer)); // <head(<denoun(BNpcName,5,lnum1,1,1,1)>)> wurde fortgeschickt.
        Register(new LogChatMessage(642,  XivChatType.SystemError,   XivChatRelationKind.LocalPlayer)); // Du hast <denoun(BNpcName,5,lnum1,1,3,1)> den Befehl „<sheet(PetAction,lnum2,0)>“ gegeben.
        Register(new LogChatMessage(3840, XivChatType.SystemMessage, XivChatRelationKind.LocalPlayer)); // <string(lstr1)> wird nächstes Mal als <string(lstr2)> erscheinen.
        Register(new LogChatMessage(3841, XivChatType.SystemMessage, XivChatRelationKind.LocalPlayer)); // <string(lstr1)> wird nächstes Mal unverwandelt erscheinen.
    }
    
    protected override void OnValidChatMessage(IHandleableChatMessage chatMessage)
    {
        if (PronounHook.LastGottenPronoun.IsNullOrWhitespace())
        {
            return;
        }
        
        if (!chatMessage.Message.TextValue.Contains(PronounHook.LastGottenPronoun))
        {
            return;
        }
        
        IPettablePet? pettablePet = PetServices.UserList.LocalPlayer?.GetYoungestPet(SkeletonType.BattlePet);
        
        if (pettablePet == null)
        {
            return;
        }

        PetServices.StringHelper.ReplaceChat(PetServices.Configuration.ShowNamesInActionLogColour, chatMessage, pettablePet, NameType.Raw);
    }
}