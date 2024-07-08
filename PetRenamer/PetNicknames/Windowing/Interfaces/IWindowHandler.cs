using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Enums;
using System;

namespace PetRenamer.PetNicknames.Windowing.Interfaces;

internal interface IWindowHandler : IDisposable
{
    PetWindowMode PetWindowMode { get; set; }

    void Open<T>() where T : IPetWindow;
    void Close<T>() where T : IPetWindow;
    void Toggle<T>() where T : IPetWindow;
    T? GetWindow<T>() where T : PetWindow;

    void AddWindow(PetWindow window);
    void RemoveWindow(PetWindow window);
}
