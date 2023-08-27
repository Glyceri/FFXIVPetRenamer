using Dalamud.Game;
using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.Attributes;
using System.Collections.Generic;
using System.Reflection;

namespace PetRenamer.Core.Updatable
{
    internal class UpdatableHandler : RegistryBase<Updatable, UpdatableAttribute>
    {
        List<Updatable> updatables => elements;

        public unsafe UpdatableHandler() 
        {
            PluginHandlers.Framework.Update += MainUpdate;
        }

        protected override void OnDipose()
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

        void MainUpdate(Framework framework)
        {
            if (PluginHandlers.ClientState.LocalPlayer! == null) return;

            foreach (Updatable updatable in updatables)
                updatable.Update(framework);

            foreach (Updatable updatable in updatables)
                updatable.LateUpdate(framework);
        }
    }
}
