// From: https://github.com/Kouzukii/ffxiv-deathrecap/blob/master/Game/ActionEffectDisplayType.cs

namespace PetRenamer.Core.Hooking.Structs;

public enum ActionEffectDisplayType : byte
{
    HideActionName = 0,
    ShowActionName = 1,
    ShowItemName = 2,
    MountName = 13
}
