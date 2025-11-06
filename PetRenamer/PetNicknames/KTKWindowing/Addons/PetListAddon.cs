using PetRenamer.PetNicknames.KTKWindowing.Base;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Addons;

internal class PetListAddon : KTKAddon
{
    [SetsRequiredMembers]
    public PetListAddon(KTKWindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDatabase database, PettableDirtyHandler dirtyHandler)
       : base(windowHandler, dalamudServices, petServices, userList, database, dirtyHandler) { }

    protected override string WindowInternalName
        => nameof(PetListAddon);

    protected override Vector2 WindowSize
        => new Vector2(520, 200);

    protected override bool HasPetBar
        => true;

    public override string WindowTooltip
        => "Pet List & Sharing";
}
