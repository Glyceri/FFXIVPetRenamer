using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.KTKWindowing.Base;
using System;

namespace PetRenamer.PetNicknames.KTKWindowing.Helpers;

internal class GuideRegistration
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

    public required KTKComponent        CallbackComponent   { get; init; }

    public          Action?             OnSelected          { get; init; }
    public          Action?             OnUnselected        { get; init; }
}
