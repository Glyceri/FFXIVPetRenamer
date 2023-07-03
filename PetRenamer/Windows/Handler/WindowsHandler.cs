using Dalamud.Interface.Windowing;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PetRenamer.Windows.Handler
{
    public class WindowsHandler
    {
        WindowSystem windowSystem = new WindowSystem("Pet Nicknames");
        public WindowSystem WindowSystem { get => windowSystem; }

        List<PetWindow> petWindows = new List<PetWindow>();

        public WindowsHandler()
        {
            PluginHandlers.PluginInterface.UiBuilder.Draw += windowSystem.Draw;
            AutoAddWindows();
        }

        ~WindowsHandler()
        {
            PluginHandlers.PluginInterface.UiBuilder.Draw -= windowSystem.Draw;
        }

        void AutoAddWindows()
        {
            Type petWindowType = typeof(PetWindow);
            Assembly petWindowAssembly = Assembly.GetAssembly(petWindowType)!;
            Type[] petWindowInheritedTypes = petWindowAssembly.GetTypes().Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.IsSubclassOf(typeof(PetWindow)) &&
                t.GetCustomAttribute<PersistentPetWindowAttribute>() != null)
            .ToArray();

            foreach (Type type in petWindowInheritedTypes)
                AddWindow(type);
        }

        public PetWindow GetWindow(Type windowType)
        {
            foreach (PetWindow window in petWindows)
                if (window.GetType() == windowType)
                    return window;

            return null!;
        }

        public PetWindow AddWindow(Type windowType)
        {
            PetWindow petWindow = GetWindow(windowType);
            if (petWindow != null) return petWindow;
            petWindow = (Activator.CreateInstance(windowType) as PetWindow)!;
            windowSystem.AddWindow(petWindow);
            petWindows.Add(petWindow);
            return petWindow;
        }

        public T GetWindow<T>() where T : PetWindow => (T)GetWindow(typeof(T));
        public T AddWindow<T>() where T : PetWindow => (T)AddWindow(typeof(T));

        public void RemoveAllWindows()
        {
            windowSystem.RemoveAllWindows();

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
