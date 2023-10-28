﻿using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.PettableUserSystem.Pet;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Singleton;
using PetRenamer.Utilization.Attributes;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using System.Runtime.InteropServices;
using System;
using PetRenamer.Core.PettableUserSystem.Enums;
using PetRenamer.Core;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class IpcUtils : UtilsRegistryType, ISingletonBase<IpcUtils>
{
    public static IpcUtils instance { get; set; } = null!;

    internal override void OnRegistered() => PluginLink.IpcStorage.Register(OnIpcChange);
    internal override void Dispose() => PluginLink.IpcStorage.Deregister(OnIpcChange);

    public void OnIpcChange(ref List<(nint, string)> data)
    {
        foreach ((nint, string) item in data)
            SetNickname(item.Item1, item.Item2);
    }

    public string GetNickname(nint pet)
    {
        PetBase pBase = PluginLink.PettableUserHandler.GetPet(pet);
        if (pBase == null) return null!;
        string customName = pBase.RawCustomName;
        if (customName == string.Empty) return null!;
        return customName;
    }

    public void SetNickname(nint pet, string nickname)
    {
        if (!IsPetAccepted(pet)) return;
        if (!IsNicknameAccepted(ref nickname)) return;
        if (SetUser(pet, nickname)) return;
        HandleAsNonExistingUser(pet, nickname);
    }

    bool IsPetAccepted(nint pet)
    {
        PettableUser user = PluginLink.PettableUserHandler.GetUser(pet);
        if (user == null) return true;
        if (user.LocalUser) return false;
        return true;
    }

    bool IsNicknameAccepted(ref string nickname)
    {
        // No null allowed
        if (nickname == null) return false;
        // Nicknames cannot be longer than 64 characters
        if (nickname.Length > PluginConstants.ffxivNameSize) return false;
        // Don't set a name to a valid URL
        if (Uri.TryCreate(nickname, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)) return false;
        return true;
    }

    bool SetUser(nint pet, string nickname)
    {
        foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
        {
            foreach (PetBase p in user.Pets)
            {
                if (p.Pet == pet)
                {
                    p.SetIPCName(nickname);
                    user.SerializableUser.SaveNickname(p.ID, nickname, true, false, true);
                    return true;
                }
            }
        }
        return false;
    }

    unsafe void HandleAsNonExistingUser(nint pet, string nickname)
    {
        try
        {
            Character* gObject = (Character*)pet;
            if (gObject == null) return;    
            uint ownerID = gObject->CompanionOwnerID;

            BattleChara* owner = PluginLink.CharacterManager->LookupBattleCharaByObjectId(ownerID);
            if (owner == null) return;

            ushort homeworld = owner->Character.HomeWorld;
            string name = Marshal.PtrToStringUTF8((IntPtr)owner->Character.GameObject.Name)!;
            PluginLink.PettableUserHandler.DeclareUser(new SerializableUserV3(name, homeworld), UserDeclareType.IPC, true);
            PettableUserUtils.instance.Solve(PluginLink.PettableUserHandler.GetUser(name, homeworld)!, true);
            SetUser(pet, nickname);
        }
        catch { }
    }
}
