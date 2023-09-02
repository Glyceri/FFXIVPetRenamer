﻿// From: https://github.com/Kouzukii/ffxiv-deathrecap/blob/master/Game/ActionEffectType.cs

namespace PetRenamer.Core.Hooking.Structs;

public enum ActionEffectType : byte
{
    Nothing = 0,
    Miss = 1,
    FullResist = 2,
    Damage = 3,
    Heal = 4,
    BlockedDamage = 5,
    ParriedDamage = 6,
    Invulnerable = 7,
    NoEffectText = 8,
    MpLoss = 10,
    MpGain = 11,
    TpLoss = 12,
    TpGain = 13,
    ApplyStatusEffectTarget = 14,
    ApplyStatusEffectSource = 15,
    RecoveredFromStatusEffect = 16,
    LoseStatusEffectTarget = 17,
    LoseStatusEffectSource = 18,
    StatusNoEffect = 20,
    ThreatPosition = 24,
    EnmityAmountUp = 25,
    EnmityAmountDown = 26,
    StartActionCombo = 27,
    Knockback = 33,
    Mount = 40,
    FullResistStatus = 55,
    Vfx = 59,
    Gauge = 60,
    PartialInvulnerable = 74,
    Interrupt = 75,
}
