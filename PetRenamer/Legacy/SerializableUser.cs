using Newtonsoft.Json;
using System;

#pragma warning disable IDE0130 // Namespace does not match folder structure (This is to keep the save file shorter)
namespace PetRenamer.Core.Serialization;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[Obsolete("Use SerializableUserV5 instead")]
[Serializable]
public class SerializableUser
{
#pragma warning disable IDE1006 // Naming Styles (Legacy save format, I cannot change this now)
    public SerializableNickname[] nicknames { get; set; } = null!;
    public string username { get; set; } = null!;
    public ushort homeworld { get; set; }
#pragma warning restore IDE1006 // Naming Styles

    [JsonConstructor]
    public SerializableUser(SerializableNickname[] nicknames, string username, ushort homeworld)
    {
        this.nicknames = nicknames;
        this.username = username;
        this.homeworld = homeworld;
    }
}
