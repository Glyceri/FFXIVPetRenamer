using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Classes;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.KTKWindowing.Base;

internal unsafe class NavigableComponent : KTKComponent
{
    private CollisionNode? standinNode;

    public NavigableComponent(KTKAddon parentAddon, KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, PettableDirtyHandler dirtyHandler) 
        : base(parentAddon, windowHandler, dalamudServices, petServices, dirtyHandler) 
    {
        CollisionNode.NodeFlags |= NodeFlags.Focusable;

        ParentAddon.RegisterNavigableComponent(this);
    }

    protected sealed override void OnDispose()
    {
        ParentAddon.DeregisterNavigableComponent(this);

        OnNavigableDispose();
    }

    public required byte NavigationIndex
    {
        get => CollisionNode.LinkedComponent->CursorNavigationInfo.Index;
        set => CollisionNode.LinkedComponent->CursorNavigationInfo.Index = value;
    }

    public CollisionNode? StandinNode
    {
        get => standinNode;
        set
        {
            if (standinNode != null)
            {
                standinNode.NodeFlags   &= ~(NodeFlags.Focusable | NodeFlags.HasCollision | NodeFlags.RespondToMouse);
            }

            CollisionNode.NodeFlags |= NodeFlags.Focusable | NodeFlags.HasCollision | NodeFlags.RespondToMouse;
            NodeFlags               |= NodeFlags.Focusable | NodeFlags.HasCollision | NodeFlags.RespondToMouse;

            standinNode = value;

            if (standinNode != null)
            {
                standinNode.NodeFlags   |= NodeFlags.Focusable | NodeFlags.HasCollision | NodeFlags.RespondToMouse;

                CollisionNode.NodeFlags &= ~(NodeFlags.Focusable | NodeFlags.HasCollision | NodeFlags.RespondToMouse);
                NodeFlags               &= ~(NodeFlags.Focusable | NodeFlags.HasCollision | NodeFlags.RespondToMouse);
            }
        }
    }

    public byte LeftIndex
    {
        get => CollisionNode.LinkedComponent->CursorNavigationInfo.LeftIndex;
        set => CollisionNode.LinkedComponent->CursorNavigationInfo.LeftIndex = value;
    }

    public byte RightIndex
    {
        get => CollisionNode.LinkedComponent->CursorNavigationInfo.RightIndex;
        set => CollisionNode.LinkedComponent->CursorNavigationInfo.RightIndex = value;
    }

    public byte UpIndex
    {
        get => CollisionNode.LinkedComponent->CursorNavigationInfo.UpIndex;
        set => CollisionNode.LinkedComponent->CursorNavigationInfo.UpIndex = value;
    }

    public byte DownIndex
    {
        get => CollisionNode.LinkedComponent->CursorNavigationInfo.DownIndex;
        set => CollisionNode.LinkedComponent->CursorNavigationInfo.DownIndex = value;
    }

    public bool UpStopFlag
    {
        get => DrawFlags.HasFlag(DrawFlags.DisableRapidUp);
        set
        {
            if (value)
            {
                DrawFlags |= DrawFlags.DisableRapidUp;
            }
            else
            {
                DrawFlags &= ~DrawFlags.DisableRapidUp;
            }
        }
    }

    public bool DownStopFlag
    {
        get => DrawFlags.HasFlag(DrawFlags.DisableRapidDown);
        set
        {
            if (value)
            {
                DrawFlags |= DrawFlags.DisableRapidDown;
            }
            else
            {
                DrawFlags &= ~DrawFlags.DisableRapidDown;
            }
        }
    }

    public bool LeftStopFlag
    {
        get => DrawFlags.HasFlag(DrawFlags.DisableRapidLeft);
        set
        {
            if (value)
            {
                DrawFlags |= DrawFlags.DisableRapidLeft;
            }
            else
            {
                DrawFlags &= ~DrawFlags.DisableRapidLeft;
            }
        }
    }

    public bool RightStopFlag
    {
        get => DrawFlags.HasFlag(DrawFlags.DisableRapidRight);
        set
        {
            if (value)
            {
                DrawFlags |= DrawFlags.DisableRapidRight;
            }
            else
            {
                DrawFlags &= ~DrawFlags.DisableRapidRight;
            }
        }
    }

    protected virtual void OnNavigableDispose() { }
}
