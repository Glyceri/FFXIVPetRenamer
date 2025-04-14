using Dalamud.Game.Gui.FlyText;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal class FlyTextHook : HookableElement
{
    delegate void AddToScreenLogWithLogMessageId(nint a1, nint a2, int a3, char a4, int a5, int a6, int a7, int a8);
    delegate nint LogMethod(nint a1, int a2, nint a3, nint a4);

    [Signature("E8 ?? ?? ?? ?? 8B 8C 24 ?? ?? ?? ?? 85 C9", DetourName = nameof(AddToScreenLogWithLogMessageIdDetour))]
    readonly Hook<AddToScreenLogWithLogMessageId>? addToScreenLogWithLogMessageId = null;

    [Signature("E8 ?? ?? ?? ?? 45 38 7E 25", DetourName = nameof(LogMessageDetour))]
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

        nint lastCastDealer = PetServices.PetCastHelper.LastCastDealer;

        IPettablePet? casterPet = UserList.GetPet(lastCastDealer);
        if (casterPet == null) return;

        string? customName = casterPet.CustomName;
        if (customName == null) return;

        IPetSheetData? sheetData = casterPet.PetData;
        if (sheetData == null) return;

        PetServices.StringHelper.ReplaceSeString(ref text1, customName, sheetData);
        PetServices.StringHelper.ReplaceSeString(ref text2, customName, sheetData);
    }

    unsafe void AddToScreenLogWithLogMessageIdDetour(nint target, nint castDealer, int unkownCastFlag, char a4, int castID, int a6, int a7, int a8)
    {
        addToScreenLogWithLogMessageId?.Original(target, castDealer, unkownCastFlag, a4, castID, a6, a7, a8);
        
        PetServices.PetCastHelper.SetLatestCast(target, castDealer, castID);

        // unkownCastFlag is most likely some cast status flag w/e thing
        // 534 means a cast FULLY succeeded, which is luckily the only part I really need for soft targeting
        if (unkownCastFlag != 534) return;
        if (PetServices.PetSheets.CastToSoftIndex((uint)castID) == null) return;

        IPettableUser? user = UserList.GetUser(castDealer);
        if (user == null) return;

        user.OnLastCastChanged((uint)castID);
    }

    nint LogMessageDetour(nint a1, int a2, nint user, nint a4)
    {
        bool isValid = 640 == a2 || 642 == a2;

        PetServices.PetActionHelper.SetLatestUser(user, isValid);
        return logMethod!.Original(a1, a2, user, a4);
    }

    protected override void OnDispose()
    {
        DalamudServices.FlyTextGui.FlyTextCreated -= OnFlyTextCreated;
        addToScreenLogWithLogMessageId?.Dispose();
        logMethod?.Dispose();
    }
}
