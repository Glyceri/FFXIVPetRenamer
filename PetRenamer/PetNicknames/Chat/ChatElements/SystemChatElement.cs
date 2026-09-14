using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using PetRenamer.PetNicknames.Chat.Base;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;
using XBMPet = PetRenamer.PetNicknames.Services.ServiceWrappers.Sheets.XBMPetButActuallyWorkingSinceForSomeReasonWePutInUnsusedSheetsAndNowEverythingIsABreakingChangeBecauseWhyWouldntItBeLikeWhatAreWeGenuinelyDoingHere;

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
    }

    protected override void SetupChatMessages()
    {
        Register(new LogChatMessage(640,   XivChatType.SystemMessage, XivChatRelationKind.LocalPlayer, OnValidChatMessage)); // <head(<denoun(BNpcName,5,lnum1,1,1,1)>)> wurde fortgeschickt.
        Register(new LogChatMessage(642,   XivChatType.SystemError,   XivChatRelationKind.LocalPlayer, OnValidChatMessage)); // Du hast <denoun(BNpcName,5,lnum1,1,3,1)> den Befehl „<sheet(PetAction,lnum2,0)>“ gegeben.
        Register(new LogChatMessage(3840,  XivChatType.SystemMessage, XivChatRelationKind.LocalPlayer, OnValidChatMessage)); // <string(lstr1)> wird nächstes Mal als <string(lstr2)> erscheinen.
        Register(new LogChatMessage(3841,  XivChatType.SystemMessage, XivChatRelationKind.LocalPlayer, OnValidChatMessage)); // <string(lstr1)> wird nächstes Mal unverwandelt erscheinen.
        Register(new LogChatMessage(4500,  XivChatType.SystemMessage, XivChatRelationKind.None,        OnValidNotebookChatMessage)); // <colortype(500)><edgecolortype(501)><head(<sheet(Companion,lnum1,0)>)><edgecolortype(0)><colortype(0)> was added to your favorites.
        Register(new LogChatMessage(4504,  XivChatType.SystemMessage, XivChatRelationKind.None,        OnValidNotebookChatMessage)); // <colortype(500)><edgecolortype(501)><head(<sheet(Companion,lnum1,0)>)><edgecolortype(0)><colortype(0)> was removed from your favorites.
        Register(new LogChatMessage(11481, XivChatType.SystemMessage, XivChatRelationKind.None,        OnValidHorn)); // Your <sheet(Pet,<sheet(XBMPet,lnum1,0)>,0)> is assigned to the <switch(lnum2,first battlehorn,second battlehorn,third battlehorn)>.
    }

    private void OnValidChatMessage(IHandleableChatMessage chatMessage)
    {
        if (PronounHook.LastGottenPronoun == null)
        {
            return;
        }
        
        if (!chatMessage.Message.TextValue.Contains(PronounHook.LastGottenPronoun.TextValue, StringComparison.InvariantCultureIgnoreCase))
        {
            return;
        }
        
        IPettablePet? pettablePet = PetServices.UserList.LocalPlayer?.GetYoungestPet([SkeletonType.BattlePet, SkeletonType.BeastMaster]);
        
        if (pettablePet == null)
        {
            return;
        }

        PetServices.StringHelper.ReplaceChat(PetServices.Configuration.ShowNamesInActionLogColour, chatMessage, pettablePet, NameType.Raw);
    }
    
    private void OnValidNotebookChatMessage(IHandleableChatMessage chatMessage)
    {
        if (PetServices.UserList.LocalPlayer == null)
        {
            return;
        }
        
        if (PetServices.HoverService.CurrentlyHoveredPet == null)
        {
            return;
        }
        
        PetServices.StringHelper.ReplaceChat(PetServices.Configuration.ShowNamesInActionLogColour, chatMessage, PetServices.HoverService.CurrentlyHoveredPet, PetServices.HoverService.CurrentNameType, PetServices.UserList.LocalPlayer);
    }
    
    private void OnValidHorn(IHandleableChatMessage chatMessage)
    {
        for (byte i = 0; i < IHornService.AmountOfSlots; i++)
        {
            if (!PetServices.HornService.TryGetPetForSlot(i, out XBMPet? pet))
            {
                continue;
            }
            
            bool hasChanged = PetServices.HornService.HasChangedForSlot(i);
            
            if (!hasChanged)
            {
                continue;
            }
            
            IPetSheetData? petData = PetServices.PetSheets.GetPetFromIcon(pet.Value.Icon);
            
            if (petData == null)
            {
                continue;
            }
            
            PetServices.StringHelper.ReplaceChat(PetServices.Configuration.ShowNamesInActionLogColour, chatMessage, petData, NameType.Raw, PetServices.UserList.LocalPlayer);
            
            break;
        }
    }
}