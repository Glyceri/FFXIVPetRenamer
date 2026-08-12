using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Pet;

internal class SystemChatLogParserElement : IChatLogPetParserElement
{
    private static readonly uint[] ValidMessageIds =
    [
        640,        // <head(<ennoun(BNpcName,2,lnum1,1,1)>)> withdraws from the battlefield.
        642,        // You give <ennoun(BNpcName,2,lnum1,1,1)> the order “<sheet(PetAction,lnum2,0)>.”
        3840,       // The next <string(lstr1)> summoned will appear glamoured as <string(lstr2)>.
        3841        // The next <string(lstr1)> summoned will appear unglamoured.
    ];
    
    private readonly IPetServices     PetServices;
    private readonly IChatPetDatabase PetDatabase;
    
    public SystemChatLogParserElement(IChatPetDatabase petDatabase, IPetServices petServices)
    {
        PetDatabase = petDatabase;
        PetServices = petServices;
    }
    
    public NameType ReplaceNameType
        => NameType.Raw;
    
    public IPetSheetData? UsedData 
        { get; private set; } = null;
    
    public bool IsMyParser(XivChatType chatType)
    {
        return (chatType == XivChatType.SystemMessage || chatType == XivChatType.SystemError);
    }

    public IChatPet? Parse(uint messageId, IChatPlayer? chatPlayer)
    {
        UsedData = null;
        
        if (!ValidMessageIds.Contains(messageId))
        {
            return null;
        }
        
        if (PetServices.UserList.LocalPlayer == null)
        {
            return null;
        }
        
        IPettablePet? pettablePet = PetServices.UserList.LocalPlayer.GetYoungestPet(SkeletonType.BattlePet);
        
        if (pettablePet == null)
        {
            return null;
        }
        
        UsedData = pettablePet.PetData;
        
        return PetDatabase.MakeChatPet(pettablePet.SkeletonId, PetServices.UserList.LocalPlayer.DataBaseEntry.Name, PetServices.UserList.LocalPlayer.DataBaseEntry.Homeworld);
    }
}