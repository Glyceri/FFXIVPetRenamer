using Dalamud.Plugin;
using PetRenamer.Core.Ipc.PenumbraIPCHelper.Enums;
using Dalamud.Plugin.Ipc;
using PetRenamer.Core.Handlers;

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

    public static void RedrawObjectByIndex(int index)
    {
        if (!PluginLink.Configuration.redrawBattlePetOnSpawn) return;
        try
        {
            redrawObjectByIndex?.InvokeAction(index, 0);
        }
        catch { }
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
