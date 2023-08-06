using Newtonsoft.Json;

namespace PetRenamer;

public class NicknameData
{
    public int ID = -1;
    public string? Nickname = string.Empty;
    public int BattleID = -1;
    public string? BattleNickname = string.Empty;

    public NicknameData() { }

    [JsonConstructor]
    public NicknameData(int ID, string? nickname, int BattleID, string? BattleNickname)
    {
        this.ID = ID;
        Nickname = nickname;
        this.BattleID = BattleID;
        this.BattleNickname = BattleNickname;
    }

    public new string ToString() => $"{ID}^{Nickname}^{BattleID}^{BattleNickname}";

    public bool Equals(NicknameData other) => ID == other.ID && Nickname == other.Nickname;
    public bool IDEquals(NicknameData other) => ID == other.ID;
}

