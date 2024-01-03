using Dalamud.Plugin.Services;
using ImGuiNET;
using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Handlers;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.Windows.Bonus;

internal class ToolbarAnimator : RegistryBase<ToolbarAnimation, ToolbarAnimationAttribute>
{
    ToolbarAnimation activeAnimation = null!;
    string[] _registeredIdentifiers = new string[0];
    public string[] registeredIdentifiers => _registeredIdentifiers;

    protected override void OnAllRegistered()
    {
        RegisterActiveAnimation(PluginLink.Configuration.activeElement);
        _registeredIdentifiers = new string[attributes.Count];
        for (int i = 0; i < _registeredIdentifiers.Length; i++)
            _registeredIdentifiers[i] = attributes[i].Identifier;
    }
    protected override void OnDipose() => PluginHandlers.Framework.Update -= OnUpdate;

    public void DoDraw(ImDrawListPtr drawListPtr, Vector2 startingPoint, Vector2 endPoint) => activeAnimation?.Draw(drawListPtr, startingPoint, endPoint);
    public void OnUpdate(IFramework framework) => activeAnimation?.Update(framework.UpdateDelta.TotalSeconds);

    public void RegisterActiveAnimation(string elementName) => RegisterActiveAnimation(FindElement(elementName));
    void RegisterActiveAnimation(ToolbarAnimation animation)
    {
        activeAnimation?.Clear();
        activeAnimation = animation;
        activeAnimation?.Initialize();
        PluginHandlers.Framework.Update -= OnUpdate;
        if (activeAnimation != null && activeAnimation is not DudAnimation) PluginHandlers.Framework.Update += OnUpdate;
    }

    ToolbarAnimation FindElement(string elementName)
    {
        if (elementName == null || elementName == string.Empty) return null!;
        for(int i = 0; i < attributes.Count; i++)
        {
            ToolbarAnimationAttribute attribute = attributes[i];
            if (attribute.Identifier == elementName) return elements[i];
        }
        return null!;
    }
}
