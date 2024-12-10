using Newtonsoft.Json;
using System;

#pragma warning disable IDE0130 // Namespace does not match folder structure (Named like this for compatibility reasons)
namespace PetRenamer.Core.Serialization;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[Obsolete("Use SerializableUserV5 instead")]
[Serializable]
public class SerializableUserV2
{

#pragma warning disable IDE1006 // Naming Styles (Legacy save format, I cannot change this now)
    public int[] ids { get; set; } = null!;
    public string[] names { get; set; } = null!;
    public string username { get; set; } = null!;
    public ushort homeworld { get; set; }
#pragma warning restore IDE1006 // Naming Styles

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
