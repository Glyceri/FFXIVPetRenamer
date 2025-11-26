using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal unsafe interface IBattleUser : IPettableEntity
{
    public string       Name          { get; }
    public ulong        ContentID     { get; }
    public ushort       Homeworld     { get; }
    public ulong        ObjectID      { get; }
    public uint         ShortObjectID { get; }
    public uint         CurrentCastID { get; }
    public BattleChara* BattleChara   { get; }
}
