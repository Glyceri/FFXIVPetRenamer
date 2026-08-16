using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Services;

internal abstract class EnablableHandler : IEnablableHandler
{
    protected bool Enabled      { get; private set; } = true;
    private   bool _lastEnabled = false;
    
    public abstract void OnEnable();
    public abstract void OnDisable();
    public abstract void OnDispose();
    
    public void Dispose()
    {
        SetEnabled(EnabledState.Disabled);
        
        OnDispose();
    }
    
    public void SetEnabled(EnabledState enabled)
    {
        Enabled      = (enabled == EnabledState.Enabled);
        _lastEnabled = !Enabled;
        
        HandleEnabledState();
    }
    
    public void Enable()
    {
        SetEnabled(EnabledState.Enabled);
    }
    
    public void Disable()
    {
        SetEnabled(EnabledState.Disabled);
    }
    
    private void HandleEnabledState()
    {
        if (_lastEnabled == Enabled)
        {
            return;
        }
        
        _lastEnabled = Enabled;
        
        if (Enabled)
        {
            OnEnable();
        }
        else
        {
            OnDisable();
        }
    }
}