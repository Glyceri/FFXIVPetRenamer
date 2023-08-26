using Dalamud.Game;
using Dalamud.Logging;
using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.Attributes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace PetRenamer.Core.Updatable;

internal class UpdatableHandler : RegistryBase<Updatable, UpdatableAttribute>
{
    List<Updatable> updatables => elements;

    public UpdatableHandler() 
    {
        PluginHandlers.Framework.Update += MainUpdate;
    }

    protected override void OnDispose()
    {
        PluginHandlers.Framework.Update -= MainUpdate;
    }

    protected override void OnAllRegistered() => updatables?.Sort(Compare);
    

    int Compare(Updatable a, Updatable b)
    {
        int aVal = a.GetType().GetCustomAttribute<UpdatableAttribute>()?.order ?? int.MaxValue;
        int bVal = b.GetType().GetCustomAttribute<UpdatableAttribute>()?.order ?? int.MaxValue;
        return aVal.CompareTo(bVal);
    }

    public void ClearAllUpdatables() => ClearAllElements();

    //Stopwatch sw = new Stopwatch();
    //Stopwatch sw2 = new Stopwatch();
    void MainUpdate(Framework framework)
    {
        //sw2.Start();
        if (PluginHandlers.ClientState.LocalPlayer! == null) return;

        foreach (Updatable updatable in elements)
        {
            //sw.Start();
            updatable.Update(framework);
            //PluginLog.Log(updatable.GetType().Name.ToString() + ": " + sw.Elapsed.Microseconds.ToString());
            //sw.Reset();
        }
        //PluginLog.Log(GetType().Name.ToString() + ": " + sw2.Elapsed.Microseconds.ToString());
        //sw.Reset();
    }
}
