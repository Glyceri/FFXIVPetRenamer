using PetRenamer.Core.Singleton;
using PetRenamer.Utilization.Attributes;
using PetRenamer.Utilization.Enum;
using TargetObjectKind = Dalamud.Game.ClientState.Objects.Enums.ObjectKind;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class EnumUtils : UtilsRegistryType, ISingletonBase<EnumUtils>
{
    public static EnumUtils instance { get; set; } = null!;

    public PetType FromTargetType(TargetObjectKind targetObjectKind)
    {
        if (targetObjectKind == TargetObjectKind.BattleNpc) return PetType.BattlePet;
        return PetType.Companion;
    }
}
