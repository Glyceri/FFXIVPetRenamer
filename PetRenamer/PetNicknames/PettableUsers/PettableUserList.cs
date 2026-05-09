using Dalamud.Utility;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;

namespace PetRenamer.PetNicknames.PettableUsers;

internal class PettableUserList : IPettableUserList
{
    public const int PettableUserArraySize = 101;
    public const int IslandIndex           = 100;

    public IPettableUser?[] PettableUsers 
        { get; } = new IPettableUser[PettableUserArraySize];

    public IPettableUser? LocalPlayer
        => PettableUsers[0];

    public IPettablePet? GetPet(nint pet)
    {
        if (pet == nint.Zero)
        {
            return null;
        }

        for (int i = 0; i < PettableUserArraySize; i++)
        {
            IPettableUser? user = PettableUsers[i];
            IPettablePet?  pPet = user?.GetPet(pet);

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
            IPettableUser? pUser = PettableUsers[i];
            IPettablePet?  pPet  = pUser?.GetPet(petId);

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
            IPettableUser? pUser = PettableUsers[i];

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
            IPettableUser? pUser = PettableUsers[i];

            if (pUser == null)
            {
                continue;
            }

            if (pUser.ObjectId != userId)
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
            IPettableUser? pUser = PettableUsers[i];

            if (pUser == null)
            {
                continue;
            }

            if (pUser.ShortObjectId != objectId)
            {
                continue;
            }

            return pUser;
        }

        return null;
    }

    public IPettableUser? GetUserFromContentId(ulong contentId)
    {
        if (contentId == 0)
        {
            return null;
        }

        for (int i = 0; i < PettableUserArraySize; i++)
        {
            IPettableUser? pUser = PettableUsers[i];

            if (pUser == null)
            {
                continue;
            }

            if (pUser.ContentId != contentId)
            {
                continue;
            }

            return pUser;
        }

        return null;
    }

    public IPettableUser? GetUserFromEntityId(uint entityId)
    {
        for (int i = 0; i < PettableUserArraySize; i++)
        {
            IPettableUser? pUser = PettableUsers[i];

            if (pUser == null)
            {
                continue;
            }

            if (pUser.EntityId != entityId)
            {
                continue;
            }

            return pUser;
        }

        return null;
    }
    
    public IPettableUser? GetUser(string username)
    {
        if (username.IsNullOrWhitespace())
        {
            return null;
        }

        for (int i = 0; i < PettableUserArraySize; i++)
        {
            IPettableUser? pUser = PettableUsers[i];

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

    public IPettableUser? GetUserFromOwnerId(uint ownerId)
    {
        if (ownerId == 0)
        {
            return null;
        }

        for (int i = 0; i < PettableUserArraySize; i++)
        {
            IPettableUser? pUser = PettableUsers[i];

            if (pUser == null)
            {
                continue;
            }

            if (pUser.ShortObjectId != ownerId)
            {
                continue;
            }

            return pUser;
        }

        return null;
    }
}
