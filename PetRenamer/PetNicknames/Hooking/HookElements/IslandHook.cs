using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.Network;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class IslandHook : HookableElement
{
    private const uint ISLAND_TERRITORY_ID = 1055;
    
    private readonly Hook<PacketDispatcher.Delegates.SendEventCompletePacket> SendEventCompletePacketHook;
    
    private readonly IPettableDatabase Database;
    
    public IslandHook(DalamudServices services, IPetServices petServices, IPettableDatabase database) 
        : base(services, petServices)
    {
        SendEventCompletePacketHook = DalamudServices.Hooking.HookFromAddress<PacketDispatcher.Delegates.SendEventCompletePacket>((nint)PacketDispatcher.Addresses.SendEventCompletePacket.Value, SendEventCompletePacketDetour);
        Database                    = database;
    }

    public override void Init()
    { 
        DalamudServices.ClientState.TerritoryChanged += OnTerritoryChanged;
        
        SendEventCompletePacketHook?.Enable();
        
        HandleForContentId(PetServices.Configuration.LastIslandContentId);
        
        OnTerritoryChanged(DalamudServices.ClientState.TerritoryType);
    }
    
    protected override void OnDispose()
    {
        DalamudServices.ClientState.TerritoryChanged -= OnTerritoryChanged;
        
        ClearIslandUser();
        
        SendEventCompletePacketHook?.Dispose();
    }
    
    private void OnTerritoryChanged(uint territory)
    {
        if (PetServices.Configuration.debugModeActive)
        {
            PetServices.PetLog.Log($"Pet Nicknames detected a zone change: {territory}");
        }
        
        if (territory == ISLAND_TERRITORY_ID)
        {
            HandleForContentId(PetServices.Configuration.LastIslandContentId);
        }
        else
        {
            ClearIslandUser();
        }
    }
    
    private void SendEventCompletePacketDetour(EventId eventId, short scene, byte a3, uint* payload, byte payloadSize, void* a6)
    {
        SendEventCompletePacketHook.Original(eventId, scene, a3, payload, payloadSize, a6);
        
        if (eventId.EntryId != 798)
        {
            return;
        }
        
        if (eventId.ContentId != EventHandlerContent.CustomTalk)
        {
            return;
        }
        
        DecodeIfLocal(scene, payload, payloadSize);
        DecodeIfOther(scene, payload, payloadSize);
    }
    
    private void DecodeIfLocal(short scene, uint* payload, byte payloadSize)
    {
        if (scene != 0 && scene != 10)
        {
            return;
        }
        
        if (payloadSize != 1)
        {
            return;
        }
        
        if (payload[0] != 2)
        { 
            return;
        }
        
        HandleForContentId(PlayerState.Instance()->ContentId);
    }
    
    private void DecodeIfOther(short scene, uint* payload, byte payloadSize)
    {
        if (scene != 20)
        {
            return;
        }
        
        if (payloadSize != 3)
        {
            return;
        }
        
        if (payload[0] != 4)
        {
            return;
        }
        
        ulong contentIdPart1 = (ulong)payload[1] << 32;
        ulong contentIdPart2 = (ulong)payload[2];
        ulong contentId      = contentIdPart1 | contentIdPart2;

        HandleForContentId(contentId);
    }
    
    private void HandleForContentId(ulong contentId)
    {
        PetServices.PetLog.Log("Handling island for contentId: " + contentId);
        
        SetupIslandUser(Database.GetEntry(contentId));
        
        if (PetServices.Configuration.LastIslandContentId == contentId)
        {
            return;
        }
        
        PetServices.Configuration.LastIslandContentId = contentId;
        PetServices.Configuration.Save();
    }
    
    private void SetupIslandUser(IPettableDatabaseEntry? entry)
    {
        ClearIslandUser();
        
        if (entry == null)
        {
            return;
        }
        
        PetServices.PetLog.LogVerbose("Island owner found: " +  entry.Name);
        
        PetServices.UserList[IUserList.IslandIndex] = new PettableIslandUser(PetServices, entry);
    }
    
    private void ClearIslandUser()
    {
        PetServices.UserList[IUserList.IslandIndex]?.Dispose(Database);
        PetServices.UserList[IUserList.IslandIndex] = null;
    }
}
