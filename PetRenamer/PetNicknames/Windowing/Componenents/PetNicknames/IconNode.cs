using Dalamud.Interface.Textures;
using ImGuiNET;
using PetRenamer.PetNicknames.Services;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class IconNode : Node
{
    ISharedImmediateTexture SharedTexture { get; }
    Node INode;

    public uint? IconID { get => INode.Style.IconId; set => INode.Style.IconId = value; }

    public IconNode(in DalamudServices DalamudServices, uint iconId)
    {
        SharedTexture = DalamudServices.TextureProvider.GetFromGame("ui/uld/DragTargetA_hr1.tex");
        ChildNodes = [
            INode = new Node()
            {
                Style = new Style()
                {
                    IconId = iconId,
                    Opacity = 0.8f,
                    Anchor = Anchor.MiddleCenter,
                    Margin = new(16),
                }
            }
        ];

        BeforeReflow += _ => 
        {
            EdgeSize SelfPaddingSize = INode.ComputedStyle.Padding;
            EdgeSize SelfMarginSize = INode.ComputedStyle.Margin;
            INode.Style.Size = (ParentNode!.ComputedStyle.Size - SelfPaddingSize.Size - SelfMarginSize.Size) / ScaleFactor;
            return true;
        };
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        nint handle = SharedTexture.GetWrapOrEmpty().ImGuiHandle;
        Rect contentRect = Bounds.ContentRect;
        drawList.AddImageQuad(handle, contentRect.BottomRight, contentRect.BottomLeft, contentRect.TopLeft, contentRect.TopRight);
        base.OnDraw(drawList);
    }
}
