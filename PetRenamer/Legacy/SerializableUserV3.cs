using Newtonsoft.Json;
using System;

#pragma warning disable IDE0130 // Namespace does not match folder structure (Named like this for compatibility reasons)
namespace PetRenamer.Core.Serialization;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[Obsolete("Use SerializableUserV5 instead")]
[Serializable]
public class SerializableUserV3
{
#pragma warning disable IDE1006
    public int[] ids { get; private set; } = Array.Empty<int>();
    public string[] names { get; private set; } = Array.Empty<string>();
    public string username { get; private set; } = string.Empty;
    public ushort homeworld { get; private set; } = 0;
    public int[] mainSkeletons { get; set; } = [-411, -417, -416, -415, -407]; // Main skeletons store the active (Summoned) pets pet mirage ID.
    public int[] softSkeletons { get; set; } = [-411, -417, -416, -415, -407]; // Soft skeletons stores the new pet mirage ID. Why are these two different? When you pet mirage Eos to a Carbuncle, your current Eos will still remain as Eos until the next summon. BUT! We do need to know the new name for the summon bar and stuff. 
#pragma warning restore IDE1006

    public SerializableUserV3(string username, ushort homeworld)
    {
        this.username  = username;
        this.homeworld = homeworld;
    }

    public SerializableUserV3(string username, ushort homeworld, int[] mainSkeletons, int[] softSkeletons) 
        : this(username, homeworld)
    {
        if (mainSkeletons?.Length == 5) this.mainSkeletons = mainSkeletons;
        if (softSkeletons?.Length == 5) this.softSkeletons = softSkeletons;
    }

    [JsonConstructor]
    public SerializableUserV3(int[] ids, string[] names, string username, ushort homeworld, int[] mainSkeletons, int[] softSkeletons) : this(username, homeworld, mainSkeletons, softSkeletons)
    {
        if (ids == null || names == null) return;
        if (ids.Length != names.Length) return;
        this.ids = ids;
        this.names = names;
    }
}