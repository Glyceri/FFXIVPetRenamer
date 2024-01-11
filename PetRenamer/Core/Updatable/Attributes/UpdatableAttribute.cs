using PetRenamer.Core.Attributes;
using System;

namespace PetRenamer.Windows.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class UpdatableAttribute : SortableAttribute
{
    public UpdatableAttribute(int order = 0) : base(order) { }
}