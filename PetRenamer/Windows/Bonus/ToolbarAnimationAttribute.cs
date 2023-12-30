using System;

namespace PetRenamer.Windows.Bonus;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal class ToolbarAnimationAttribute : Attribute
{
    string s_identifier;
    int i_order;
    public string Identifier => s_identifier;
    public int Order => i_order;
    public ToolbarAnimationAttribute(string identifier, int order = 0)
    {
        i_order = order;
        s_identifier = identifier;
    }
}
