using System;

namespace PetRenamer.PetNicknames.KTKWindowing.Interfaces;

internal interface IHasSelectableCallbacks
{
    public Action? OnSelected
        { get; set; }

    public Action? OnUnselected
        { get; set; }
}
