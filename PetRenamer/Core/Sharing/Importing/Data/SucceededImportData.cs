using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.Core.Sharing.Importing.Data;

public class SucceededImportData : ImportData
{
    public readonly string UserName;
    public readonly ushort HomeWorld;

    public readonly bool isNew;
    public readonly int[] ids;
    public readonly string[] names;
    public readonly ImportType[] importTypes;

    public SerializableUserV3 CreateSerializableUser()
    {
        List<int> newIDs = new List<int>();
        List<string> newNames = new List<string>();
        for(int i = 0; i < ids.Length; i++)
        {
            if (importTypes[i] == ImportType.Remove) continue;
            newIDs.Add(ids[i]);
            newNames.Add(names[i]);
        }
        return new SerializableUserV3(newIDs.ToArray(), newNames.ToArray(), UserName, HomeWorld, PluginConstants.baseSkeletons, PluginConstants.baseSkeletons);
    }
    public string GetString(ImportType type)
    {
        return type switch
        {
            ImportType.Rename => "N",
            ImportType.New => "+",
            ImportType.Remove => "X",
            ImportType.None => "=",
            _ => "=",
        };
    }

    public SucceededImportData(string UserName, ushort HomeWorld, int[] ids, string[] names)
    {
        this.UserName = UserName;
        this.HomeWorld = HomeWorld;
        isNew = IsNew(UserName, HomeWorld);
        (int[], string[], ImportType[]) arrays = GetArrays(UserName, HomeWorld, ids, names);
        this.ids = arrays.Item1;
        this.names = arrays.Item2;
        importTypes = arrays.Item3;
    }

    (int[], string[], ImportType[]) GetArrays(string UserName, ushort HomeWorld, int[] ids, string[] names)
    {
        PettableUser meUser = PluginLink.PettableUserHandler.GetUser(UserName, HomeWorld)!;
        if (meUser == null) return (ids, names, Enumerable.Repeat(ImportType.New, ids.Length).ToArray());
        SerializableUserV3 me = meUser.SerializableUser;
        List<int> listIds = new List<int>();
        List<string> listStrings = new List<string>();
        List<ImportType> importTypes = new List<ImportType>();
        
        for(int i = 0; i < me.length; i++)
        {
            QuickName pet = me[i];
            listIds.Add(pet.ID);

            if (ids.Contains(pet.ID))
            {
                int index = ids.ToList().IndexOf(pet.ID);
                string n = names[index];
                listStrings.Add(n);
                if (n == pet.Name) importTypes.Add(ImportType.None);
                else importTypes.Add(ImportType.Rename);
            }
            else 
            {
                listStrings.Add(pet.Name);
                importTypes.Add(ImportType.Remove); 
            }
        }
        
        for (int i = 0; i < ids.Length; i++)
        {
            int id = ids[i];
            string name = names[i];
            if (!me.Contains(id))
            {
                listIds.Add(id);
                listStrings.Add(name);
                importTypes.Add(ImportType.New);
            }
        }

        return (listIds.ToArray(), listStrings.ToArray(), importTypes.ToArray());
    }

    bool IsNew(string UserName, ushort HomeWorld)
    {
        foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
            if (user.SerializableUser.Equals(UserName, HomeWorld))
                return false;
        return true;
    }
}
