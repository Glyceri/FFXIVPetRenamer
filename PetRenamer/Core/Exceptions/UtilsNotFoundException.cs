using System;

namespace PetRenamer.Core.Exceptions
{
    public class UtilsNotFoundException : Exception
    {
        public new string Message => "Utils has not yet been instantiated!";

        public UtilsNotFoundException() { }
    }
}
