using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.PettableUsers;

internal class PettableUserList : IPettableUserList
{
    public const int PettableUserArraySize = 101;
    public const int IslandIndex = 100;

    private readonly IPettableUser?[]     Users = new IPettableUser[PettableUserArraySize];
    private readonly IPettableDirtyCaller DirtyCaller;

    public PettableUserList(IPettableDirtyCaller dirtyCaller)
    {
        DirtyCaller = dirtyCaller;
    }

    public IReadOnlyList<IPettableUser?> PettableUsers
        => Users;

    public IPettableUser? LocalPlayer
        => Users[0];

    public IPettablePet? GetPet(nint pet)
    {
        if (pet == nint.Zero)
        {
            return null;
        }

        for (int i = 0; i < PettableUserArraySize; i++)
        {
            IPettableUser? user = Users[i];

            if (user == null)
            {
                continue;
            }

            IPettablePet? pPet = user.GetPet(pet);

            if (pPet == null)
            {
                continue;
            }

            return pPet;
        }

        return null;
    }

    public IPettablePet? GetPet(ulong petId)
    {
        if (petId == 0)
        {
            return null;
        }

        for (int i = 0; i < PettableUserArraySize; i++)
        {
            IPettableUser? pUser = Users[i];

            if (pUser == null)
            {
                continue;
            }

            IPettablePet? pPet = pUser.GetPet(petId);

            if (pPet == null)
            {
                continue;
            }

            return pPet;
        }
        return null;
    }

    public IPettableUser? GetUser(nint user, bool petMeansOwner = true)
    {
        if (user == nint.Zero)
        {
            return null;
        }

        for (int i = 0; i < PettableUserArraySize; i++)
        {
            IPettableUser? pUser = Users[i];

            if (pUser == null)
            {
                continue;
            }

            if (pUser.Address == user)
            {
                return pUser;
            }

            if (petMeansOwner)
            {
                if (pUser.GetPet(user) == null)
                {
                    continue;
                }

                return pUser;
            }
        }

        return null;
    }

    public IPettableUser? GetUser(ulong userId)
    {
        if (userId == 0)
        {
            return null;
        }

        for (int i = 0; i < PettableUserArraySize; i++)
        {
            IPettableUser? pUser = Users[i];

            if (pUser == null)
            {
                continue;
            }

            if (pUser.ObjectID != userId)
            {
                continue;
            }

            if (pUser.GetPet(userId) == null)
            {
                continue;
            }

            return pUser;
        }
        return null;
    }

    public IPettableUser? GetUserFromObjectId(uint objectId)
    {
        if (objectId == 0)
        {
            return null;
        }

        for (int i = 0; i < PettableUserArraySize; i++)
        {
            IPettableUser? pUser = Users[i];

            if (pUser == null)
            {
                continue;
            }

            if (pUser.ShortObjectID != objectId)
            {
                continue;
            }

            return pUser;
        }

        return null;
    }

    public IPettableUser? GetUserFromContentID(ulong contentID)
    {
        if (contentID == 0)
        {
            return null;
        }

        for (int i = 0; i < PettableUserArraySize; i++)
        {
            IPettableUser? pUser = Users[i];

            if (pUser == null)
            {
                continue;
            }

            if (pUser.ContentID != contentID)
            {
                continue;
            }

            return pUser;
        }

        return null;
    }

    public IPettableUser? GetUser(string username)
    {
        if (username == null || username == string.Empty)
        {
            return null;
        }

        for (int i = 0; i < PettableUserArraySize; i++)
        {
            IPettableUser? pUser = Users[i];

            if (pUser == null)
            {
                continue;
            }

            if (!pUser.Name.InvariantEquals(username))
            {
                continue;
            }

            return pUser;
        }

        return null;
    }

    public IPettableUser? GetUserFromOwnerID(uint ownerID)
    {
        if (ownerID == 0)
        {
            return null;
        }

        for (int i = 0; i < PettableUserArraySize; i++)
        {
            IPettableUser? pUser = Users[i];

            if (pUser == null)
            {
                continue;
            }

            if (pUser.ShortObjectID != ownerID)
            {
                continue;
            }

            return pUser;
        }

        return null;
    }

    public void SetUser(int index, in IPettableUser? user)
    {
        if (Users[index] != null)
        {
            Users[index]?.Dispose();
            Users[index] = null;
        }

        Users[index] = user;

        DirtyCaller.DirtyUserList(this);
    }
}
