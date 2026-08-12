using System;

namespace PetRenamer.PetNicknames.Windowing.Interfaces;

internal interface IWindowHandler : IDisposable
{
    void Open<T>() 
        where T : IPetWindow;

    void Close<T>()
        where T : IPetWindow;

    void Toggle<T>()
        where T : IPetWindow;

    T? GetWindow<T>() 
        where T : IPetWindow;
    
    IPetWindow? FocussedWindow { get; }
}
