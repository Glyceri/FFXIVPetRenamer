using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using System;
using System.Globalization;
using System.Text;

namespace PetRenamer.Utilization.UtilsModule
{
    internal class StringUtils : UtilsRegistryType
    {
        public string FromBytes(byte[] bytes) => Encoding.Default.GetString(bytes);

        public string GetName(int ID)
        {
            foreach (SerializableNickname nickname in PluginLink.Configuration.nicknames!)
                if (nickname.ID == ID)
                    return nickname.Name;

            return string.Empty;
        }

        public byte[] GetBytes(string name)
        {
            byte[] bytes = new byte[PluginConstants.ffxivNameSize];
            if (name == string.Empty) return bytes;

            int smallestLength = Math.Min(name.Length, bytes.Length);
            for (int i = 0; i < smallestLength; i++)
                bytes[i] = (byte)name[i];

            return bytes;
        }

        public byte[] GetBytes(int ID) => GetBytes(GetName(ID));

        public string MakeTitleCase(string str) => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str.ToLower());
    }
}
