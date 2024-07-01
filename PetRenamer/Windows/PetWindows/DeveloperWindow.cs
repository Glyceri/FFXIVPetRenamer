using PetRenamer.Core.Handlers;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
internal class DeveloperWindow : PetWindow
{

    public DeveloperWindow() : base("Dev Window Pet Renamer")
    {
        if(PluginLink.Configuration.debugMode && PluginLink.Configuration.autoOpenDebug) IsOpen = true;
    }

   
    public override void OnDraw()
    {
       
    }
}