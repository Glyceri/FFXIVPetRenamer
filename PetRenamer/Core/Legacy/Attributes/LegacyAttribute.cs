using System;
using System.Collections.Generic;

namespace PetRenamer.Core.Legacy.Attributes;

internal class LegacyAttribute : Attribute
{
    public int[] forVersions;

    public LegacyAttribute(int[] forVersions)
    {
        this.forVersions = forVersions;
    }
}
