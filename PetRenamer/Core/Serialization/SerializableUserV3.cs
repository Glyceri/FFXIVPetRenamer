using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.Core.Serialization;

[Serializable]
public class SerializableUserV3
{
    public int[] ids { get; private set; } = new int[0];
    public string[] names { get; private set; } = new string[0];
    public string username { get; private set; } = string.Empty;
    public ushort homeworld { get; private set; } = 0;

    [JsonIgnore] public bool changed = false;
    [JsonIgnore] public bool hasAny => hasCompanion || hasBattlePet;
    [JsonIgnore] public bool hasCompanion { get; private set; } = false;
    [JsonIgnore] public bool hasBattlePet { get; private set; } = false;
    [JsonIgnore] public int length => ids.Length;

    public SerializableUserV3(string username, ushort homeworld)
    {
        this.username = username.Replace(((char)0).ToString(), ""); //Dont start about it... literally. If I dont replace (char)0 with an empty string it WILL bitch...
        this.homeworld = homeworld;
    }

    [JsonConstructor]
    public SerializableUserV3(int[] ids, string[] names, string username, ushort homeworld) : this(username, homeworld)
    {
        if (ids.Length != names.Length) return;
        for (int i = 0; i < ids.Length; i++)
            SaveNickname(ids[i], names[i], i == ids.Length - 1);
    }

    public void LoopThroughBreakable(Func<(int, string), bool> callback)
    {
        if (callback == null) return;
        for (int i = 0; i < ids.Length; i++)
            if (callback.Invoke((ids[i], names[i])))
                break;
    }

    public void LoopThrough(Action<(int, string)> callback)
    {
        if (callback == null) return;
        for (int i = 0; i < ids.Length; i++)
            callback.Invoke((ids[i], names[i]));
    }

    public string? GetNameFor(int id)
    {
        int index = IndexOf(id);
        if (index == -1) return null;
        if (names.Length < index) return null;
        if (names[index].Length > PluginConstants.ffxivNameSize)
            return names[index].Substring(0, PluginConstants.ffxivNameSize);
        return names[index];
    }

    public bool ToggleBackChanged()
    {
        if (changed)
        {
            changed = false;
            return true;
        }
        return false;
    }

    public void SaveNickname(int id, string name, bool doCheck = true, bool notifyICP = false)
    {
        if (id == -1) return;
        //if (name == string.Empty && id >= 0) RemoveNickname(id, notifyICP);
        if (ids.Contains(id)) OverwriteNickname(id, name, notifyICP);
        else GenerateNewNickname(id, name, notifyICP);

        if (!doCheck) return;
        hasCompanion = false;
        hasBattlePet = false;
        for (int i = 0; i < ids.Length; i++)
        {
            int curID = ids[i];
            if (curID >= 0) hasCompanion = true;
            if (curID <= -1) hasBattlePet = true;
            if (hasCompanion && hasBattlePet) break;
        }
    }

    void GenerateNewNickname(int id, string name, bool notifyICP = false)
    {
        List<int> idList = ids.ToList();
        List<string> namesList = names.ToList();
        idList.Add(id);
        namesList.Add(name);
        ids = idList.ToArray();
        names = namesList.ToArray();
        changed = true;
        if(notifyICP) IpcProvider.ChangedPetNickname(new NicknameData(id, name, id, name));
    }

    public void OverwriteNickname(int id, string name, bool notifyICP = false)
    {
        int index = IndexOf(id);
        if (index == -1) return;
        if (names[index] == name) return;
        names[index] = name;
        changed = true;
        if (notifyICP) IpcProvider.ChangedPetNickname(new NicknameData(id, name, id, name));
    }

    public void RemoveNickname(int id, bool notifyICP = false)
    {
        int index = IndexOf(id);
        if (index == -1) return;
        changed = true;
        List<int> idList = ids.ToList();
        List<string> namesList = names.ToList();
        idList.RemoveAt(index);
        namesList.RemoveAt(index);
        ids = idList.ToArray();
        names = namesList.ToArray();
        if (notifyICP) IpcProvider.ChangedPetNickname(new NicknameData(id, string.Empty, id, string.Empty));
    }

    int IndexOf(int id)
    {
        for (int i = 0; i < ids.Length; i++)
            if (ids[i] == id)
                return i;
        return -1;
    }

    public bool HasID(int id) => ids.Contains(id);
    public bool Equals(string username, ushort homeworld) => this.username.ToLowerInvariant().Trim().Normalize() == username.ToLowerInvariant().Trim().Normalize() && this.homeworld == homeworld;
    public bool Equals((string, ushort) user) => Equals(user.Item1, user.Item2);

    public void Reset()
    {
        ids = new int[0];
        names = new string[0];
    }
}
