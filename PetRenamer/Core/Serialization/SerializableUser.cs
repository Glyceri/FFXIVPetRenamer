using System;
using Newtonsoft.Json;

namespace PetRenamer.Core.Serialization;

[Obsolete]
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
        this.username = username.Replace(((char)0).ToString(), ""); //Dont start about it... literally. If I dont replace (char)0 with an empty string it WILL bitch...
        this.homeworld = homeworld;
    }

    public SerializableUser(string username, ushort homeworld)
    {
        this.nicknames = new SerializableNickname[0];
        this.username = username;
        this.homeworld = homeworld;
    }

    public override string ToString() => $"username:{username},nicknames:{nicknames},homeworld:{homeworld}";
}
