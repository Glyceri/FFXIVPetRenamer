using Dalamud.Game.Gui.FlyText;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class FlyTextHook : HookableElement
{
    public delegate void AddToScreenLogWithLogMessageId(IntPtr a1, IntPtr a2, int a3, char a4, int a5, int a6, int a7, int a8);
    public delegate IntPtr LogMethod(IntPtr a1, int a2, IntPtr a3, IntPtr a4);

    [Signature("E8 ?? ?? ?? ?? 8B 8C 24 ?? ?? ?? ?? 85 C9", DetourName = nameof(AddToScreenLogWithLogMessageIdDetour))]
    readonly Hook<AddToScreenLogWithLogMessageId>? addToScreenLogWithLogMessageId = null;

    [Signature("E8 ?? ?? ?? ?? 45 38 7E 1F", DetourName = nameof(LogMessageDetour))]
    readonly Hook<LogMethod>? logMethod = null;

    public FlyTextHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IPettableDirtyListener dirtyListener) : base(services, userList, petServices, dirtyListener) { }

    public override void Init()
    {
        DalamudServices.FlyTextGui.FlyTextCreated += OnFlyTextCreated;
        addToScreenLogWithLogMessageId?.Enable();
        logMethod?.Enable();
    }

    void OnFlyTextCreated(ref FlyTextKind kind, ref int val1, ref int val2, ref SeString text1, ref SeString text2, ref uint color, ref uint icon, ref uint damageTypeIcon, ref float yOffset, ref bool handled)
    {
        if (!PetServices.Configuration.showOnFlyout) return;

        nint lastCastDealer = (nint)PetServices.PetCastHelper.LastCastDealer;

        IPettablePet? casterPet = UserList.GetPet(lastCastDealer);
        if (casterPet == null) return;

        string? customName = casterPet.CustomName;
        if (customName == null) return;

        IPetSheetData? sheetData = casterPet.PetData;
        if (sheetData == null) return;

        PetServices.StringHelper.ReplaceSeString(ref text1, customName, sheetData);
        PetServices.StringHelper.ReplaceSeString(ref text2, customName, sheetData);
    }

    unsafe void AddToScreenLogWithLogMessageIdDetour(IntPtr target, IntPtr castDealer, int unkownCastFlag, char a4, int castID, int a6, int a7, int a8)
    {
        addToScreenLogWithLogMessageId?.Original(target, castDealer, unkownCastFlag, a4, castID, a6, a7, a8);
        
        PetServices.PetCastHelper.SetLatestCast((BattleChara*)target, (BattleChara*)castDealer, castID);

        // unkownCastFlag is most likely some cast status flag w/e thing
        // 534 means a cast FULLY succeeded, which is luckily the only part I really need for soft targeting
        if (unkownCastFlag != 534) return;
        if (PetServices.PetSheets.CastToSoftIndex((uint)castID) == null) return;
        UserList.GetUser(castDealer)?.OnLastCastChanged((uint)castID);
    }

    unsafe IntPtr LogMessageDetour(IntPtr a1, int a2, IntPtr a3, IntPtr a4)
    {
        BattleChara* chara = ((BattleChara*)a3);
        bool isValid = 640 == a2 || 642 == a2;

        PetServices.PetActionHelper.SetLatestUser(chara, isValid);
        return logMethod!.Original(a1, a2, a3, a4);
    }

    protected override void OnDispose()
    {
        DalamudServices.FlyTextGui.FlyTextCreated -= OnFlyTextCreated;
        addToScreenLogWithLogMessageId?.Dispose();
        logMethod?.Dispose();
    }
}
