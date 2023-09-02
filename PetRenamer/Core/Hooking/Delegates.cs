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

    public delegate void ReceiveAbilityDelegate(int sourceId, IntPtr sourceCharacter, IntPtr pos, ActionEffectHeader* effectHeader, ActionEffect* effectArray, ulong* effectTrail);
    public delegate void ActionIntegrityDelegate(uint targetId, IntPtr actionIntegrityData, bool isReplay);
}

