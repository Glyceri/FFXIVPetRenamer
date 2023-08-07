using Dalamud.Game;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.Attributes;
using System.Collections.Generic;

namespace PetRenamer.Core.Updatable
{
    internal class UpdatableHandler : RegistryBase<Updatable, UpdatableAttribute>
    {
        List<Updatable> updatables => elements;

        public unsafe UpdatableHandler() 
        {
            PluginHandlers.Framework.Update += MainUpdate;
        }

        ~UpdatableHandler()
        {
            PluginHandlers.Framework.Update -= MainUpdate;
            ClearAllUpdatables();
        }

        public void ClearAllUpdatables() => ClearAllElements();

        void MainUpdate(Framework framework)
        {
            foreach (Updatable updatable in updatables)
                updatable.Update(framework);

            foreach (Updatable updatable in updatables)
                updatable.LateUpdate(framework);
        }
    }
}
