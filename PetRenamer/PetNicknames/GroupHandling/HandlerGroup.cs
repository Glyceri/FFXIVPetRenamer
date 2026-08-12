using PetRenamer.PetNicknames.GroupHandling.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.GroupHandling;

internal abstract class HandlerGroup : IHandlerGroup
{
    private readonly IEnablableHandler Handler1;
    private readonly IEnablableHandler Handler2;
    private readonly IDirtyListener    DirtyListener;
    
    /// <summary>
    /// Makes a handler group out of the two handlers.
    /// Keep in mind this takes OWNERSHIP of the handlers, so if you dispose this group, it disposes the handlers.
    /// </summary>
    /// <param name="handler1">The handler enabled on high.</param>
    /// <param name="handler2">The handler enabled on low.</param>
    /// <param name="dirtyListener">A dirtyListener ref from PetServices.</param>
    public HandlerGroup(IEnablableHandler handler1, IEnablableHandler handler2, IDirtyListener dirtyListener)
    {
        DirtyListener   = dirtyListener;
        Handler1        = handler1;
        Handler2        = handler2;
        
        DirtyListener.RegisterOnDirtyConfig(OnDirtyConfig);
    }
    
    public void Dispose()
    {
        DirtyListener.UnregisterOnDirtyConfig(OnDirtyConfig);
        
        Handler1.Dispose();
        Handler2.Dispose();
    }
    
    public abstract ref Configuration.GroupConfig GetGroupConfig(Configuration configuration);

    public void SetGroupState(ref Configuration.GroupConfig groupConfig)
    {
        if (groupConfig.High)
        {
            Handler1.Enable();
            Handler2.Disable();
        }
        else
        {
            Handler1.Disable();
            Handler2.Enable();
        }
    }
    
    private void OnDirtyConfig(Configuration configuration) 
        => SetGroupState(ref GetGroupConfig(configuration)); 
}