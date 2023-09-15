using Dalamud.Logging;
using ImGuiScene;
using PetRenamer.Core.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Networking.Structs;
using PetRenamer.Core.PettableUserSystem.Enums;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PetRenamer.Core.PettableUserSystem;

internal class PettableUserHandler : IDisposable, IInitializable
{
    List<PettableUser> _users = new List<PettableUser>();

    public List<PettableUser> Users { get => _users; set => _users = value; }

    LastActionUsed _lastCast;
    public LastActionUsed LastCast { get => _lastCast; private set => _lastCast = value; }

    public void BackwardsSAFELoopThroughUser(Action<PettableUser> action)
    {
        if (action == null) return;
        for (int i = _users.Count - 1; i >= 0; i--)
            action.Invoke(_users[i]);
    }

    public void LoopThroughUsers(Action<PettableUser> action)
    {
        if (action == null) return;
        foreach (PettableUser user in _users)
            action.Invoke(user);
    }

    public void LoopThroughBreakable(Func<PettableUser, bool> func)
    {
        if (func == null) return;
        foreach (PettableUser user in _users)
            if (func.Invoke(user))
                break;
    }

    public void Dispose()
    {
        _users?.Clear();

        foreach (TextureWrap textureWrap in fileInfos.Values)
            textureWrap?.Dispose();
        fileInfos?.Clear();
    }

    public void Initialize()
    {
        int length = PluginLink.Configuration.serializableUsersV3!.Length;
        for (int i = 0; i < length; i++)
        {
            SerializableUserV3 user = PluginLink.Configuration.serializableUsersV3![i];
            DeclareUser(user, UserDeclareType.Add);
        }
    }

    public void DeclareUser(SerializableUserV3 user, UserDeclareType userDeclareType, bool force = false)
    {
        if (userDeclareType == UserDeclareType.Add)
        {
            if (force)
                for (int i = _users.Count - 1; i >= 0; i--)
                    if (_users[i].UserName == user.username && _users[i].Homeworld == user.homeworld)
                        _users.RemoveAt(i);
            if (!Contains(user))
            {
                PettableUser u = new PettableUser(user.username, user.homeworld, user);
                _users.Add(u);
                try
                {
                    OnDeclare(u, UserDeclareType.Add, false);
                }
                catch { }
            }
        }
        else if (userDeclareType == UserDeclareType.Remove)
        {
            for (int i = _users.Count - 1; i >= 0; i--)
            {
                if (_users[i].UserName != user.username || _users[i].Homeworld != user.homeworld) continue;

                try
                {
                    OnDeclare(_users[i], UserDeclareType.Remove, false);
                }
                catch { }
                _users.RemoveAt(i);
            }
        }
    }

    public nint GetTexture(PettableUser user)
    {
        if (user == null) return nint.Zero;
        if (!fileInfos.ContainsKey((user.UserName, user.Homeworld)))
        {
            string iconPath = PluginHandlers.TextureProvider.GetIconPath(786)!;
            if (iconPath == null) return nint.Zero;
            TextureWrap textureWrap = PluginHandlers.TextureProvider.GetTextureFromGame(iconPath)!;
            if (textureWrap == null) return nint.Zero;
            return textureWrap.ImGuiHandle;
        }
        return fileInfos[(user.UserName, user.Homeworld)].ImGuiHandle;
    }

    Dictionary<(string, uint), TextureWrap> fileInfos = new Dictionary<(string, uint), TextureWrap>();

    public void OnDeclare(PettableUser user, UserDeclareType type, bool force)
    {
        (string, uint) currentUser = (user.UserName, user.Homeworld);
        if (type == UserDeclareType.Remove)
        {
            if (!fileInfos.ContainsKey(currentUser)) return;
            fileInfos[currentUser]?.Dispose();
            fileInfos.Remove(currentUser);
            PluginLog.LogVerbose($"User Removed: {currentUser}");
        }
        else if (type == UserDeclareType.Add)
        {
            if (fileInfos.ContainsKey(currentUser)) return;
            string path = MakePath(currentUser);
            try
            {
                if (Path.Exists(path))
                {
                    PluginLog.LogVerbose("File already exists! Grabbing from cache!");
                    DeclareDownload((user.UserName, user.Homeworld));
                }
                else if (PluginLink.Configuration.downloadProfilePictures || force)
                {
                    PluginLog.LogVerbose("File doesn't exists! Downloading file!");
                    DownloadPagination((user.UserName, user.Homeworld));
                }
            }
            catch { }
        }
    }

    public async void DownloadPagination((string, uint) characterData)
    {
        if (fileInfos.ContainsKey(characterData))
        {
            fileInfos[characterData]?.Dispose();
            fileInfos?.Remove(characterData);
        }
        try
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"https://xivapi.com/character/search?name={characterData.Item1}&server={SheetUtils.instance.GetWorldName((ushort)characterData.Item2)}");
            response.EnsureSuccessStatusCode();
            Stream str = response.Content.ReadAsStream();
            StreamReader reader = new StreamReader(str);
            string lines = reader.ReadToEnd();

            PaginationRoot? paginatedStruct = JsonSerializer.Deserialize<PaginationRoot>(lines);
            if (paginatedStruct != null)
            {
                List<Result> pStruct = paginatedStruct.Results;
                if (pStruct != null && pStruct.Count != 0 && pStruct[0].Avatar != null)
                    await Task.Run(() => AsyncDownload(pStruct[0].Avatar.ToString(), characterData));
            }

