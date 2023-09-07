﻿using Dalamud.Game.Gui.FlyText;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;
using PetRenamer.Core.Hooking.Structs;
using System;

namespace PetRenamer.Core.Hooking;

//Shamelessly stolen from: https://github.com/MidoriKami/KamiLib/blob/master/Hooking/Delegates.cs

public static unsafe class Delegates
{
    public delegate nint AddonOnSetup(AtkUnitBase* addon, int valueCount, AtkValue* values);
    public delegate void AddonDraw(AtkUnitBase* addon);
    public delegate byte AddonOnRefresh(AtkUnitBase* addon, int valueCount, AtkValue* values);
    public delegate void AddonFinalize(AtkUnitBase* addon);
    public delegate byte AddonUpdate(AtkUnitBase* addon);

    public delegate void AgentShow(AgentInterface* agent);
    public delegate nint AgentReceiveEvent(AgentInterface* agent, nint rawData, AtkValue* args, uint argCount, ulong sender);


    public delegate void OnActionUsedDelegate(uint sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
    public delegate void OnActorControlDelegate(uint entityId, uint id, uint unk1, uint type, uint unk2, uint unk3, uint unk4, uint unk5, UInt64 targetId, byte unk6);
    public delegate void OnCastDelegate(uint sourceId, IntPtr sourceCharacter);

    public delegate void* UpdateNameplateDelegate(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* battleChara, int numArrayIndex, int stringArrayIndex);
    public delegate void* UpdateNameplateNpcDelegate(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* gameObject, int numArrayIndex, int stringArrayIndex);

    public delegate bool TryActionDelegate(IntPtr tp, ActionType t, uint id, ulong target, uint param, uint origin, uint unk, void* l);
    //public delegate bool UseActionDelegate(IntPtr tp, ActionType t, uint id, ulong target, Vector3* loc, uint unk);
    //public delegate nint SetCooldown(IntPtr ptr, ActionType t, uint id);
    public delegate bool ActionOffCooldown(IntPtr ptr, ActionType t, uint id);

    public delegate void ReceiveActorControlSelfDelegate(uint entityId, uint type, uint a3, uint amount, uint a5, uint source, uint a7, uint a8, ulong a9, byte flag);

    public delegate void ReceiveAbilityDelegate(int sourceId, IntPtr sourceCharacter, IntPtr pos, ActionEffectHeader* effectHeader, ActionEffect* effectArray, ulong* effectTrail);
    public delegate void ActionIntegrityDelegate(uint targetId, IntPtr actionIntegrityData, bool isReplay);

    public delegate void EffectResultDelegate(uint sourceId, IntPtr* sourceCharacter, IntPtr pos, IntPtr* effectHeader, IntPtr* effectArray, ulong* effectTail);
    public delegate void TestDelegate(IntPtr a, UInt32 targetId, IntPtr dataPtr);
    public delegate void CreateVFX(Int64 a1);

    public delegate char sub_140341720(Int64 a1, Int64 a2, int a3, char a4, int a5);

    public delegate void AddLogMessage(int a1, IntPtr a2, int a4, int a5, float a6);
    public delegate void AddActionLogMessage(int a1, IntPtr a2, IntPtr a3, uint a4, uint a5, uint a6, uint a7, uint a8, uint a9, float a10);
    public delegate void AddToScreenLogWithLogMessageId(IntPtr a1, IntPtr a2, int a3, char a4, int a5, int a6, int a7, int a8);
    public delegate void AddToScreenLogWithScreenLogKind(IntPtr a1, IntPtr a2, int a3, char a4, char a9, int a5, int a6, int a7, int a8);

    public delegate void AddScreenLogDelegate(
        IntPtr target,
        IntPtr source,
        FlyTextKind logKind,
        int option,
        int actionKind,
        int actionId,
        int val1,
        int val2,
        int val3,
        int val4);
}

