using Dalamud.Game;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using System.Collections.Generic;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable]
internal class AutoBattlePetUpdatable : Updatable
{
    ConfigurationUtils configurationUtils { get; set; } = null!;
    bool hasFilled = false;

    public AutoBattlePetUpdatable() : base()
    {
        configurationUtils = PluginLink.Utils.Get<ConfigurationUtils>();
    }

    public override void LateUpdate(Framework frameWork)
    {
        if (hasFilled) return;
        SerializableUser? localUser = configurationUtils.GetLocalUser();
        if (localUser == null) return;

        List<int> missingIDs = new List<int>();

        foreach(int id in PluginConstants.allowedNegativePetIDS)
        {
            bool found = false;
            foreach(SerializableNickname nickname in localUser.nicknames)
            {
                if(nickname.ID == id) found = true;
                if (found) break;
            }
            if(!found)
            missingIDs.Add(id);
        }

        foreach (int id in missingIDs)
        {
            configurationUtils.SetLocalNickname(id, "");
        }
        hasFilled = true;
    }

    public override void Update(Framework frameWork) { }
}
