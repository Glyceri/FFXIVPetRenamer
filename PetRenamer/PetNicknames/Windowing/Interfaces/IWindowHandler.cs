using PetRenamer.PetNicknames.Windowing.Base;
using System;

namespace PetRenamer.PetNicknames.Windowing.Interfaces;

internal interface IWindowHandler : IDisposable
{
    void Open<T>() where T : IPetWindow;
    void Close<T>() where T : IPetWindow;
    void Toggle<T>() where T : IPetWindow;

    void AddWindow(PetWindow window);
    void RemoveWindow(PetWindow window);
}
