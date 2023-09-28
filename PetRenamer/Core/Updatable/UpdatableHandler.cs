using Dalamud.Plugin.Services;
using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.Attributes;
using System.Collections.Generic;
using System.Reflection;

namespace PetRenamer.Core.Updatable;

internal class UpdatableHandler : RegistryBase<Updatable, UpdatableAttribute>
{
    List<Updatable> updatables => elements;

    public UpdatableHandler() => PluginHandlers.Framework.Update += MainUpdate;
    protected override void OnDipose() => PluginHandlers.Framework.Update -= MainUpdate;
    protected override void OnAllRegistered() => updatables?.Sort(Compare);
    public void ClearAllUpdatables() => ClearAllElements();

    int Compare(Updatable a, Updatable b)
    {
        int aVal = a.GetType().GetCustomAttribute<UpdatableAttribute>()?.order ?? int.MaxValue;
        int bVal = b.GetType().GetCustomAttribute<UpdatableAttribute>()?.order ?? int.MaxValue;
        return aVal.CompareTo(bVal);
    }

    void MainUpdate(IFramework framework)
    {
        if (!(PluginHandlers.ClientState is { LocalPlayer: { } player })) return;

        foreach (Updatable updatable in updatables)
            updatable.Update(ref framework, ref player);
    }
}
