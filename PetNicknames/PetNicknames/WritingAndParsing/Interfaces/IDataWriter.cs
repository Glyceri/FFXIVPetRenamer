using PetRenamer.PetNicknames.ColourProfiling.Interfaces;

namespace PetRenamer.PetNicknames.ReadingAndParsing.Interfaces;

internal interface IDataWriter
{
    string WriteData();
    string WriteColourData(in IColourProfile colourProfile);
}
