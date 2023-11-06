using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Handlers;
using PetRenamer.Logging;
using PetRenamer.Windows.Attributes;
using System.Reflection;

namespace PetRenamer.Core.Updatable;

internal class UpdatableHandler : RegistryBase<Updatable, UpdatableAttribute>
{
    public void ReleaseUpdate() => PluginHandlers.Framework.Update += MainUpdate;
    protected override void OnDipose() => PluginHandlers.Framework.Update -= MainUpdate;
    protected override void OnAllRegistered() => elements?.Sort(Compare);
    public void ClearAllUpdatables() => ClearAllElements();

    int Compare(Updatable a, Updatable b)
    {
        int aVal = a.GetType().GetCustomAttribute<UpdatableAttribute>()?.order ?? int.MaxValue;
        int bVal = b.GetType().GetCustomAttribute<UpdatableAttribute>()?.order ?? int.MaxValue;
        return aVal.CompareTo(bVal);
    }

    void MainUpdate(IFramework framework)
    {
        PlayerCharacter player = PluginHandlers.ClientState.LocalPlayer!;
        if (player == null) return;

        int elementCount = elements.Count;
        for(int i = 0; i < elementCount; i++)
            elements[i].Update(ref framework, ref player);
    }
}
