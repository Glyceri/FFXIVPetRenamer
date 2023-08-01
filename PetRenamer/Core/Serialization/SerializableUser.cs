using System;
using Newtonsoft.Json;

namespace PetRenamer.Core.Serialization
{
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

        public override string ToString() => $"username:{username},nicknames:{nicknames},homeworld:{homeworld}";
    }
}
