using Newtonsoft.Json;
using PetRenamer.Utilization.UtilsModule;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.Core.Serialization;

[Serializable]
public class SerializableUserV3
{
    public int[] ids { get; private set; } = Array.Empty<int>();
    public string[] names { get; private set; } = Array.Empty<string>();
    public string username { get; private set; } = string.Empty;
    public ushort homeworld { get; private set; } = 0;
    public int[] mainSkeletons { get; set; } = PluginConstants.baseSkeletons;
    public int[] softSkeletons { get; set; } = PluginConstants.baseSkeletons;

    [JsonIgnore] public string[] ipcNames { get; private set; } = Array.Empty<string>();
    [JsonIgnore] public bool changed = false;
    [JsonIgnore] public bool hasAny => hasCompanion || hasBattlePet;
    [JsonIgnore] public bool hasCompanion { get; private set; } = false;
    [JsonIgnore] public bool hasBattlePet { get; private set; } = false;
    [JsonIgnore] public int length => ids.Length;
    [JsonIgnore] public int lastTouchedID = -1;
    [JsonIgnore] public QuickName this[int i] => new QuickName(ids[i], names[i], ipcNames[i]);
    public bool Contains(int id) => ids.Contains(id);

    public SerializableUserV3(string username, ushort homeworld)
    {
        this.username = username.Replace(((char)0).ToString(), ""); //Dont start about it... literally. If I dont replace (char)0 with an empty string it WILL bitch...
        this.homeworld = homeworld;
    }

    public SerializableUserV3(string username, ushort homeworld, int[] mainSkeletons, int[] softSkeletons) : this(username, homeworld)
    {
        if (mainSkeletons?.Length == 5) this.mainSkeletons = mainSkeletons;
        if (softSkeletons?.Length == 5) this.softSkeletons = softSkeletons;
    }

    public SerializableUserV3(int[] ids, string[] names, string username, ushort homeworld) : this(username, homeworld)
    {
        if (ids == null || names == null) return;
        if (ids.Length != names.Length) return;
        for (int i = 0; i < ids.Length; i++)
            SaveNickname(ids[i], names[i], i == ids.Length - 1);
    }

    [JsonConstructor]
    public SerializableUserV3(int[] ids, string[] names, string username, ushort homeworld, int[] mainSkeletons, int[] softSkeletons) : this(username, homeworld, mainSkeletons, softSkeletons)
    {
        if (ids == null || names == null) return;
        if (ids.Length != names.Length) return;
        for (int i = 0; i < ids.Length; i++)
            SaveNickname(ids[i], names[i], i == ids.Length - 1);
    }

    public void LoopThrough(Action<(int, string)> callback)
    {
        if (callback == null) return;
        for (int i = 0; i < ids.Length; i++)
            callback.Invoke((ids[i], names[i]));
    }

    public string GetNameFor(string name) => GetNameFor(SheetUtils.instance.GetIDFromName(name));
    public string GetNameFor(int id, bool allowIPC = true)
    {
        int index = IndexOf(id);
        if (index == -1) return string.Empty;
        if (names.Length < index) return string.Empty;
        string currentName = allowIPC ? ipcNames[index] : string.Empty;
        if (currentName == string.Empty) currentName = names[index];
        if (currentName.Length > PluginConstants.ffxivNameSize)
            return currentName[..PluginConstants.ffxivNameSize];
        return currentName ?? string.Empty;
    }

    public bool ToggleBackChanged()
    {
        bool curChanged = changed;
        changed = false;
        return curChanged;
    }

