using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace PetRenamer.PetNicknames.Chat.Interfaces;

internal interface IChatElement
{
    void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled);
}
