using System;

namespace PetRenamer.Utilization.Exceptions
{
    public class UtilsNotFoundException : Exception
    {
        public new string Message => "Utils has not yet been instantiated!";

        public UtilsNotFoundException() { }
    }
}
