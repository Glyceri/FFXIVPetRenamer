using Dalamud.Game.Gui.FlyText;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.PettableUsers;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class FlyTextHook : HookableElement
{
    public delegate void AddToScreenLogWithLogMessageId(IntPtr a1, IntPtr a2, int a3, char a4, int a5, int a6, int a7, int a8);

    [Signature("E8 ?? ?? ?? ?? 8B 8C 24 ?? ?? ?? ?? 85 C9", DetourName = nameof(AddToScreenLogWithLogMessageIdDetour))]
    readonly Hook<AddToScreenLogWithLogMessageId>? addToScreenLogWithLogMessageId = null;

    public FlyTextHook(DalamudServices services, IPetServices petServices, IPettableUserList userList) : base(services, userList, petServices) { }

    public override void Init()
    {
        DalamudServices.FlyTextGui.FlyTextCreated += OnFlyTextCreated;
        addToScreenLogWithLogMessageId?.Enable();
    }

    void OnFlyTextCreated(ref FlyTextKind kind, ref int val1, ref int val2, ref SeString text1, ref SeString text2, ref uint color, ref uint icon, ref uint damageTypeIcon, ref float yOffset, ref bool handled)
    {

    }

    unsafe void AddToScreenLogWithLogMessageIdDetour(IntPtr target, IntPtr castDealer, int a3, char a4, int castID, int a6, int a7, int a8)
    {
        addToScreenLogWithLogMessageId?.Original(target, castDealer, a3, a4, castID, a6, a7, a8);
        
        PetServices.PetCastHelper.SetLatestCast((BattleChara*)target, (BattleChara*)castDealer, castID);

        // a3 is most likely some cast status flag w/e thing
        // 534 means a cast FULLY succeeded, which is luckily the only part I really need for soft targeting
        if (a3 != 534) return;
        if (PetServices.PetSheets.CastToSoftIndex((uint)castID) == null) return;
        PetServices.PetLog.Log("CAST: " + castID + " : " + a3 + " : " + a4 + " : " + a6 + " : " + a7 + " : " + a8);
        UserList.GetUser(castDealer)?.OnLastCastChanged((uint)castID);
    }

    public override void Dispose()
    {
        DalamudServices.FlyTextGui.FlyTextCreated -= OnFlyTextCreated;
        addToScreenLogWithLogMessageId?.Dispose();
    }
}
