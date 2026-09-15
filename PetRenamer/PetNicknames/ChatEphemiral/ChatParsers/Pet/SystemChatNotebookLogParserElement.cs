using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Linq;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Pet;

internal class SystemChatNotebookLogParserElement : IChatLogPetParserElement
{
    private static readonly uint[] ValidMessageIds =
    [
        4500, // <colortype(500)><edgecolortype(501)><head(<sheet(Companion,lnum1,0)>)><edgecolortype(0)><colortype(0)> was added to your favorites.
        4504, // <colortype(500)><edgecolortype(501)><head(<sheet(Companion,lnum1,0)>)><edgecolortype(0)><colortype(0)> was removed from your favorites.
    ];
    
    private readonly IPetServices PetServices;
    
    public SystemChatNotebookLogParserElement(IPetServices petServices)
    {
        PetServices = petServices;
    }
    
    public NameType ReplaceNameType
        { get; private set; } = NameType.Raw;
    
    public IPetSheetData? UsedData 
        { get; private set; } = null;
    
    public void Reset() 
        => UsedData = null;
    
    public bool IsMyParser(XivChatType chatType, uint messageId)
    {
        if (!ValidMessageIds.Contains(messageId))
        {
            return false;
        }
        
        return chatType == XivChatType.SystemMessage;
    }

    public IChatPet? Parse(uint messageId, IChatPlayer? chatPlayer)
    {
        UsedData        = null;
        ReplaceNameType = NameType.Raw;
        
        if (chatPlayer == null)
        {
            return null;
        }
        
        if (PetServices.UserList.LocalPlayer == null)
        {
            return null;
        }
        
        if (PetServices.HoverService.CurrentlyHoveredPet == null)
        {
            return null;
        }
        
        ReplaceNameType = PetServices.HoverService.CurrentNameType;
        UsedData        = PetServices.HoverService.CurrentlyHoveredPet;
        
        return PetServices.ChatDatabaseService.PetDatabase.MakeChatPet(PetServices.HoverService.CurrentlyHoveredPet.Model, chatPlayer);
    }
}