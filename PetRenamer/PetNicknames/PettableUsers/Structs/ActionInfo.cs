using Dalamud.Game;
using System;

namespace PetRenamer.PetNicknames.PettableUsers.Structs;

internal readonly struct ActionData : IEquatable<ActionData>
{
    public readonly uint       ActionId;
    public readonly ActionKind ActionKind;
    
    public ActionData(uint actionId, ActionKind actionKind)
    {
        ActionId   = actionId;
        ActionKind = actionKind;
    }

    public override string ToString()
        => $"Action: [{ActionId}, {ActionKind}]";

    public static bool operator ==(ActionData left, ActionData right)
        => right.ActionId == left.ActionId && right.ActionKind == left.ActionKind;

    public static bool operator !=(ActionData left, ActionData right)
        => right.ActionId != left.ActionId || right.ActionKind != left.ActionKind;
    
    public bool Equals(ActionData other) 
        => this == other;
    
    public override bool Equals(object? obj)
        => obj is ActionData other && Equals(other);
    
    public override int GetHashCode() 
        => HashCode.Combine(ActionId, (int)ActionKind);
}