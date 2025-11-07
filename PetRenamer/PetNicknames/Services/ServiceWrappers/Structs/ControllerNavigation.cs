namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

internal readonly struct ControllerNavigation
{
    public ControllerNavigation()
        { }

    public required byte Index
        { get; init; } = 0;

    public byte LeftIndex
        { get; init; } = 0;

    public byte RightIndex
        { get; init; } = 0;

    public byte UpIndex
        { get; init; } = 0;

    public byte DownIndex
        { get; init; } = 0;

    public bool LeftStop
        { get; init; } = false;

    public bool RightStop
        { get; init; } = false;

    public bool UpStop
        { get; init; } = false;

    public bool DownStop
        { get; init; } = false;
}
