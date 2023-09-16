using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.Serialization;
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
        for(int i = 0; i < me.ids.Length; i++)
        {
            int id = me.ids[i];
            string name = me.names[i];
            listIds.Add(id);
            listStrings.Add(names[i]);
            if (ids.Contains(id)) importTypes.Add(names.Contains(name) ? ImportType.None : ImportType.Rename);
            else importTypes.Add(ImportType.Remove);
        }

        for (int i = 0; i < ids.Length; i++)
        {
            int id = ids[i];
            string name = names[i];
            if (!me.ids.Contains(id))
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
