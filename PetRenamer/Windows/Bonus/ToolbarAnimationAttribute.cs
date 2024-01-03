using PetRenamer.Core.Attributes;
using System;

namespace PetRenamer.Windows.Bonus;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal class ToolbarAnimationAttribute : SortableAttribute
{
    string s_identifier;
    public string Identifier => s_identifier;

    public ToolbarAnimationAttribute(string identifier, int order = 0) : base(order)
    {
        s_identifier = identifier;
    }
}
