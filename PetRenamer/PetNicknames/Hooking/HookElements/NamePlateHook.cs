using Dalamud.Game.NativeWrapper;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Numerics;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class NamePlateHook : HookableElement
{
    private readonly Hook<RaptureAtkModule.Delegates.UpdateBattleCharaNameplates>? NameplateHook;
    private readonly Hook<RaptureAtkModule.Delegates.UpdateNpcNameplates>?         NameplateMinionHook;

    public NamePlateHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices) 
    {
         NameplateHook       = DalamudServices.Hooking.HookFromAddress<RaptureAtkModule.Delegates.UpdateBattleCharaNameplates>((nint)RaptureAtkModule.MemberFunctionPointers.UpdateBattleCharaNameplates, UpdateNameplateDetour);
         NameplateMinionHook = DalamudServices.Hooking.HookFromAddress<RaptureAtkModule.Delegates.UpdateNpcNameplates>        ((nint)RaptureAtkModule.MemberFunctionPointers.UpdateNpcNameplates,         UpdateNameplateNpcDetour);
    }

    public override void Init()
    {
        NameplateHook?.Enable();
        NameplateMinionHook?.Enable();
    }

    protected override void OnDispose()
    {
        NameplateHook?.Dispose();
        NameplateMinionHook?.Dispose();
    }

    public override void Refresh()
    {
        AtkUnitBasePtr namePlateAddon = DalamudServices.GameGui.GetAddonByName("NamePlate");

        if (namePlateAddon.IsNull)
        {
            return;
        }

        AddonNamePlate* addonNamePlate = (AddonNamePlate*)namePlateAddon.Address;

        if (addonNamePlate == null)
        {
            return;
        }

        addonNamePlate->DoFullUpdate = 1;
    }

    private int UpdateNameplateDetour(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, BattleChara* battleChara, int numArrayIndex, int stringArrayIndex)
    {
        try
        {
            SetNameplate(namePlateInfo, (nint)battleChara);
        }
        catch(Exception e)
        {
            PetServices.PetLog.LogException(e);
        }

        return NameplateHook!.Original(raptureAtkModule, namePlateInfo, numArray, stringArray, battleChara, numArrayIndex, stringArrayIndex);
    }

    public int UpdateNameplateNpcDetour(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, GameObject* gameObject, int numArrayIndex, int stringArrayIndex)
    {
        try
        {
            SetNameplate(namePlateInfo, (nint)gameObject);
        }
        catch(Exception e)
        {
            PetServices.PetLog.LogException(e);
        }

        return NameplateMinionHook!.Original(raptureAtkModule, namePlateInfo, numArray, stringArray, gameObject, numArrayIndex, stringArrayIndex);
    }

    private void SetNameplate(RaptureAtkModule.NamePlateInfo* namePlateInfo, nint obj)
    {
        if (!PetServices.Configuration.ShowOnNameplatesColour.Enabled)
        {
            return;
        }

        IPettablePet? pPet = PetServices.UserList.GetPet(obj);

        if (pPet == null)
        {
            return;
        }
        
        if (pPet.Owner == null)
        {
            return;
        }
        
        string? customPetName = pPet.Owner.DataBaseEntry.GetName(pPet.SkeletonId);

        if (customPetName.IsNullOrWhitespace())
        {
            return;
        }

        pPet.GetDrawColours(PetServices.Configuration.ShowOnNameplatesColour, out Vector3? edgeColour, out Vector3? textColour);

        SeString colouredPetName = PetServices.StringHelper.WrapInColor(customPetName, edgeColour, textColour);

        namePlateInfo->Name.SetString(colouredPetName.EncodeWithNullTerminator());
    }
}
