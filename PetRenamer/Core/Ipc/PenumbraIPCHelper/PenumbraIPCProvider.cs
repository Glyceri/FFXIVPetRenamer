using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Ipc.PenumbraIPCHelper.Enums;
using PetRenamer.Logging;
using System;

namespace PetRenamer.Core.Ipc.PenumbraIPCHelper;

public static class PenumbraIPCProvider
{
    public static ICallGateSubscriber<int, RedrawType, object>? redrawObjectByIndex;
    public static ICallGateSubscriber<bool>? getEnabledState;

    public static void Init(ref DalamudPluginInterface dalamudPluginInterface)
    {
        redrawObjectByIndex = dalamudPluginInterface.GetIpcSubscriber<int, RedrawType, object>("Penumbra.RedrawObjectByIndex");
        getEnabledState = dalamudPluginInterface.GetIpcSubscriber<bool>("Penumbra.GetEnabledState");
    }

    public static void RedrawPetByIndex(int index)
    {
        if (index == -1) return;
        if (index > -1) RedrawMinionByIndex(index);
        if (index < -1) RedrawBattlePetByIndex(index);
    }

    public static void RedrawBattlePetByIndex(int index)
    {
        if (!PluginLink.Configuration.redrawBattlePetOnSpawn) return;
        RedrawObjectByIndex(index);
    }

    public static void RedrawMinionByIndex(int index)
    {
        if (!PluginLink.Configuration.redrawMinionOnSpawn) return;
        RedrawObjectByIndex(index);
    }

    public static void RedrawObjectByIndex(int index)
    {
        if (!PluginLink.Configuration.understoodWarningThirdPartySettings) return;
        try
        {
            redrawObjectByIndex?.InvokeAction(index, 0);
        }
        catch(Exception e) { PetLog.Log(e.Message); }
    }

    public static bool PenumbraEnabled()
    {
        try
        {
            return getEnabledState?.InvokeFunc() ?? false;
        }
        catch
        {
            return false;
        }
    }

    public static void DeInit()
    {
        
    }
}
