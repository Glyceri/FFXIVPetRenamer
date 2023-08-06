using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.Core.Serialization;

[Serializable]
public class SerializableUserV2
{
    public int[] ids { get; set; } = null!;
    public string[] names { get; set; } = null!;
    public string username { get; set; } = null!;
    public ushort homeworld { get; set; }

    [JsonIgnore]
    public SerializableNickname[] nicknames 
    {
        get => ToArray();
        set
        {
            int length = value.Length;
            ids = new int[length];
            names = new string[length];
            for (int i = 0; i < length; i++)
            {
                ids[i] = value[i].ID;
                names[i] = value[i].Name;
            }
        }
    }

    [JsonIgnore]
    public SerializableNickname this[ int i]
    {
        get => new SerializableNickname(ids[i], names[i]);
        set
        {
            ids[i] = value.ID;
            names[i] = value.Name;
        } 
    }

    [JsonIgnore]
    public int length => ids.Length;

    public SerializableUserV2(string username, ushort homeworld) : this(new int[0], new string[0], username, homeworld) { }
    public SerializableUserV2(SerializableNickname[] nicknames, string username, ushort homeworld) : this(username, homeworld)
    {
        ids= new int[nicknames.Length];
        names= new string[nicknames.Length];
        for(int i = 0; i < nicknames.Length; i++)
        {
            ids[i] = nicknames[i].ID;
            names[i] = nicknames[i].Name;
        }
    }

    [JsonConstructor]
    public SerializableUserV2(int[] ids, string[] names, string username, ushort homeworld)
    {
        this.ids = ids;
        this.names = names;
        this.username = username.Replace(((char)0).ToString(), ""); //Dont start about it... literally. If I dont replace (char)0 with an empty string it WILL bitch...
        this.homeworld = homeworld;
    }

    public SerializableNickname[] ToArray()
    {
        SerializableNickname[] array = new SerializableNickname[length];
        for (int i = 0; i < length; i++)
            array[i] = this[i];
        return array;
    }
    public List<SerializableNickname> ToList() => ToArray().ToList();




    public override string ToString() => $"username:{username},ids:{ids},names:{names},homeworld:{homeworld}";
}
