using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PetRenamer.Core.Hooking.Hooks;

internal class EmoteHook : HookableElement
{

    // source https://github.com/Caraxi/SimpleTweaksPlugin/blob/278688543b936b2081b366ef80667ae48bb784e0/Tweaks/EmoteLogSubcommand.cs#L39
    // This will probably be used to change to your petname upon emote
    /*
    //"4C 8B DC 53 55 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 8B 2D"
    //or maybe this: https://github.com/RokasKil/EmoteLog/blob/2834e49a184da04b9f266fa46a401250b4eec5d7/EmoteLog/Hooks/EmoteReaderHook.cs#L24

    [StructLayout(LayoutKind.Explicit, Size = 0x10)]
    public struct EmoteCommandStruct
    {
        [FieldOffset(0x08)] public short TextCommandParam;
    }

    internal override void OnInit()
    {
        
    }

    private void* ExecuteDetour(void* a1, EmoteCommandStruct* command, void* a3)
    {
        var didEnable = false;
        try
        {
            if (command->TextCommandParam is 20 or 21)
            {
                if (!EmoteTextType)
                {
                    EmoteTextType = didEnable = true;
                }
            }
            return executeEmoteCommandHook.Original(a1, command, a3);
        }
        catch (Exception ex)
        {
            return executeEmoteCommandHook.Original(a1, command, a3);
        }
        finally
        {
            if (didEnable) EmoteTextType = false;
        }
    }

    internal override void OnDispose()
    {
        
    }*/
}
