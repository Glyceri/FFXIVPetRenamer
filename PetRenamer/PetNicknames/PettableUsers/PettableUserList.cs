using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.PettableUsers;

internal class PettableUserList : IPettableUserList
{
    const int PettableUserArraySize = 101;
    public const int IslandIndex = 100;

    public IPettableUser?[] PettableUsers { get; set; } = new IPettableUser[PettableUserArraySize];
    public IPettableUser? LocalPlayer { get => PettableUsers[0]; }

    public IPettablePet? GetPet(nint pet)
    {
        if (pet == nint.Zero) return null;
        for (int i = 0; i < PettableUserArraySize; i++)
        {
            IPettableUser? user = PettableUsers[i];
            if(user == null) continue;
            if (!user.IsActive) continue;
            IPettablePet? pPet = user.GetPet(pet);
            if (pPet == null) continue;
            return pPet;
        }
        return null;
    }

    public IPettablePet? GetPet(ulong petId)
    {
        if (petId == 0) return null;

        for (int i = 0; i < PettableUserArraySize; i++)
        {
            IPettableUser? pUser = PettableUsers[i];
            if (pUser == null) continue;
            if (!pUser.IsActive) continue;

            IPettablePet? pPet = pUser.GetPet(petId);
            if (pPet == null) continue;

            return pPet;
        }
        return null;
    }

    public IPettableUser? GetUser(nint user)
    {
        if (user == nint.Zero) return null;

        for (int i = 0; i < PettableUserArraySize; i++)
        {
            IPettableUser? pUser = PettableUsers[i];
            if (pUser == null) continue;
            if (!pUser.IsActive) continue;
            if (pUser.Address == user) return pUser;
            if (pUser.GetPet(user) == null) continue;

            return pUser;
        }
        return null;
    }

    public IPettableUser? GetUser(ulong userId)
    {
        if (userId == 0) return null;

        for (int i = 0; i < PettableUserArraySize; i++)
        {
            IPettableUser? pUser = PettableUsers[i];
            if (pUser == null) continue;
            if (!pUser.IsActive) continue;
            if (pUser.ObjectID == userId) return pUser;
            if (pUser.GetPet(userId) == null) continue;

            return pUser;
        }
        return null;
    }

    public IPettableUser? GetUserFromContentID(ulong contentID)
    {
        if (contentID == 0) return null;

        for (int i = 0; i < PettableUserArraySize; i++)
        {
            IPettableUser? pUser = PettableUsers[i];
            if (pUser == null) continue;
            if (!pUser.IsActive) continue;
            if (pUser.ContentID != contentID) continue;

            return pUser;
        }
        return null;
    }

    public IPettableUser? GetUser(string username)
    {
        if (username == null || username == string.Empty) return null;

        for (int i = 0; i < PettableUserArraySize; i++)
        {
            IPettableUser? pUser = PettableUsers[i];
            if (pUser == null) continue;
            if (!pUser.IsActive) continue;
            if (!string.Equals(pUser.Name, username, System.StringComparison.InvariantCultureIgnoreCase)) continue;
            return pUser;
        }
        return null;
    }
}
