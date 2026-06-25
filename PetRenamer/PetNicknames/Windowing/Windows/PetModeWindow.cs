using Dalamud.Bindings.ImGui;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Components.Header;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows;

internal class PetModeWindow : PetWindow
{
    private IPetWindow? attachedTo = null;
    
    public PetModeWindow(WindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices) 
        : base(windowHandler, dalamudServices, petServices, "PetModeWindow", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoNav)
    {
        DisableFadeInFadeOut = true;
        DisableWindowSounds  = true;
        AllowPinning         = false;
        AllowBackgroundBlur  = true;
        AllowClickthrough    = false;
        RespectCloseHotkey   = false;
        IsTopMost            = true;
        IsOpen               = true;
    }
    
    private static readonly Vector2 WindowSize = new Vector2(1000, 32);
    
    protected override Vector2 MinSize       
        => WindowSize;
    
    protected override Vector2 MaxSize     
        => WindowSize;
    
    protected override Vector2 DefaultSize
        => WindowSize; 
    
    public override bool HasModeToggle
        => true;
    
    public override bool ShowQuickButtons
        => false;
    
    public override void Update()
    {
        if (PetServices.Configuration.oldBarStyleLayout)
        {
            return;
        }

        if (WindowHandler.FocussedWindow is null)
        {
            return;
        }
        
        if (WindowHandler.FocussedWindow == this)
        {
            return;
        }
        
        attachedTo = WindowHandler.FocussedWindow;
        
        ImGui.SetNextWindowPos(attachedTo.CurrentPosition - new Vector2(0, WindowSize.Y) * WindowHandler.GlobalScale, ImGuiCond.Always);
    }

    private Vector2 _endPos;
    private Vector2 _startPos;
    
    protected override void OnDraw()
    {
        if (PetServices.Configuration.oldBarStyleLayout)
        {
            return;
        }
        
        _startPos = ImGui.GetCursorScreenPos();
        
        DrawModeButton(SkeletonType.Minion,      PluginConstants.MinionColourHover, PluginConstants.MinionColourIdle, PluginConstants.MinionColourClick);
        DrawModeButton(SkeletonType.BattlePet,   PluginConstants.BattlePetHover,    PluginConstants.BattlePetIdle,    PluginConstants.BattlePetClick);
        DrawModeButton(SkeletonType.BeastMaster, PluginConstants.BeastmasterHover,  PluginConstants.BeastmasterIdle,  PluginConstants.BeastmasterClick);
        
        ImDrawListPtr drawList = ImGui.GetWindowDrawList();
        
        Vector4 bgColour = ImGui.GetStyle().Colors[(int)ImGuiCol.WindowBg];
        
        drawList.AddRectFilled(_startPos - new Vector2(5, 5), _endPos + new Vector2(5, 5), ImGui.GetColorU32(bgColour), ImGui.GetStyle().FrameRounding);
    }
    
    private void DrawModeButton(SkeletonType mode, Vector4 hoverColour, Vector4 idleColour, Vector4 clickColour)
    {
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Vector4.Zero);
        ImGui.PushStyleColor(ImGuiCol.Button,        Vector4.Zero);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive,  Vector4.Zero);
        
        DrawFor(mode, hoverColour, idleColour, clickColour);
        
        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            ImGui.SetTooltip(ModeToggleNode.CreateStringFromMode(mode));
        }
        
        ImGui.PopStyleColor(3);
        
        ImGui.SameLine();
    }
    
    private void DrawFor(SkeletonType mode, Vector4 hoverColour, Vector4 idleColour, Vector4 clickColour)
    {
        bool    buttonPressed = false;

        if (attachedTo == null)
        {
            return;
        }
        
        bool petModeIsThis = attachedTo.PetMode == mode;
        
        ImGui.BeginDisabled(petModeIsThis);
        
        Vector2 startPos = ImGui.GetCursorScreenPos();
        
        _endPos = startPos + ModeToggleNode.ButtonSize;
        
        buttonPressed = ImGui.Button($"##ModeToggle_{WindowHandler.InternalCounter}", ModeToggleNode.ButtonSize);
        
        ImGui.EndDisabled();
        
        ImDrawListPtr drawList = ImGui.GetForegroundDrawList();
        ImGuiStylePtr style    = ImGui.GetStyle();
        
        Vector4 activeColour   = idleColour;
        
        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            activeColour = hoverColour;
        }
        
        if (petModeIsThis)
        {
            activeColour = clickColour;
        }
        
        drawList.AddRectFilled(startPos, _endPos, ImGui.GetColorU32(activeColour), style.FrameRounding);
        drawList.AddRect(startPos,       _endPos, ImGui.GetColorU32(style.Colors[(int)ImGuiCol.Border]), style.FrameRounding, style.FrameBorderSize);
        
        if (!buttonPressed)
        {
            return;
        }
        
        attachedTo.SetPetMode(mode);
    }
}