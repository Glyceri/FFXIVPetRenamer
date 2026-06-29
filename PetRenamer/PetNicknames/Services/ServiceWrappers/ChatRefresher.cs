using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class ChatRefresher : IChatRefresher
{
    private const byte AMOUNT_OF_TABS_IN_CHAT = 4;
    
    private readonly IDirtyListener DirtyListener;
    
    public ChatRefresher(IDirtyListener dirtyListener)
    {
        DirtyListener = dirtyListener;
        
        dirtyListener.RegisterOnDirtyConfig(OnDirtyConfig);
    }
    
    public void Dispose()
        => RefreshChat();
    
    private void OnDirtyConfig(Configuration _)
        => RefreshChat();
    
    public unsafe void RefreshChat()
    {
        RaptureLogModule* raptureLogModule = RaptureLogModule.Instance();
        
        if (raptureLogModule == null)
        {
            return;
        }
        
        for (byte i = 0; i < AMOUNT_OF_TABS_IN_CHAT; i++)
        {
            raptureLogModule->ChatTabIsPendingReload[i] = true;
        }
    }
}