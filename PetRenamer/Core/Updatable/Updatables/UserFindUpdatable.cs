using Dalamud.Game;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using PetRenamer.Windows.PetWindows;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable(-10)]
internal class UserFindUpdatable : Updatable
{
    public override void Update(Framework frameWork)
    {
        PluginLink.PettableUserHandler.LoopThroughUsers(OnUser);
    }

    unsafe void OnUser(PettableUser user)
    {
        if(user == null) return;

        user.Reset();
        if(!PluginLink.Configuration.displayCustomNames) return;

        BattleChara* bChara = PluginLink.CharacterManager->LookupBattleCharaByName(StringUtils.instance.MakeTitleCase(user.UserName.ToLowerInvariant()), true, (short)user.Homeworld);
        if (bChara == null) return;
        user.SetUser(bChara);

        if (user.SerializableUser.hasCompanion || user.LocalUser)
        {
            int companionIndex = bChara->Character.GameObject.ObjectIndex + 1;
            Companion* companion = (Companion*)GameObjectManager.GetGameObjectByIndex(companionIndex);
            user.SetCompanion(companion);
        }

        if (user.SerializableUser.hasBattlePet || user.LocalUser)
        {
            BattleChara* battlePet = PluginLink.CharacterManager->LookupPetByOwnerObject(bChara);
            user.SetBattlePet(battlePet);
        }
        if (!user.LocalUser) return;
        if (!user.AnyPetChanged) return;
        PetRenameWindow window = PluginLink.WindowHandler.GetWindow<PetRenameWindow>();
        if (window == null) return;
        if (user.BattlePetChanged) window.OpenForBattleID   (user.BattlePetID);
        if (user.CompanionChanged) window.OpenForId         (user.CompanionID);
    }
}