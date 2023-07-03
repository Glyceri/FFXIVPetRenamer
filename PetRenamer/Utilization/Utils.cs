using PetRenamer.Utilization.Exceptions;
using System;
using System.Collections.Generic;

namespace PetRenamer.Utilization
{
    public class Utils
    {
        internal Dictionary<Type, UtilsRegistryType> typeRegistry = new Dictionary<Type, UtilsRegistryType>();

        internal T Get<T>() where T : UtilsRegistryType
        {
            if (!typeRegistry.ContainsKey(typeof(T))) return Create<T>();
            return (T)typeRegistry[typeof(T)];
        }

        private T Create<T>() where T : UtilsRegistryType
        {
            T newT = Activator.CreateInstance<T>();
            if (newT == null) throw new UtilsNotFoundException();
            typeRegistry.Add(typeof(T), newT);
            newT.OnRegistered();
            return newT;
        }      
    }
}
