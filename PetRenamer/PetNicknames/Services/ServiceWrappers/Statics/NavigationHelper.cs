using KamiToolKit.Nodes;
using KamiToolKit.Classes;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;

internal static class NavigationHelper
{
    public static unsafe void SetNavigation<T>(ref T nodeBase, ControllerNavigation controllerNavigation) where T : ComponentNode
    {
        nodeBase.ComponentBase->CursorNavigationInfo.Index = controllerNavigation.Index;
        nodeBase.ComponentBase->CursorNavigationInfo.LeftIndex = controllerNavigation.LeftIndex;
        nodeBase.ComponentBase->CursorNavigationInfo.RightIndex = controllerNavigation.RightIndex;
        nodeBase.ComponentBase->CursorNavigationInfo.UpIndex = controllerNavigation.UpIndex;
        nodeBase.ComponentBase->CursorNavigationInfo.DownIndex = controllerNavigation.DownIndex;

        if (controllerNavigation.LeftStop)
        {
            nodeBase.DrawFlags |= DrawFlags.DisableRapidLeft;
        }
        else
        {
            nodeBase.DrawFlags &= ~DrawFlags.DisableRapidLeft;
        }

        if (controllerNavigation.RightStop)
        {
            nodeBase.DrawFlags |= DrawFlags.DisableRapidRight;
        }
        else
        {
            nodeBase.DrawFlags &= ~DrawFlags.DisableRapidRight;
        }

        if (controllerNavigation.UpStop)
        {
            nodeBase.DrawFlags |= DrawFlags.DisableRapidUp;
        }
        else
        {
            nodeBase.DrawFlags &= ~DrawFlags.DisableRapidUp;
        }

        if (controllerNavigation.DownStop)
        {
            nodeBase.DrawFlags |= DrawFlags.DisableRapidDown;
        }
        else
        {
            nodeBase.DrawFlags &= ~DrawFlags.DisableRapidDown;
        }
    }
}
