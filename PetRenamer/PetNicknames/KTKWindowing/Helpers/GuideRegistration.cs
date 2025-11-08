using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.KTKWindowing.Interfaces;
using System;

namespace PetRenamer.PetNicknames.KTKWindowing.Helpers;

internal class GuideRegistration : IHasSelectableCallbacks
{
    public          byte                LowerGuideId        { get; init; } = 2;

    public required uint                LeftGuideId         { get; init; }
    public required OperationGuidePoint LeftPoint           { get; init; }
    public required OperationGuidePoint LeftRelativePoint   { get; init; }
    public required short               LeftOffsetX         { get; set; }
    public required short               LeftOffsetY         { get; set; }

    public required uint                RightGuideId        { get; init; }
    public required OperationGuidePoint RightPoint          { get; init; }
    public required OperationGuidePoint RightRelativePoint  { get; init; }
    public required short               RightOffsetX        { get; set; }
    public required short               RightOffsetY        { get; set; }

    public required ICustomInput        CallbackComponent   { get; init; }

    public          Action?             OnSelected          { get; set; }
    public          Action?             OnUnselected        { get; set; }
}
