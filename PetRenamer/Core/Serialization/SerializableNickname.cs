using System;
using Newtonsoft.Json;
using PetRenamer.Utilization.UtilsModule;

namespace PetRenamer.Core.Serialization
{
    [Serializable]
    public class SerializableNickname
    {
        public int ID { get; set; }
        public string Name { get; set; } = null!;
        [JsonIgnore]
        public string BaseName { get; set; } = null!;
        [JsonIgnore]
        public string BaseNamePlural { get; set; } = null!;
        [JsonIgnore]
        bool done = false;

        [JsonConstructor]
        public SerializableNickname(int ID, string Nickname)
        {
            this.ID = ID;
            this.Name = Nickname;
        }

        public void Setup()
        {
            if (done) return;
            done = true;
            if (ID >= 0) BaseName = SheetUtils.instance.GetPetName(ID);
            if (ID >= 0) BaseNamePlural = SheetUtils.instance.GetPetName(ID,NameType.Plural);
        }

        public override string ToString() => $"ID:{ID},Name:{Name},BaseName:{BaseName},BaseNamePlural{BaseNamePlural}";

        public string ToSaveString() => $"{ID}{PluginConstants.forbiddenCharacter}{Name}";

        public bool Valid() => Name != string.Empty && Name != null && ID != -1;

        public bool BaseNameEquals(string other) => (BaseName?.Equals(other, StringComparison.OrdinalIgnoreCase) ?? false) || (BaseNamePlural?.Equals(other, StringComparison.OrdinalIgnoreCase) ?? false);
    }
}
