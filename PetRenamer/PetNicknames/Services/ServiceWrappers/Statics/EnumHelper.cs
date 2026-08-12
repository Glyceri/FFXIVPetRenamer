using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;

public static class EnumHelper
{
    /// <summary>
    /// Gets an attribute on an enum field value
    /// </summary>
    /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
    /// <param name="enumVal">The enum value</param>
    /// <returns>The attribute of type T that exists on the enum value</returns>
    /// <example><![CDATA[string desc = myEnumVariable.GetAttributeOfType<DescriptionAttribute>().Description;]]></example>
    public static T? GetAttributeOfType<T>(this Enum enumVal) 
        where T:System.Attribute
    {
        Type         type       = enumVal.GetType();
        MemberInfo[] memInfo    = type.GetMember(enumVal.ToString());
        object[]     attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
        
        return (attributes.Length > 0) ? (T)attributes[0] : null;
    }
    
    public static int GetEnumIndex<T>(this T value)
        where T : struct, Enum
        => Array.IndexOf(Enum.GetValues<T>(), value);

    public static string GetDescription(this Enum value)
    {
        DescriptionAttribute? attribute = value.GetType()?
            .GetField(value.ToString())?
            .GetCustomAttributes(typeof(DescriptionAttribute), false)?
            .SingleOrDefault() as DescriptionAttribute;

        return attribute == null ? string.Empty : attribute.Description;
    }
}