using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace PetRenamer.Core.Debug;

public class PetChatMessage
{
    public string Message { get; set; } = "";
    public string Sender { get; set; } = "";
    public SeString NewMessage { get; set; } = "";
    public SeString NewSender { get; set; } = "";
    public XivChatType ChatType { get; set; } = XivChatType.None;
    public uint SenderId { get; set; } = 0;

    public PetChatMessage(string Message, string Sender, SeString newMessage, SeString newSender, XivChatType type, uint SenderID) 
    { 
        this.NewMessage = newMessage;
        this.NewSender = newSender;
        this.Message = Message;
        this.Sender = Sender;
        this.ChatType = type;
        this.SenderId = SenderID;
    }
}
