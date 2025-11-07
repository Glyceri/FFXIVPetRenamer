using KamiToolKit.Nodes;
using PetRenamer.PetNicknames.KTKWindowing.Base;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes;

internal class PetImageNode : KTKResNode
{
    private readonly IconImageNode      MinionImage;
    private readonly VerminionImageNode MinionTypeNode;

    private IPetSheetData? _data;

    public PetImageNode(KTKAddon parentAddon, KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, PettableDirtyHandler dirtyHandler) 
        : base(parentAddon, windowHandler, dalamudServices, petServices, dirtyHandler)
    {
        IsVisible       = true;

        MinionImage     = new IconImageNode
        {
            IconId      = 66310,
            TextureSize = new Vector2(192, 192),
            IsVisible   = true,
            FitTexture  = true,
            Size        = Size
        };

        AttachNode(ref MinionImage);

        MinionTypeNode  = new VerminionImageNode()
        {
            Size        = Size,
        };

        petServices.NativeController.AttachNode(MinionTypeNode, MinionImage);

        OnDataUpdate();
    }

    private void OnDataUpdate()
    {
        if (_data == null)
        {
            MinionImage.IconId        = 66310;
            MinionTypeNode.MinionRace = 0;

            return;
        }

        uint adder = 0;

        if (_data.Model.SkeletonType == SkeletonType.Minion)
        {
            if (PetServices.Configuration.minionIconType == 1)
            {
                adder = 64000;
            }
            else if (PetServices.Configuration.minionIconType == 2)
            {
                adder = 55000;
            }
        }

        MinionImage.IconId        = _data.Icon + adder;
        MinionTypeNode.MinionRace = _data.RaceID;
    }

    public IPetSheetData? PetData
    {
        get => _data;
        set
        {
            _data = value;

            OnDataUpdate();
        }
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        MinionImage.Size    = Size;
        MinionTypeNode.Size = MinionImage.Size * new Vector2(0.193f, 0.191f);
        MinionTypeNode.X    = MinionImage.Size.X * 0.707f;
        MinionTypeNode.Y    = MinionImage.Size.Y * 0.01f;

        OnDataUpdate();
    }
}
