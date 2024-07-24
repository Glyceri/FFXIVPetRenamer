using System.ComponentModel;
using System.Linq;
using System;

namespace PetRenamer.PetNicknames.WritingAndParsing.Enums;

internal static class EnumHelper
{
    public static string GetDescription(this Enum value)
    {
        DescriptionAttribute? attribute = value.GetType()?
            .GetField(value.ToString())?
            .GetCustomAttributes(typeof(DescriptionAttribute), false)?
            .SingleOrDefault() as DescriptionAttribute;
        return attribute == null ? string.Empty : attribute.Description;
    }
}
