using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class PetRenameNode : Node
{
    readonly Node RenameNode;
    readonly Node ImageNode;

    readonly IconNode IconNode;

    readonly Node TextNodeHolder;
    readonly Node TextNode;

    readonly IPettableUser User;
    readonly IPetSheetData ActivePet;

    public PetRenameNode(in IPettableUser user, in IPetSheetData activePet)
    {
        ActivePet = activePet;
        User = user;

        IconNode = new IconNode(activePet.Icon);
        IconNode.Style.Anchor = Anchor.MiddleRight;

        ChildNodes = [
            RenameNode = new Node()
            {
                Id = "RenamePortion",
                ChildNodes = [
                    TextNodeHolder = new Node()
                    {
                        Id = "TextNodeHolder",
                        Style = new Style()
                        {
                            BackgroundColor = new Color("Widget.Background"),

                        },
                        ChildNodes = 
                        [
                            TextNode = new Node()
                            {
                                Style = new Style()
                                {
                                    BackgroundColor = new Color(255, 255, 255),
                                },
                                NodeValue = $"Your {activePet.BaseSingular} is named {user.DataBaseEntry.GetName(activePet.Model)}"
                            }
                        ]
                    },
                    
                ]
            },
            ImageNode = new Node()
            {
                Id = "ImagePortion",
                ChildNodes = [
                    IconNode
                ]
            }
        ];

        BeforeReflow += _ =>
        {
            Style.Size = (ParentNode!.Bounds.ContentSize - ParentNode!.ComputedStyle.Padding.Size) / ScaleFactor;

            int height = Style.Size.Height;
            ImageNode.Style.Size = new Size(height, height);
            RenameNode.Style.Size = Style.Size - new Size(height, 0);

            IconNode.Style.Size = ImageNode.Style.Size;
            TextNodeHolder.Style.Size = RenameNode.Style.Size;

            TextNode.Style.Size = new Size(TextNodeHolder.Style.Size.Width, 50);

            return true;
        };
    }
}
