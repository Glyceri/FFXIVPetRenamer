using Dalamud.Utility;
using System;
using System.Globalization;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;

public static class StringHelper
{
    public static string ToTitleCase(this string baseString)
        => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(baseString.ToLower());

    public static string CleanString(this string baseString, string toBeCleaned)
        => baseString.Replace(toBeCleaned, string.Empty, StringComparison.InvariantCultureIgnoreCase);

    public static string CleanString(this string baseString, string[] toBeCleaned)
    {
        foreach (string item in toBeCleaned)
        {
            baseString = baseString.CleanString(item);
        }

        return baseString;
    }

    public static bool InvariantEquals(this string baseString, string toCompare)
    {
        if (baseString.IsNullOrWhitespace())
        {
            return false;
        }

        if (toCompare.IsNullOrWhitespace())
        {
            return false;
        }

        return string.Equals(baseString, toCompare, StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool InvariantContains(this string baseString, string shouldContain)
        => baseString.Contains(shouldContain, StringComparison.InvariantCultureIgnoreCase);

    public static bool InvariantContains(this string baseString, string[] shouldContain)
    {
        for (int i = 0; i < shouldContain.Length; i++)
        {
            if (!baseString.Contains(shouldContain[i], StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    public static string InvariantReplace(this string baseString, string marker, string toReplace)
        => baseString.Replace(marker, toReplace, StringComparison.InvariantCultureIgnoreCase);
}
