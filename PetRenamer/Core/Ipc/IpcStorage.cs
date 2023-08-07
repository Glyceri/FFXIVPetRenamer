using PetRenamer.Core.Updatable.Updatables;
using System.Collections.Generic;

namespace PetRenamer;

public class IpcStorage
{
    // (string, uint) is the Equivelant of PetRenamer.Core.Serialization.SerializableUser
    // Which is now Obsolete ;)

    public delegate void OnIpcChange(Dictionary<(string, uint), NicknameData> change);
    public event OnIpcChange IpcChange = null!;

    private Dictionary<(string, uint), NicknameData> _IpcAssignedNicknames = new Dictionary<(string, uint), NicknameData>();

    public List<FoundPlayerCharacter> characters = new List<FoundPlayerCharacter>();

    public Dictionary<(string, uint), NicknameData> IpcAssignedNicknames 
    {
        get => _IpcAssignedNicknames; 
        set
        {
            _IpcAssignedNicknames = value;
            IpcChange?.Invoke(value);
        }
    }

    public void Register(OnIpcChange IpcChange)
    {
        if (this.IpcChange == null) this.IpcChange = IpcChange;
        this.IpcChange += IpcChange;
    }

    public void Deregister(OnIpcChange IpcChange)
    {
        this.IpcChange -= IpcChange;
    }
}
