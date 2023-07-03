using System;
using Newtonsoft.Json;

namespace PetRenamer.Core.Serialization
{
    [Serializable]
    public class SerializableUser
    {
        public SerializableNickname[] nicknames { get; set; } = null!;
        public string username { get; set; } = null!;
        public byte gender { get; set; }
        public ushort homeworld { get; set; }

        [JsonConstructor]
        public SerializableUser(SerializableNickname[] nicknames, string username, byte gender, ushort homeworld)
        {
            this.nicknames = nicknames;
            this.username = username;
            this.gender = gender;
            this.homeworld = homeworld;
        }

        public override string ToString() => $"username:{username},nicknames:{nicknames},gender:{gender},homeworld:{homeworld}";
    }
}
