namespace PetRenamer.PetNicknames.KTKWindowing.ControllerNavigation.Struct;

internal readonly struct CursorNavigationInfo
{
    public readonly byte Index;
    public readonly byte UpIndex;
    public readonly byte DownIndex;
    public readonly byte LeftIndex;
    public readonly byte RightIndex;
    public readonly bool UpStop;
    public readonly bool DownStop;
    public readonly bool LeftStop;
    public readonly bool RightStop;

    public CursorNavigationInfo(byte index, byte upIndex, byte downIndex, byte leftIndex, byte rightIndex, bool upStop, bool downStop, bool leftStop, bool rightStop)
    {
        Index      = index;
        UpIndex    = upIndex;
        DownIndex  = downIndex;
        LeftIndex  = leftIndex;
        RightIndex = rightIndex;
        UpStop     = upStop;
        DownStop   = downStop;
        LeftStop   = leftStop;
        RightStop  = rightStop;
    }
}
