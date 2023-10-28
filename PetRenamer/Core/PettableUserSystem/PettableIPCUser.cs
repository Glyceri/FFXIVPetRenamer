using PetRenamer.Core.Serialization;

namespace PetRenamer.Core.PettableUserSystem;

public class PettableIPCUser : PettableUser
{
    public PettableIPCUser(string username, ushort homeworld, SerializableUserV3 serializableUser) : base(username, homeworld, serializableUser) => IsIPCOnlyUser = true;
}