            reader?.Dispose();
            str?.Dispose();
            response?.Dispose();
            client?.Dispose();
        }
        catch { }
    }

    async void AsyncDownload(string URL, (string, uint) charaData)
    {
        try
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(URL);
            response.EnsureSuccessStatusCode();
            Stream str = response.Content.ReadAsStream();

            ImageCodecInfo myImageCodecInfo;
            Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;

            Image bMap = Image.FromStream(str);
            myImageCodecInfo = GetEncoderInfo("image/jpeg");
            myEncoder = Encoder.Quality;

            myEncoderParameters = new EncoderParameters(1);
            myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            bMap.Save(MakePath(charaData), myImageCodecInfo, myEncoderParameters);
            DeclareDownload(charaData);
            str?.Dispose();
            response?.Dispose();
            httpClient?.Dispose();
        }
        catch { }
    }

    public void DeclareDownload((string, uint) characterData)
    {
        try
        {
            lock (fileInfos)
            {
                if (fileInfos.ContainsKey(characterData))
                    fileInfos[characterData]?.Dispose();
                fileInfos.Remove(characterData);

                string path = MakePath(characterData);
                FileInfo info = new FileInfo(path);
                if (!info.Exists) return;

                TextureWrap wrap = PluginHandlers.TextureProvider.GetTextureFromFile(info, false)!;
                if (wrap == null) return;
                fileInfos.Add(characterData, wrap);
            }
        }
        catch { }
    }

    ImageCodecInfo GetEncoderInfo(string mimeType)
    {
        int j;
        ImageCodecInfo[] encoders;
        encoders = ImageCodecInfo.GetImageEncoders();
        for (j = 0; j < encoders.Length; ++j)
        {
            if (encoders[j].MimeType == mimeType)
                return encoders[j];
        }
        return null!;
    }

    string MakePath((string, uint) characterData) => Path.Combine(Path.GetTempPath(), $"PetNicknames_{characterData.Item1.Replace(" ", "_")}_{SheetUtils.instance.GetWorldName((ushort)characterData.Item2)}.jpg");


    public bool LocalPetChanged()
    {
        PettableUser user = LocalUser()!;
        if (user == null) return false;
        return user.AnyPetChanged;
    }

    public PettableUser? GetUser(string name)
    {
        PettableUser? returnThis = null;
        LoopThroughBreakable((user) =>
        {
            if (name.Contains(user.UserName))
            {
                returnThis = user;
                return true;
            }
            return false;
        });
        return returnThis;
    }

    public PettableUser? GetUser(string name, ushort homeworld)
    {
        PettableUser? returnThis = null;
        LoopThroughBreakable((user) =>
        {
            if (name.Contains(user.UserName) && homeworld == user.Homeworld)
            {
                returnThis = user;
                return true;
            }
            return false;
        });
        return returnThis;
    }

    public PettableUser? LocalUser()
    {
        PettableUser? returnThis = null;
        LoopThroughBreakable((user) =>
        {
            if (user.LocalUser)
            {
                returnThis = user;
                return true;
            }
            return false;
        });

        return returnThis;
    }

    public PettableUser? LastCastedUser()
    {
        PettableUser user = null!;
        foreach (PettableUser user1 in PluginLink.PettableUserHandler.Users)
        {
            if (user1 == null) continue;
            if (!user1.UserExists) continue;
            if (user1.nintUser != _lastCast.castDealer && user1.nintBattlePet != _lastCast.castDealer) continue;
            user = user1;
            break;
        }

        return user;
    }

    public (string, string)[] GetValidNames(PettableUser user, string beContainedIn)
    {
        List<(string, string)> validNames = new List<(string, string)>();
        if (beContainedIn == null) return validNames.ToArray();
        if (user == null) return validNames.ToArray();
        if (!user.UserExists) return validNames.ToArray();
        foreach (int skelID in RemapUtils.instance.battlePetRemap.Keys)
        {
            string bPetname = SheetUtils.instance.GetBattlePetName(skelID) ?? string.Empty;
            if (bPetname == string.Empty) continue;
            if (!beContainedIn.ToString().Contains(bPetname)) continue;
            if (!RemapUtils.instance.skeletonToClass.ContainsKey(skelID)) continue;
            int jobID = RemapUtils.instance.skeletonToClass[skelID];
            string cName = user.SerializableUser.GetNameFor(jobID) ?? string.Empty;
            if (cName == string.Empty || cName == null) continue;
            validNames.Add((bPetname, cName));
        }
        validNames.Sort((el1, el2) => el1.Item1.Length.CompareTo(el2.Item1.Length));
        return validNames.ToArray();
    }

    bool Contains(SerializableUserV3 user)
    {
        for (int i = 0; i < _users.Count; i++)
            if (_users[i].UserName.ToLowerInvariant().Trim().Normalize() == user.username.ToLowerInvariant().Trim().Normalize() && _users[i].Homeworld == user.homeworld)
                return true;
        return false;
    }

    public void SetLastCast(IntPtr castUser, IntPtr castDealer)
    {
        _lastCast = new LastActionUsed(castUser, castDealer);
    }
}

public struct LastActionUsed
{
    public IntPtr castUser;
    public IntPtr castDealer;

    public LastActionUsed(IntPtr castUser, IntPtr castDealer)
    {
        this.castUser = castUser;
        this.castDealer = castDealer;
    }
}

public enum LastActionType
{
    Cast,
    ActionUsed
}
