﻿using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkTooltipManager;

namespace PetRenamer.Core.Hooking;

//Shamelessly stolen from: https://github.com/MidoriKami/KamiLib/blob/master/Hooking/Delegates.cs
//And from: https://github.com/NightmareXIV/QuestAWAY/blob/master/QuestAWAY/QuestAWAY.cs#L235

public static unsafe class Delegates
{
    public delegate nint AddonOnSetup(AtkUnitBase* addon, int valueCount, AtkValue* values);
    public delegate void AddonDraw(AtkUnitBase* addon);
    public delegate byte AddonOnRefresh(AtkUnitBase* addon, int valueCount, AtkValue* values);
    public delegate void AddonFinalize(AtkUnitBase* addon);
    public delegate byte AddonUpdate(AtkUnitBase* addon);
    public delegate IntPtr AddonOnRequestedUpdate(IntPtr a1, IntPtr a2, IntPtr a3);

    public unsafe delegate void* AddonOnUpdate(AtkUnitBase* atkUnitBase, NumberArrayData* nums, StringArrayData* strings);

    public delegate void AgentShow(AgentInterface* agent);
    public delegate nint AgentReceiveEvent(AgentInterface* agent, nint rawData, AtkValue* args, uint argCount, ulong sender);

    public delegate void OnActionUsedDelegate(uint sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
    public delegate void OnActorControlDelegate(uint entityId, uint id, uint unk1, uint type, uint unk2, uint unk3, uint unk4, uint unk5, UInt64 targetId, byte unk6);
    public delegate void OnCastDelegate(uint sourceId, IntPtr sourceCharacter);

    public delegate void* UpdateNameplateDelegate(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* battleChara, int numArrayIndex, int stringArrayIndex);
    public delegate void* UpdateNameplateNpcDelegate(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* gameObject, int numArrayIndex, int stringArrayIndex);

    public delegate void AddLogMessage(int a1, IntPtr a2, int a4, int a5, float a6);
    public delegate void AddActionLogMessage(int a1, IntPtr a2, IntPtr a3, uint a4, uint a5, uint a6, uint a7, uint a8, uint a9, float a10);
    public delegate void AddToScreenLogWithLogMessageId(IntPtr a1, IntPtr a2, int a3, char a4, int a5, int a6, int a7, int a8);
    public delegate void AddToScreenLogWithScreenLogKind(IntPtr a1, IntPtr a2, int a3, char a4, char a9, int a5, int a6, int a7, int a8);

    public delegate void AddTooltip(AtkTooltipManager* atkTooltipManager, AtkTooltipType atkTooltipType, ushort parentID, AtkResNode* atkResNode, AtkTooltipArgs* atkTooltipArgs);
    public delegate void RemoveTooltip(AtkTooltipManager* atkTooltipManager, AtkResNode* atkResNode);
    public delegate void ShowTooltip(AtkTooltipManager* atkTooltipManager, AtkTooltipType tooltipType, ushort parentID, AtkResNode* atkResNode, AtkTooltipArgs* atkTooltipArgs, delegate* unmanaged[Stdcall]<float*, float*, void*> whatTheFock, bool bool1, bool bool2);
    public delegate AtkTooltipArgs* TooltipArgs(AtkTooltipArgs* atkTooltipArgs);

    public delegate int AccurateShowTooltip(AtkUnitBase* tooltip, byte a2, uint a3, IntPtr a4, IntPtr a5, IntPtr a6, char a7, char a8);

    public delegate char NaviMapTooltip(AtkUnitBase* tooltip, int a2);

    public delegate char AreaMapTooltipDelegate(AtkUnitBase* a1, uint a2, char a3);
    public delegate IntPtr AreaMapOnMouseMoveDelegate(AtkUnitBase* unk1, IntPtr unk2);
    public delegate IntPtr NaviMapOnMouseMoveDelegate(AtkUnitBase* unk1, IntPtr unk2, IntPtr unk3);

    public delegate void ProcessChatBoxDelegate(nint uiModule, nint message, nint unused, byte a4);
}

