using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;

//Code from daily duties
namespace PetRenamer.Core.Hooking;

internal unsafe class BaseNode
{
    private readonly AtkUnitBase* node;

    public BaseNode(AtkUnitBase* node)
    {
        this.node = node;
    }

    public BaseNode(string addon)
    {
        node = (AtkUnitBase*)PluginHandlers.GameGui.GetAddonByName(addon, 1);
    }

    public AtkResNode* GetRootNode()
    {
        if (node == null) return null;

        return node->RootNode;
    }

    public T* GetNode<T>(uint id) where T : unmanaged
    {
        if (node == null) return null;

        var targetNode = (T*)node->GetNodeById(id);

        return targetNode;
    }

    public ComponentNode GetComponentNode(uint id)
    {
        if (node == null) return new ComponentNode(null);

        var targetNode = (AtkComponentNode*)node->GetNodeById(id);

        return new ComponentNode(targetNode);
    }

    public ComponentNode GetNestedNode(params uint[] idList)
    {
        uint index = 0;

        ComponentNode startingNode;

        do
        {
            startingNode = GetComponentNode(idList[index]);

        } while (index++ < idList.Length);

        return startingNode;
    }
}

internal unsafe class ComponentNode
{
    private readonly AtkComponentNode* node;
    private readonly AtkComponentBase* componentBase;

    public ComponentNode(AtkComponentNode* node)
    {
        this.node = node;

        componentBase = node == null ? null : node->Component;
    }

    public ComponentNode GetComponentNode(uint id)
    {
        if (componentBase == null) return new ComponentNode(null);

        var targetNode = Node.GetNodeByID<AtkComponentNode>(componentBase->UldManager, id);

        return new ComponentNode(targetNode);
    }

    public T* GetNode<T>(uint id) where T : unmanaged
    {
        if (componentBase == null) return null;

        return Node.GetNodeByID<T>(componentBase->UldManager, id);
    }

    public AtkComponentNode* GetPointer()
    {
        return node;
    }
}

internal static unsafe class Node
{
    public static T* GetNodeByID<T>(AtkUldManager uldManager, uint nodeId) where T : unmanaged
    {
        for (var i = 0; i < uldManager.NodeListCount; i++)
        {
            var currentNode = uldManager.NodeList[i];

            if (currentNode->NodeId != nodeId) continue;

            return (T*)currentNode;
        }

        return null;
    }
}