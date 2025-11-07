using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes.StyledNodes;

internal class MaskTest : ResNode
{
    private IDrawListTextureWrap DrawListTextureWrap;

    private ImGuiImageNode imageNode;

    public MaskTest(DalamudServices dalamudServices, IPetServices petServices)
    {

    }


    public void Draw()
    {

    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();
    }

    protected override void Dispose(bool disposing, bool isNativeDestructor)
    {
       
    }
}
