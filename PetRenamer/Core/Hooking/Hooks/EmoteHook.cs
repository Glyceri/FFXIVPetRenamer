using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using Lumina.Excel.GeneratedSheets;
using PetRenamer.Core.Hooking.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PetRenamer.Core.Hooking.Hooks;

internal unsafe class EmoteHook : HookableElement
{
    /*
    // source https://github.com/Caraxi/SimpleTweaksPlugin/blob/278688543b936b2081b366ef80667ae48bb784e0/Tweaks/EmoteLogSubcommand.cs#L39
    // This will probably be used to change to your petname upon emote

    //"4C 8B DC 53 55 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 8B 2D"
    //or maybe this: https://github.com/RokasKil/EmoteLog/blob/2834e49a184da04b9f266fa46a401250b4eec5d7/EmoteLog/Hooks/EmoteReaderHook.cs#L24

    //look at this other approach for epic stuff: https://github.com/Ottermandias/Glamourer/blob/a2eb6ccc1f5c6e570e7ab29d1dfdcd5d8c037ba9/Glamourer/Interop/MetaService.cs#L19

    public delegate void* OnEmoteFuncDelegate(ulong unk, ulong instigatorAddr, ushort emoteId, ulong targetId, ulong unk2);

    [Signature("48 89 5c 24 08 48 89 6c 24 10 48 89 74 24 18 48 89 7c 24 20 41 56 48 83 ec 30 4c 8b 74 24 60 48 8b d9 48 81 c1 60 2f 00 00", DetourName = nameof(OnEmoteDetour))]
    private Hook<OnEmoteFuncDelegate>? emoteHook;

    void OnEmoteDetour(ulong unk, ulong instigatorAddr, ushort emoteId, ulong targetId, ulong unk2)
    {
        PluginLog.Log(emoteId.ToString());
        emoteHook!.Original(unk, instigatorAddr, emoteId, targetId, unk2);
    }

    internal override void OnInit()
    {
        emoteHook?.Enable();
    }

    internal override void OnDispose()
    {
        emoteHook?.Disable();
        emoteHook?.Dispose();
        emoteHook = null;
    }*/
}
