using PetRenamer.PetNicknames.Windowing.Windows.PetListWindow.Enum;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetListWindow.Structs;

internal struct OffsetHelper
{
    readonly int Offset;
    readonly int OffsetPlusOne;
    int currentValidOffset = 0;

    public OffsetHelper(int currentIndex)
    {
        Offset = PetListWindow.ElementsPerList * currentIndex;
        OffsetPlusOne = PetListWindow.ElementsPerList * (currentIndex + 1);
    }

    public void IncrementValidOffset() => currentValidOffset++;

    public OffsetResult OffsetResult()
    {
        bool isEarly = currentValidOffset < Offset;
        bool isTechnicallyLate = currentValidOffset == OffsetPlusOne;
        bool isLate = currentValidOffset > OffsetPlusOne;

        if (isEarly || isTechnicallyLate)
        {
            return Enum.OffsetResult.Early;
        }

        if (isLate)
        {
            return Enum.OffsetResult.Late;
        }

        return Enum.OffsetResult.Valid;
    }
}
