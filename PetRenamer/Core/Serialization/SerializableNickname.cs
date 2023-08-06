using System;
using Newtonsoft.Json;

namespace PetRenamer.Core.Serialization
{
    [Serializable]
    public class SerializableNickname
    {
        public int ID { get; set; }
        public string Name { get; set; } = null!;

        [JsonConstructor]
        public SerializableNickname(int ID, string Nickname) 
        { 
            this.ID = ID;
            this.Name = Nickname;
        }

        public override string ToString() => $"ID:{ID},Name:{Name}";

        public string ToSaveString() => $"{ID},{Name}";
    }
}
