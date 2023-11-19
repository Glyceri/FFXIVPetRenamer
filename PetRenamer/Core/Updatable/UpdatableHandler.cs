using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.Attributes;
using System.Collections.Generic;
using System.Reflection;

namespace PetRenamer.Core.Updatable;

internal class UpdatableHandler : RegistryBase<Updatable, UpdatableAttribute>
{
    public void ReleaseUpdate() => PluginHandlers.Framework.Update += MainUpdate;
    protected override void OnDipose() => PluginHandlers.Framework.Update -= MainUpdate;
    protected override void OnAllRegistered() => elements?.Sort(Compare);
    public void ClearAllUpdatables() => ClearAllElements();

    bool hasRemovables = false;
    List<Updatable> removables = new List<Updatable>();

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

        if (!hasRemovables) return;
        RemoveRemovables();
    }

    void RemoveRemovables() 
    {
        hasRemovables = false;

        for(int i = removables.Count - 1; i >= 0; i--)
        {
            Updatable removable = removables[i];
            int index = elements.IndexOf(removable);
            if (index == -1) continue;
            elements.RemoveAt(index);
            attributes.RemoveAt(index);
            removable.Dispose();
        }

        removables.Clear();
    }

    public void RegisterRemovable(Updatable updatable)
    {
        removables.Add(updatable);
        hasRemovables = true;
    }
}
