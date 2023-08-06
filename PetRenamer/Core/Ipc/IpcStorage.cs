using System.Collections.Generic;

namespace PetRenamer;

public class IpcStorage
{
    // (string, uint) is the Equivelant of PetRenamer.Core.Serialization.SerializableUser

    public Dictionary<(string, uint), List<NicknameData>> IpcAssignedNicknames { get; } = new Dictionary<(string, uint), List<NicknameData>>();

}
