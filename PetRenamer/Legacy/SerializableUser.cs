using Newtonsoft.Json;
using System;

namespace PetRenamer.Core.Serialization;

[Obsolete("Use SerializableUserV3 instead")]
[Serializable]
public class SerializableUser
{
    public SerializableNickname[] nicknames { get; set; } = null!;
    public string username { get; set; } = null!;
    public ushort homeworld { get; set; }

    [JsonConstructor]
    public SerializableUser(SerializableNickname[] nicknames, string username, ushort homeworld)
    {
        this.nicknames = nicknames;
        this.username = username;
        this.homeworld = homeworld;
    }
}
