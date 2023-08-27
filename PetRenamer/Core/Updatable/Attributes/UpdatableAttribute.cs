using System;

namespace PetRenamer.Windows.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class UpdatableAttribute : Attribute 
{
    public int order = 0;

    public UpdatableAttribute(int order = 0) 
    { 
        this.order = order;
    }
}