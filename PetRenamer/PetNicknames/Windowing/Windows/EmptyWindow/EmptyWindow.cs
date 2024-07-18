using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Base;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows.EmptyWindow;

internal class EmptyWindow : PetWindow
{
    protected override Vector2 MinSize { get; } = new Vector2(300, 300);
    protected override Vector2 MaxSize { get; } = new Vector2(1200, 1200);
    protected override Vector2 DefaultSize { get; } = new Vector2(800, 800);
    protected override bool HasModeToggle { get; } = true;
    protected override bool HasExtraButtons { get; } = true;

    protected override string Title { get; } = "Empty Window";
    protected override string ID { get; } = "EmptyWindow";

    public EmptyWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration, in IPetServices petServices, in IPettableUserList userList, in IPettableDatabase database, IPettableDatabase legacyDatabase, in IImageDatabase imageDatabase) : base(windowHandler, dalamudServices, configuration, "EmptyWindow")
    {
        //193902u  Background
        //195009u  Blob
        //68579u   Pet         


        // node.OnClick += _ => overlayNode.Style.ImageBlendMode += 1;
    }

    public override void OnDraw()
    {

    }
}
