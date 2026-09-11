#pragma warning disable CS9113
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using System.CodeDom.Compiler;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Sheets;

[GeneratedCode("Lumina.Excel.Generator", "2.0.0")]
[Sheet("XBMPet", 0xED3C4185)]
public readonly unsafe struct XBMPetButActuallyWorkingSinceForSomeReasonWePutInUnsusedSheetsAndNowEverythingIsABreakingChangeBecauseWhyWouldntItBeLikeWhatAreWeGenuinelyDoingHere(ExcelPage page, uint offset, uint row)
    : IExcelRow<XBMPetButActuallyWorkingSinceForSomeReasonWePutInUnsusedSheetsAndNowEverythingIsABreakingChangeBecauseWhyWouldntItBeLikeWhatAreWeGenuinelyDoingHere>
{
    public ExcelPage ExcelPage 
        => page;
    
    public uint RowOffset 
        => offset;
    
    public uint RowId
        => row;

    public readonly ReadOnlySeString Description 
        => page.ReadString(offset, offset);
    
    public readonly ReadOnlySeString TrickDescription 
        => page.ReadString(offset + 4, offset);
    
    public readonly ReadOnlySeString TemperedDescription
        => page.ReadString(offset + 8, offset);
    
    public readonly uint Icon 
        => page.ReadUInt32(offset + 12);
    
    public readonly RowRef<Pet> Pet 
        => new(page.Module, (uint)page.ReadInt32(offset + 16), page.Language);
    
    public readonly RowRef<Action> Action
        => new(page.Module, (uint)page.ReadUInt16(offset + 20), page.Language);
    
    public readonly RowRef Location
        => (/* LocationKey */ page.ReadUInt8(offset + 27)) switch
        {
            1 => RowRef.Create<PlaceName>(page.Module, (uint)page.ReadUInt16(offset + 22), page.Language),
            2 => RowRef.Create<ContentFinderCondition>(page.Module, (uint)page.ReadUInt16(offset + 22), page.Language),
            _ => RowRef.CreateUntyped((uint)page.ReadUInt16(offset + 22), page.Language),
        };
    
    /// <summary>
    /// Lookup from Addon#R17741 through Addon#R17748 through array in exe .rdata section
    /// </summary>
    public readonly byte Classification 
        => page.ReadUInt8(offset + 24);
    
    /// <summary>
    /// Main RowId for XBMPetParamGrow where subrow is Rank - 1
    /// </summary>
    public readonly byte ParamGrow
        => page.ReadUInt8(offset + 25);
    
    public readonly byte SatietyMax 
        => page.ReadUInt8(offset + 26);
    
    public readonly byte LocationKey
        => page.ReadUInt8(offset + 27);
    
    public readonly Collection<byte> Unknown12 
        => new(page, offset, offset, &Unknown12Ctor, 5);
    
    public readonly Collection<bool> InflictsStatus 
        => new(page, offset, offset, &InflictsStatusCtor, 11);
    
    private static byte Unknown12Ctor(ExcelPage page, uint parentOffset, uint offset, uint i) 
        => page.ReadUInt8(offset + 28 + i);
    
    private static bool InflictsStatusCtor(ExcelPage page, uint parentOffset, uint offset, uint i)
        => page.ReadBool(offset + 33 + i);

    static XBMPetButActuallyWorkingSinceForSomeReasonWePutInUnsusedSheetsAndNowEverythingIsABreakingChangeBecauseWhyWouldntItBeLikeWhatAreWeGenuinelyDoingHere IExcelRow<XBMPetButActuallyWorkingSinceForSomeReasonWePutInUnsusedSheetsAndNowEverythingIsABreakingChangeBecauseWhyWouldntItBeLikeWhatAreWeGenuinelyDoingHere>.Create(ExcelPage page, uint offset, uint row) =>
        new(page, offset, row);
}