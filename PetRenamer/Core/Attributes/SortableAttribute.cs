using System;

namespace PetRenamer.Core.Attributes;

public class SortableAttribute : Attribute
{
    int i_order;
    public int Order => i_order;

    public SortableAttribute(int order = 0)
    {
        i_order = order;
    }
}
