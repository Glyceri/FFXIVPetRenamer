using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;

namespace PetRenamer.Utilization.UtilsModule
{
    internal class NicknameUtils : UtilsRegistryType
    {
        internal bool Contains(int ID)
        {
            foreach (SerializableNickname nickname in PluginLink.Configuration.nicknames!)
            {
                if (nickname == null) continue;
                if (nickname.ID == ID) return true;
            }

            return false;
        }

        internal SerializableNickname GetNickname(int ID)
        {
            for (int i = 0; i < PluginLink.Configuration.nicknames!.Length; i++)
            {
                if (PluginLink.Configuration.nicknames[i].ID == ID)
                    return PluginLink.Configuration.nicknames[i];
            }

            return null;
        }
    }
}
