using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.PetWindows;
using System.Collections.Generic;

namespace PetRenamer.Core.Ipc.FindAnythingIPCHelper;

public static class FindAnythingIPCProvider
{
    static ICallGateSubscriber<string, string, uint, string>? cgRegister;
    static ICallGateSubscriber<string, bool>? cgUnregisterAll;
    static ICallGateSubscriber<string, bool>? cgInvoke;

    static List<(string, int)> guids = new List<(string, int)>();

    public static void Init(ref DalamudPluginInterface dalamudPluginInterface)
    {
        cgRegister = dalamudPluginInterface.GetIpcSubscriber<string, string, uint, string>("FA.Register");
        cgUnregisterAll = dalamudPluginInterface.GetIpcSubscriber<string, bool>("FA.UnregisterAll");
        cgInvoke = dalamudPluginInterface.GetIpcSubscriber<string, bool>("FA.Invoke");

        cgInvoke.Subscribe(OnRename);
    }

    static void OnRename(string guid)
    {
        foreach ((string, int) idel in guids)
        {
            if (guid != idel.Item1) continue;
            PetRenameWindow window = PluginLink.WindowHandler.GetWindow<PetRenameWindow>();
            if (window == null) break;
            if (idel.Item2 > -1) window.OpenForMinion(idel.Item2, true);
            else window.OpenForBattlePet(idel.Item2, true);
            break;
        }
    }

    public static void RegisterInitialNames()
    {
        Deregister();
        PettableUser localUser = PluginLink.PettableUserHandler.LocalUser()!;
        if (localUser == null) return;
        localUser.SerializableUser.LoopThrough((pet) =>
        {
            if (pet.Item1 == -1 || pet.Item2 == null || pet.Item2 == string.Empty) return;
            Register("Rename: ", pet.Item2, pet.Item1, RemapUtils.instance.GetTextureID(pet.Item1));
        });
    }

    static void Register(string prefix, string value, int petID, uint textureID)
    {
        try
        {
            string guid = cgRegister?.InvokeFunc(PluginConstants.internalName, prefix + value, textureID) ?? null!;
            if (guid == null || guid == string.Empty) return;
            guids.Add((guid, petID));
        }
        catch { }
    }

    public static void Deregister()
    {
        try
        {
            cgUnregisterAll?.InvokeFunc(PluginConstants.internalName);
        }
        catch { }
    }

    public static void DeInit()
    {
        guids.Clear();
        Deregister();
        cgInvoke?.Unsubscribe(OnRename);
    }
}
