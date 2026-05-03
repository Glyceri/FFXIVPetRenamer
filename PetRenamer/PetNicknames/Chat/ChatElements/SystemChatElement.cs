using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using PetRenamer.PetNicknames.Chat.Interfaces;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using System;

namespace PetRenamer.PetNicknames.Chat.ChatElements;

internal class SystemChatElement : IChatElement
{
    private readonly DalamudServices   DalamudServices;
    private readonly IPetServices      PetServices;
    private readonly IPronounHook      PronounHook;
    private readonly IPettableUserList UserList;
    
    private uint expectedLogCount = 0;
    
    private static readonly uint[] ProperLogMessages = 
    [
        640,    // <head(<denoun(BNpcName,5,lnum1,1,1,1)>)> wurde fortgeschickt.
        642,    // Du hast <denoun(BNpcName,5,lnum1,1,3,1)> den Befehl „<sheet(PetAction,lnum2,0)>“ gegeben.
        3840,   // <string(lstr1)> wird nächstes Mal als <string(lstr2)> erscheinen.
        3841,   // <string(lstr1)> wird nächstes Mal unverwandelt erscheinen.
    ];
    
    public SystemChatElement(DalamudServices dalamudServices, IPetServices petServices, IPronounHook pronounHook, IPettableUserList userList)
    {
        DalamudServices = dalamudServices;
        PetServices     = petServices;
        PronounHook     = pronounHook;
        UserList        = userList;
        
        DalamudServices.ChatGui.LogMessage += OnLogMessage;
    }
    
    public void Dispose()
    {
        DalamudServices.ChatGui.LogMessage -= OnLogMessage;
    }
    
    // Een queue van messages?
    // Dit werkt opzich maar is natuurlijk niet erg robuust. 
    private void OnLogMessage(ILogMessage message)
    {
        if (!ProperLogMessages.Contains(message.LogMessageId))
        {
            return;
        }
        
        expectedLogCount++;
    }

    public void OnChatMessage(IHandleableChatMessage chatMessage)
    {
        if (chatMessage.LogKind != XivChatType.SystemMessage && chatMessage.LogKind != XivChatType.SystemError)
        {
            return;
        }
        
        if (chatMessage.SourceKind != XivChatRelationKind.LocalPlayer)
        {
            return;
        }
        
        if (PronounHook.LastGottenPronoun == null)
        {
            return;
        }
        
        if (!chatMessage.Message.TextValue.Contains(PronounHook.LastGottenPronoun))
        {
            return;
        }
        
        if (expectedLogCount == 0)
        {
            return;
        }
        
        expectedLogCount--;
        
        if (UserList.LocalPlayer == null)
        {
            return;
        }
        
        IPettablePet? pettablePet = UserList.LocalPlayer.GetYoungestPet(IPettableUser.PetFilter.BattlePet);
        
        if (pettablePet == null)
        {
            return;
        }

        PetServices.StringHelper.ReplaceChat(PetServices.Configuration.ShowNamesInActionLogColour, chatMessage, pettablePet, NameType.Raw);
    }
}