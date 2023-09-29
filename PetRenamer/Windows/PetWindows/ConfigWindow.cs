using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Windows.PetWindows;

[ConfigPetWindow]
[PersistentPetWindow]
[ModeTogglePetWindow]
public class ConfigWindow : PetWindow
{
    Vector2 baseSize = new Vector2(300, 442);

    public ConfigWindow() : base("Pet Nicknames Settings", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse)
    {
        Size = baseSize;
        SizeCondition = ImGuiCond.FirstUseEver;
        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = baseSize,
            MaximumSize = new Vector2(9999, 9999)
        };
    }

    public override void OnWindowOpen() { }
    public override void OnWindowClose() { }

    public override void OnDraw()
    {
        new ConfigElement(ref PluginLink.Configuration.debugMode, "Debug mode").Draw();
    }

    public override void OnDrawNormal()
    {

    }

    public override void OnDrawBattlePet()
    {
        
    }

    public override void OnDrawSharing()
    {
        
    }

    public override void OnLateDraw()
    {
        
    }
}

internal readonly ref struct ConfigElement
{
    internal readonly ref bool value;
    internal readonly string Title = string.Empty;
    internal readonly string Description = string.Empty;
    internal readonly string Tooltip = string.Empty;
    internal readonly Action<bool> OnToggle = null!;
    internal readonly Func<bool> Allowed = null!;

    public ConfigElement(ref bool value, string Title, string Description = "", string Tooltip = "", Action<bool> OnToggle = null!, Func<bool> Allowed = null!) 
    { 
        this.value = ref value; 
        this.Title = Title; 
        this.Description = Description; 
        this.Tooltip = Tooltip; 
        this.OnToggle = OnToggle;
        this.Allowed = Allowed;
    }

    public void Draw()
    {
        if (System.Runtime.CompilerServices.Unsafe.IsNullRef(ref value)) return;
        ImGui.Checkbox("TESTING!", ref value);
    }
}
