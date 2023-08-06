using Newtonsoft.Json;

namespace PetRenamer;

public class NicknameData
{
    public int ID = -1;
    public string? Nickname = string.Empty;

    [JsonConstructor]
    public NicknameData(int iD, string? nickname)
    {
        ID = iD;
        Nickname = nickname;
    }

    public bool Equals(NicknameData other) => ID == other.ID && Nickname == other.Nickname;
    public bool IDEquals(NicknameData other) => ID == other.ID;
}

