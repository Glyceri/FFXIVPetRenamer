using PetRenamer.Core.Attributes;
using System;

namespace PetRenamer.Core.Chat.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ChatAttribute : SortableAttribute 
{
    public ChatAttribute() { }
    public ChatAttribute(int order) : base(order) { }
}