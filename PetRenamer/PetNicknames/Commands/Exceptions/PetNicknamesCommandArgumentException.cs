using System;

namespace PetRenamer.PetNicknames.Commands.Exceptions;

internal class PetNicknamesCommandArgumentException : Exception
{
    public PetNicknamesCommandArgumentException(string exception) : base(exception)
    {

    }
}
