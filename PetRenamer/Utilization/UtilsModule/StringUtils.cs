using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Singleton;
using PetRenamer.Utilization.Attributes;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class StringUtils : UtilsRegistryType, ISingletonBase<StringUtils>
{
    public static StringUtils instance { get; set; } = null!;

    public string GetLocalName(int ID)
    {
        SerializableUserV2 user;
        if ((user = ConfigurationUtils.instance.GetLocalUserV2()!) == null) return string.Empty;

        foreach (SerializableNickname nickname in user.nicknames)
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

    public unsafe byte[] GetBytes(byte* bytePtr)
    {
        byte[] nameBytes = new byte[PluginConstants.ffxivNameSize];
        IntPtr intPtr = (nint)bytePtr;
        Marshal.Copy(intPtr, nameBytes, 0, PluginConstants.ffxivNameSize);
        return nameBytes;
    }

    public unsafe string GetCharacterName(byte*  bytePtr) => FromBytes(GetBytes(bytePtr)).Replace(((char)0).ToString(), "");

    public string FromBytes(byte[] bytes) => Encoding.Default.GetString(bytes);
    public string MakeTitleCase(string str) => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str.ToLower());

    #region OBSOLETE
    [Obsolete]
    public string GetName(int ID)
    {
        foreach (SerializableNickname nickname in PluginLink.Configuration.users!)
            if (nickname.ID == ID)
                return nickname.Name;

        return string.Empty;
    }
    #endregion
}
