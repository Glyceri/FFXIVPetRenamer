using Newtonsoft.Json;
using System;

namespace PetRenamer.Core.Serialization;

[Serializable]
public class SerializableUserV2
{
    public int[] ids { get; set; } = null!;
    public string[] names { get; set; } = null!;
    public string username { get; set; } = null!;
    public ushort homeworld { get; set; }

    [JsonConstructor]
    public SerializableUserV2(int[] ids, string[] names, string username, ushort homeworld)
    {
        this.ids = ids;
        this.names = names;
        this.username = username;
        this.homeworld = homeworld;
    }

    public override string ToString() => $"username:{username},ids:{ids},names:{names},homeworld:{homeworld}";
}
