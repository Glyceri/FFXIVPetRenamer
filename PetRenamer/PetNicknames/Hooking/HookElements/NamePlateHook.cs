using Dalamud.Game.NativeWrapper;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Utility;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Numerics;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class NamePlateHook : HookableElement
{
    private delegate nint UpdateNameplateDelegate(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, BattleChara* battleChara, int numArrayIndex, int stringArrayIndex);
    private delegate nint UpdateNameplateNpcDelegate(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, GameObject* gameObject, int numArrayIndex, int stringArrayIndex);

    [Signature("40 53 55 57 41 56 48 81 EC ?? ?? ?? ?? 48 8B 84 24", DetourName = nameof(UpdateNameplateDetour))]
    private readonly Hook<UpdateNameplateDelegate>? NameplateHook = null;

    [Signature("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 4C 89 44 24 ?? 57 41 54 41 55 41 56 41 57 48 83 EC 20 48 8B 7C 24", DetourName = nameof(UpdateNameplateNpcDetour))]
    private readonly Hook<UpdateNameplateNpcDelegate>? NameplateMinionHook = null;

    public NamePlateHook(DalamudServices services, IPetServices petServices, IPettableUserList pettableUserList, IPettableDirtyListener dirtyListener) 
        : base(services, pettableUserList, petServices, dirtyListener) 
    {

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

    protected override void Refresh()
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

    private nint UpdateNameplateDetour(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, BattleChara* battleChara, int numArrayIndex, int stringArrayIndex)
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

    public nint UpdateNameplateNpcDetour(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* gameObject, int numArrayIndex, int stringArrayIndex)
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
        if (!PetServices.Configuration.showOnNameplates)
        {
            return;
        }

        IPettablePet? pPet = UserList.GetPet(obj);

        if (pPet == null)
        {
            return;
        }

        string? customPetName = pPet.CustomName;

        if (customPetName.IsNullOrWhitespace())
        {
            return;
        }

        pPet.GetDrawColours(out Vector3? edgeColour, out Vector3? textColour);

        SeString colouredPetName = PetServices.StringHelper.WrapInColor(customPetName, edgeColour, textColour);

        namePlateInfo->Name.SetString(colouredPetName.EncodeWithNullTerminator());
    }
}
