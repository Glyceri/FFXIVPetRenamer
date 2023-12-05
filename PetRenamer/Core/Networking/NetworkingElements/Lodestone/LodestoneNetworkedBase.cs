using HtmlAgilityPack;
using System.Net;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PetRenamer.Core.Networking.NetworkingElements.Lodestone;

public class LodestoneNetworkedBase : NetworkingElement
{
    internal async Task<HtmlDocument?> GetDocument(string url, Action<Exception> errorCallback)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
        HttpResponseMessage? response;

        try
        {
            response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            errorCallback?.Invoke(e);
            return null!;
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            errorCallback?.Invoke(new Exception("Response not found."));
            return null!;
        }

        try
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(await response.Content.ReadAsStringAsync());
            return document;
        }
        catch (Exception e)
        {
            errorCallback?.Invoke(e);
        }
        return null!;
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
