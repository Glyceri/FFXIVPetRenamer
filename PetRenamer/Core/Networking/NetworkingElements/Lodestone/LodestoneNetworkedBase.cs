using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PetRenamer.Core.Networking.NetworkingElements.Lodestone;

public class LodestoneNetworkedBase : NetworkingElement
{
    internal void GetDocument(string url, Action<HtmlDocument> onSuccess, Action<Exception> errorCallback)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
        HttpRequestQueue.Enqueue(request, async (httpResponse) => await OnDocument(httpResponse, onSuccess, errorCallback), errorCallback);
    }

    internal async Task OnDocument(HttpResponseMessage response, Action<HtmlDocument> onSuccess, Action<Exception> errorCallback)
    {
        try
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(await response.Content.ReadAsStringAsync());
            onSuccess?.Invoke(document);
        }
        catch (Exception e)
        {
            errorCallback?.Invoke(e);
        }
    }

    internal HtmlNode? GetNode(HtmlNode baseNode, string nodeName)
    {
        foreach(HtmlNode childNode in baseNode.ChildNodes)
        {
            HtmlNode gottenNode = GetNode(childNode, nodeName)!;
            if (gottenNode != null) return gottenNode;
            if (!childNode.HasClass(nodeName)) continue;
            return childNode;
        }

        return null;
    }

    List<HtmlNode> nodes = new List<HtmlNode>();
    internal HtmlNode[] GetNodes(HtmlNode baseNode, string nodeName)
    {
        nodes.Clear();

        GetNodesRecursive(baseNode, nodeName);

        return nodes.ToArray();
    }

    internal void GetNodesRecursive(HtmlNode baseNode, string nodeName)
    {
        foreach (HtmlNode childNode in baseNode.ChildNodes)
        {
            GetNodesRecursive(childNode, nodeName);
            if (!childNode.HasClass(nodeName)) continue;
            nodes.Add(childNode);
        }
    }
}
