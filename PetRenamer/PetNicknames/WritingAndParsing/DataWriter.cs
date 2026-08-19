using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing.Enums;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces;
using PetRenamer.PetNicknames.WritingAndParsing.WriterElements;

namespace PetRenamer.PetNicknames.WritingAndParsing;

internal class DataWriter : IDataWriter
{
    private readonly IDataWriterElement[] DataWriters =
    [
        new WriterElementVersion2(),
        new WriterElementVersion3(),
        new WriterElementVersion4(),
    ];
    
    public string? WriteData(IPettableUser forUser, ParseVersion forVersion = ParseVersion.COUNT - 1)
    {
        foreach (IDataWriterElement dataWriterElement in DataWriters)
        {
            if (dataWriterElement.WriteVersion != forVersion)
            {
                continue;
            }
            
            return dataWriterElement.WriteData(forUser);
        }
        
        return null;
    }
}