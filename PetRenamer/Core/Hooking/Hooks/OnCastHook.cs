using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Common.Math;
using Lumina.Excel.GeneratedSheets;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.Hooking.Structs;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using System;
using System.Runtime.InteropServices;

namespace PetRenamer.Core.Hooking.Hooks;

// Signatures from: https://github.com/Tischel/ActionTimeline/blob/master/ActionTimeline/Helpers/TimelineManager.cs
// Signatures from: https://github.com/cairthenn/Redirect/blob/main/Redirect/GameHooks.cs
//and from https://github.com/Kouzukii/ffxiv-deathrecap/blob/master/Events/CombatEventCapture.cs
// I store these so when they inevitably change, I can just yoink them again from there.

[Hook]
internal class OnCastHook : HookableElement
{
    [Signature("40 55 53 57 41 54 41 55 41 56 41 57 48 8D AC 24 ?? ?? ?? ?? 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 45 70", DetourName = nameof(OnActionUsed))]
    Hook<Delegates.OnActionUsedDelegate>? _onActionUsedHook;

    Hook<Delegates.TryActionDelegate> TryActionHook = null!;

    [Signature("40 55 53 57 41 54 41 55 41 56 41 57 48 8D AC 24 60 FF FF FF 48 81 EC A0 01 00 00", DetourName = nameof(ReceiveAbilityEffectDetour))]
    Hook<Delegates.ReceiveAbilityDelegate> receiveAbilityEffectHook = null!;


    [Signature("48 8B C4 44 88 40 18 89 48 08", DetourName = nameof(ActionIntegrityDelegateDetour))]
    private readonly Hook<Delegates.ActionIntegrityDelegate> actionIntegrityDelegateHook = null!;

    unsafe internal override void OnInit()
    {
        TryActionHook ??= Hook<Delegates.TryActionDelegate>.FromAddress((IntPtr)ActionManager.MemberFunctionPointers.UseAction, TryActionCallback);
        TryActionHook?.Enable();
        _onActionUsedHook?.Enable();
        receiveAbilityEffectHook?.Enable();
        actionIntegrityDelegateHook?.Enable();
    }

    private unsafe void ReceiveAbilityEffectDetour(int sourceId, IntPtr sourceCharacter, IntPtr pos, ActionEffectHeader* effectHeader, ActionEffect* effectArray, ulong* effectTrail)
    {
        PluginLog.Log("Received Ability");
        receiveAbilityEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
    }

    unsafe void OnActionUsed(uint sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail)
    {
        _onActionUsedHook?.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);

        uint actionId = (uint)Marshal.ReadInt32(effectHeader, 0x8);
        string actionName = SheetUtils.instance.GetAction(actionId)?.Name ?? string.Empty;

        PluginLink.PettableUserHandler.SetLastCast(sourceId, actionId, actionName);
    }

    private unsafe bool TryActionCallback(IntPtr action_manager, ActionType type, uint id, ulong target, uint param, uint origin, uint unk, void* location)
    {
        if (type != ActionType.Spell)
        {
            return TryActionHook.Original(action_manager, type, id, target, param, origin, unk, location);
        }

        bool returnThing = TryActionHook!.Original(action_manager, type, id, target, param, origin, unk, location);

        if (returnThing)
        {
            uint actionId = (uint)ActionManager.MemberFunctionPointers.GetAdjustedActionId((ActionManager*)action_manager, id);
            string actionName = SheetUtils.instance.GetAction(actionId)?.Name ?? string.Empty;
            if (GetElapsedCastTime((ActionManager*)action_manager, actionId) == 0)
                PluginLink.PettableUserHandler.SetLastUsedAction(id, actionId, actionName);
        }
        return returnThing;
    }

    unsafe public static float GetElapsedCastTime(ActionManager* action_manager, uint actionId)
    {
        uint adjustedId = action_manager->GetAdjustedActionId(actionId);
        return action_manager->GetRecastTimeElapsed(ActionType.Spell, adjustedId);
    }

    unsafe public static float GetCastTime(ActionManager* action_manager, uint actionId)
    {
        uint adjustedId = action_manager->GetAdjustedActionId(actionId);
        return action_manager->GetRecastTime(ActionType.Spell, adjustedId);
    }

    private unsafe void ActionIntegrityDelegateDetour(uint targetId, IntPtr actionIntegrityData, bool isReplay)
    {
        actionIntegrityDelegateHook.Original(targetId, actionIntegrityData, isReplay);
    }

    internal override void OnDispose()
    {
        _onActionUsedHook?.Disable();
        TryActionHook?.Disable();
        receiveAbilityEffectHook?.Disable();
        actionIntegrityDelegateHook?.Disable();

        actionIntegrityDelegateHook?.Dispose();
        receiveAbilityEffectHook?.Dispose();
        TryActionHook?.Dispose();
        _onActionUsedHook?.Dispose();
    }
}