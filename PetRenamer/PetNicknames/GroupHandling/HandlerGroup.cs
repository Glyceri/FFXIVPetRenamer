using PetRenamer.PetNicknames.GroupHandling.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;

namespace PetRenamer.PetNicknames.GroupHandling;

internal abstract class HandlerGroup : IHandlerGroup
{
    private readonly string                TitleTranslatorKey;
    private readonly EnablableRegistration Handler1;
    private readonly EnablableRegistration Handler2;
    private readonly IDirtyListener        DirtyListener;
    
    /// <summary>
    /// Makes a handler group out of the two handlers.
    /// Keep in mind this takes OWNERSHIP of the handlers, so if you dispose this group, it disposes the handlers.
    /// </summary>
    /// <param name="titleTranslatorKey">The key for the translator to display on the config.</param>
    /// <param name="handler1">The handler enabled on high.</param>
    /// <param name="handler2">The handler enabled on low.</param>
    /// <param name="dirtyListener">A dirtyListener ref from PetServices.</param>
    public HandlerGroup(string titleTranslatorKey, EnablableRegistration handler1, EnablableRegistration handler2, IDirtyListener dirtyListener)
    {
        TitleTranslatorKey  = $"Enablable.{titleTranslatorKey}.Title";
        DirtyListener       = dirtyListener;
        Handler1            = handler1;
        Handler2            = handler2;
        
        DirtyListener.RegisterOnDirtyConfig(OnDirtyConfig);
    }
    
    public void Dispose()
    {
        DirtyListener.UnregisterOnDirtyConfig(OnDirtyConfig);
        
        Handler1.EnablableHandler.Dispose();
        Handler2.EnablableHandler.Dispose();
    }
    
    public abstract ref Configuration.GroupConfig GetGroupConfig(Configuration configuration);
    
    public void SetGroupState(ref Configuration.GroupConfig groupConfig)
    {
        if (groupConfig.High)
        {
            Handler1.EnablableHandler.Enable();
            Handler2.EnablableHandler.Disable();
        }
        else
        {
            Handler1.EnablableHandler.Disable();
            Handler2.EnablableHandler.Enable();
        }
    }
    
    private EnablableRegistration GetRegistration(EnabledState enabledState)
    {
        if (enabledState == EnabledState.Enabled)
        {
            return Handler1;
        }
        
        return Handler2;
    }
    
    public string GetHandlerTitle()
        => TitleTranslatorKey;
    
    public string GetTitle(EnabledState enabledState) 
        => GetRegistration(enabledState).TitleTranslatorKey;
    
    public string GetDescription(EnabledState enabledState)
        => GetRegistration(enabledState).DescriptionTranslatorKey;
    
    private void OnDirtyConfig(Configuration configuration) 
        => SetGroupState(ref GetGroupConfig(configuration)); 
}