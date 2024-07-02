using Newtonsoft.Json;
using System;

namespace PetRenamer.Core.Serialization;

[Obsolete("This is here for legacy support")]
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

    public string ToSaveString() => $"{ID}{PluginConstants.forbiddenCharacter}{Name}";

    public bool Valid() => Name != string.Empty && Name != null && ID != -1;
}
