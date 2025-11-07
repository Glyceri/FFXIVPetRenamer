using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using PetRenamer.PetNicknames.KTKWindowing.Base;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes;

internal class PetFootstepIcon : KTKResNode
{
    private readonly IconImageNode FootstepImage;

    private IPetSheetData? _data;

    public PetFootstepIcon(KTKAddon parentAddon, KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, PettableDirtyHandler dirtyHandler)
        : base(parentAddon, windowHandler, dalamudServices, petServices, dirtyHandler)
    {
        IsVisible          = true;

        FootstepImage      = new IconImageNode
        {
            IconId         = 66310,
            TextureSize    = new Vector2(192, 192),
            IsVisible      = true,
            FitTexture     = true,
            ImageNodeFlags = (ImageNodeFlags)0xA0,
            Size           = Size
        };

        AttachNode(ref FootstepImage);

        OnDataUpdate();
    }

    private void OnDataUpdate()
    {
        if (_data == null)
        {
            FootstepImage.IsVisible = false;

            return;
        }

        if (_data.Model.SkeletonType != SkeletonType.Minion)
        {
            FootstepImage.IsVisible = false;

            return;
        }

        FootstepImage.IsVisible = true;
        FootstepImage.IconId    = _data.Icon + 65000;
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

        FootstepImage.Size = Size;

        OnDataUpdate();
    }
}
