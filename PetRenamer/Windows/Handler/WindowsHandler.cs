using Dalamud.Interface.Windowing;
using System;
using System.Collections.Generic;

namespace PetRenamer.Windows.Handler
{
    public class WindowsHandler
    {
        WindowSystem _WindowSystem = new WindowSystem("Pet Nicknames");
        public WindowSystem WindowSystem { get => _WindowSystem; }

        List<PetWindow> petWindows = new List<PetWindow>();

        public T GetWindow<T>() where T : PetWindow
        {
            foreach(PetWindow window in petWindows)
                if (window.GetType() == typeof(T))
                    return (T)window;

            return null!;
        }

        public T AddWindow<T>() where T : PetWindow
        {
            T petWindow = GetWindow<T>();
            if (petWindow != null) return petWindow;
            petWindow = (Activator.CreateInstance(typeof(T)) as T)!;

            _WindowSystem.AddWindow(petWindow);
            petWindows.Add(petWindow);
            return petWindow;
        }

        public void RemoveAllWindows()
        {
            _WindowSystem.RemoveAllWindows();

            foreach (PetWindow window in petWindows)
                window?.Dispose();

            petWindows.Clear();
        }

        public void CloseAllWindows()
        {
            foreach (PetWindow window in petWindows)
                window.IsOpen = false;
        }

       
    }
}
