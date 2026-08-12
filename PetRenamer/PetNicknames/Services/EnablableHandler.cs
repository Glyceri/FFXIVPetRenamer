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
        SetEnabled(false);
        
        OnDispose();
    }
    
    public void SetEnabled(bool enabled)
    {
        Enabled      = enabled;
        _lastEnabled = !Enabled;
        
        HandleEnabledState();
    }
    
    public void Enable()
    {
        SetEnabled(true);
    }
    
    public void Disable()
    {
        SetEnabled(false);
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