    public void SaveNickname(int id, string name, bool doCheck = true, bool force = false, bool isIPCName = false)
    {
        if (id == -1) return;
        if (name == string.Empty && id > -1 && !isIPCName) RemoveNickname(id);
        if (ids.Contains(id)) OverwriteNickname(id, name, isIPCName);
        else GenerateNewNickname(id, name, force, isIPCName);

        if (!doCheck) return;
        FillBattlePets();
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

    void GenerateNewNickname(int id, string name, bool force = false, bool isIpcName = false)
    {
        if (!force)
            if (id == -1 || name == string.Empty)
                return;
        List<int> idList = ids.ToList();
        List<string> namesList = names.ToList();
        List<string> ipcList = ipcNames.ToList();
        idList.Add(id);
        namesList.Add(isIpcName ? string.Empty : name);
        ipcList.Add(isIpcName ? name : string.Empty);
        ids = idList.ToArray();
        names = namesList.ToArray();
        ipcNames = ipcList.ToArray();

        lastTouchedID = id;
        changed = true;
    }

    public void OverwriteNickname(int id, string name, bool isIPCName = false)
    {
        int index = IndexOf(id);
        if (index == -1) return;
        if (!isIPCName)
        {
            if (names[index] == name) return;
            names[index] = name;
        }
        else
        {
            if (ipcNames[index] == name) return;
            ipcNames[index] = name;
        }

        lastTouchedID = id;
        changed = true;
    }

    public void RemoveNickname(int id)
    {
        FillBattlePets();
        int index = IndexOf(id);
        if (index == -1) return;

        lastTouchedID = id;
        changed = true;
        List<int> idList = ids.ToList();
        List<string> namesList = names.ToList();
        List<string> ipcList = ipcNames.ToList();
        idList.RemoveAt(index);
        namesList.RemoveAt(index);
        ipcList.RemoveAt(index);
        ids = idList.ToArray();
        names = namesList.ToArray();
        ipcNames = ipcList.ToArray();
    }

    void FillBattlePets()
    {
        foreach (int id in RemapUtils.instance.bakedBattlePetSkeletonToName.Keys)
        {
            bool found = false;
            for (int i = 0; i < length; i++)
            {
                if (ids[i] != id) continue;
                found = true;
                break;
            }
            if (!found) SaveNickname(id, "", true, true);
        }
    }

    public void ClearAllIPC()
    {
        string[] newIPCNames = new string[ipcNames.Length];
        for (int i = 0; i < ipcNames.Length; i++)
            newIPCNames[i] = string.Empty;
        ipcNames = newIPCNames;
        changed = true;
    }

    public int IndexOf(int id)
    {
        for (int i = 0; i < ids.Length; i++)
            if (ids[i] == id)
                return i;
        return -1;
    }

    public bool HasID(int id) => ids.Contains(id);
    public bool Equals(string username, ushort homeworld) => this.username.ToLowerInvariant().Trim() == username.ToLowerInvariant().Trim() && this.homeworld == homeworld;
    public bool Equals((string, ushort) user) => Equals(user.Item1, user.Item2);

    public int AccurateTotalPetCount()
    {
        int counter = 0;
        foreach (string str in names)
        {
            if (str != string.Empty && str != null)
                counter++;
        }
        return counter;
    }

    public int AccurateMinionCount()
    {
        int counter = 0;
        for (int i = 0; i < names.Length!; i++)
        {
            if (ids[i] <= -1) continue;
            if (names[i] != string.Empty && names[i] != null)
                counter++;
        }
        return counter;
    }

    public int AccurateBattlePetCount()
    {
        int counter = 0;
        for (int i = 0; i < names.Length!; i++)
        {
            if (ids[i] >= -1) continue;
            if (names[i] != string.Empty && names[i] != null)
                counter++;
        }
        return counter;
    }

    public int AccurateIPCCount()
    {
        int counter = 0;
        for (int i = 0; i < ipcNames.Length!; i++)
        {
            if (ipcNames[i] != string.Empty && ipcNames[i] != null)
                counter++;
        }
        return counter;
    }

    public void Reset()
    {
        ids = Array.Empty<int>();
        names = Array.Empty<string>();
    }

    public void Swap(int startIndex, int endIndex)
    {
        ids = LinqUtils.instance.Swap(ids.ToList(), startIndex, endIndex).ToArray();
        names = LinqUtils.instance.Swap(names.ToList(), startIndex, endIndex).ToArray();
        ipcNames = LinqUtils.instance.Swap(ipcNames.ToList(), startIndex, endIndex).ToArray();
    }
}

public struct QuickName
{
    public int ID;
    public string RawName;
    public string Name => IpcName == string.Empty ? RawName : IpcName;
    public string IpcName;

    public bool HasIPCName => IpcName != string.Empty && IpcName != null;

    public QuickName(int id, string name, string ipcName)
    {
        ID = id;
        RawName = name;
        IpcName = ipcName;
    }
}