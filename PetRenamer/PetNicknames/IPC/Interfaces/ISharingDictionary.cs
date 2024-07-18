﻿using FFXIVClientStructs.FFXIV.Client.Game.Object;
using System;

namespace PetRenamer.PetNicknames.IPC.Interfaces;

internal interface ISharingDictionary : IDisposable
{
    void Clear();
    void Set(GameObjectId gameObjectID, string customName);
}
