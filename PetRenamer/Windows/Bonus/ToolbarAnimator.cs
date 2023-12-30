using Dalamud.Plugin.Services;
using ImGuiNET;
using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Handlers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace PetRenamer.Windows.Bonus;

internal class ToolbarAnimator : RegistryBase<ToolbarAnimation, ToolbarAnimationAttribute>
{
    ToolbarAnimation activeAnimation = null!;
    string[] _registeredIdentifiers = new string[0];
    public string[] registeredIdentifiers => _registeredIdentifiers;

    protected override void OnAllRegistered()
    {
        PluginHandlers.Framework.Update += OnUpdate;
        RegisterActiveAnimation(PluginLink.Configuration.activeElement);
        _registeredIdentifiers = new string[attributes.Count];
        List<(int, string)> identifiers = new List<(int, string)>();
        for (int i = 0; i < attributes.Count; i++)
            identifiers.Add((attributes[i].Order, attributes[i].Identifier));
        identifiers.Sort((el1, el2) => el1.Item1.CompareTo(el2.Item1));
        for (int i = 0; i < identifiers.Count; i++)
            _registeredIdentifiers[i] = identifiers[i].Item2;
    }
    protected override void OnDipose() => PluginHandlers.Framework.Update += OnUpdate;

    public void DoDraw(ImDrawListPtr drawListPtr, Vector2 startingPoint, Vector2 endPoint) => activeAnimation?.Draw(drawListPtr, startingPoint, endPoint);
    public void OnUpdate(IFramework framework) => activeAnimation?.Update(framework.UpdateDelta.TotalSeconds);

    public void RegisterActiveAnimation(string elementName) => RegisterActiveAnimation(FindElement(elementName));
    void RegisterActiveAnimation(ToolbarAnimation animation)
    {
        activeAnimation?.Clear();
        activeAnimation = animation;
        activeAnimation?.Initialize();
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
