using ImGuiNET;
using System;

namespace PetRenamer.Windows;

public abstract class TemporaryPetWindow : PetWindow, IDisposable
{
    Action<object> callback { get; set; } = null!;
    public bool closed { get; private set; } = false;

    protected TemporaryPetWindow(string name, Action<object> callback, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false) : base(name, flags, forceMainWindow)
    {
        this.callback = callback; 
    }

    public void DoCallback(object data)
    {
        callback?.Invoke(data);
        Close();
    }
    public new void Dispose() { closed = true; OnDispose(); }
    public void Close() => Dispose();
    protected override void OnDispose() { }

    public unsafe override void OnDraw() { }
}
