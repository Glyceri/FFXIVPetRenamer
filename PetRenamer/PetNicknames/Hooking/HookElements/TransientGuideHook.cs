using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using PetRenamer.PetNicknames.Hooking.Unsafe;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class TransientGuideHook : HookableElement
{
    private delegate nint RaptureTextModule_FormatAddonTransientDelegate(nint selfPtr, uint rowId, int unk3, nint unk4);

    [Signature("E8 ?? ?? ?? ?? 48 85 C0 74 0C 48 8D 4F 08", DetourName = nameof(RaptureTextModule_FormatAddonTransientDetour))]
    private readonly Hook<RaptureTextModule_FormatAddonTransientDelegate>? FormatAddonTransientHook = null;

    private readonly Dictionary<uint, TransientGuideString> _transientGuideStrings = [];

    // This has to actually become a dalamud service if more and more plugins want to use native ui and more importantly TransientGuides
    public TransientGuideHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IPettableDirtyListener dirtyListener) 
        : base(services, userList, petServices, dirtyListener) 
    {
        AddTransientGuideString(3, new SeStringBuilder()
            .AddIcon(BitmapFontIcon.ControllerButton3)
            .AddText(" to toggle Quick Select ")
            .AddIcon(BitmapFontIcon.ControllerButton0)
            .AddText(" to click button.")
            .Build());

        AddTransientGuideString(2, new SeStringBuilder()
            .AddIcon(BitmapFontIcon.ControllerButton3)
            .AddText(" to toggle Quick Select.")
            .Build());

        AddTransientGuideString(1, new SeStringBuilder()
            .AddIcon(BitmapFontIcon.ControllerShoulderRight)
            .Build());

        AddTransientGuideString(0, new SeStringBuilder()
            .AddIcon(BitmapFontIcon.ControllerShoulderLeft)
            .Build());
    }

    private void AddTransientGuideString(uint id, SeString seString)
    {
        uint actualId = id + PluginConstants.PET_NICKNAMES_TRANSIENT_OFFSET;

        _transientGuideStrings.Add(actualId, new TransientGuideString(seString));
    }

    public override void Init()
    {
        FormatAddonTransientHook?.Enable();
    }

    private nint RaptureTextModule_FormatAddonTransientDetour(nint selfPtr, uint rowId, int unk3, nint unk4)
    {
        if (rowId < PluginConstants.PET_NICKNAMES_TRANSIENT_OFFSET)
        {
            return FormatAddonTransientHook!.OriginalDisposeSafe(selfPtr, rowId, unk3, unk4);
        }

        if (!_transientGuideStrings.TryGetValue(rowId, out TransientGuideString? transientString))
        {
            return FormatAddonTransientHook!.OriginalDisposeSafe(selfPtr, rowId, unk3, unk4);
        }

        return transientString.String;
    }

    protected override void OnDispose()
    {
        FormatAddonTransientHook?.Dispose();
    }
}
