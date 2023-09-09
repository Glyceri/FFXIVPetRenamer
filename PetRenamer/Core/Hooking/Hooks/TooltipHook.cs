using Dalamud.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using System.Collections.Generic;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal class TooltipHook : QuickTextHookableElement
{
    internal override void OnQuickInit()
    {
        RegisterHook("ActionDetail", 5, -1);
        RegisterHook("Tooltip", 2, 3);
    }

    internal override void OnUpdate(Framework framework) =>
        OnBaseUpdate(framework, PluginLink.Configuration.displayCustomNames && PluginLink.Configuration.allowTooltips);
}

public unsafe static class TooltipHelper
{
    static BattleChara* nextTooltipIs = null!;

    public static BattleChara* GetNext() => nextTooltipIs;
    public static void ClearupNext() => nextTooltipIs = null!;

    public static List<PartyListInfo> partyListInfos = new List<PartyListInfo>();
}

public class PartyListInfo
{
    public string UserName = string.Empty;
    public bool hasPet;
    public bool hasChocobo;

    public PartyListInfo(string userName, bool hasPet, bool hasChocobo)
    {
        UserName = userName;
        this.hasPet = hasPet;
        this.hasChocobo = hasChocobo;
    }
}
