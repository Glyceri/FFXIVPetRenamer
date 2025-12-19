using Dalamud.Plugin.Services;
using HtmlAgilityPack;
using PetRenamer.PetNicknames.Lodestone.Enums;
using PetRenamer.PetNicknames.Lodestone.Interfaces;
using PetRenamer.PetNicknames.Lodestone.Lodestone;
using PetRenamer.PetNicknames.Lodestone.Structs;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PetRenamer.PetNicknames.Lodestone;

internal class LodestoneNetworker : ILodestoneNetworker, IDisposable
{
    // Lodestone rate limter is about 1 second
    private const double QueueIntervalTimer = 1.1f;

    private double queueTimer = 0;

    private readonly List<LodestoneQueueElement> _queueElements = [];
    private readonly List<HtmlNode>              _nodes         = [];

    private readonly HttpClient              Client;
    private readonly PetServices             PetServices;
    private readonly CancellationTokenSource CancellationTokenSource;

    public LodestoneNetworker(PetServices petServices)
    {
        PetServices             = petServices;
        CancellationTokenSource = new CancellationTokenSource();
        Client                  = new HttpClient();
    }

    public ILodestoneQueueElement SearchCharacter(IPettableDatabaseEntry entry, Action<IPettableDatabaseEntry, LodestoneSearchData> success, Action<Exception> failure)
    {
        LodestoneQueueElement queueElement = new LodestoneQueueElement(PetServices, entry, success, failure);

        _queueElements.Add(queueElement);
        queueElement.SetState(LodestoneQueueState.Queued);

        return queueElement;
    }

    public void Update(IFramework framework)
    {
        queueTimer += framework.UpdateDelta.TotalSeconds;

        if (queueTimer >= QueueIntervalTimer)
        {
            queueTimer -= QueueIntervalTimer;

            for (int i = 0; i < _queueElements.Count; i++)
            {
                LodestoneQueueElement element = _queueElements[i];

                if (element.CurrentState != LodestoneQueueState.Queued)
                {
                    continue;
                }

                MoveToObtain(element);

                break;
            }

            for (int i = _queueElements.Count - 1; i >= 0; i--)
            {
                LodestoneQueueElement queueElement = _queueElements[i];

                if (queueElement.CurrentState != LodestoneQueueState.Error && queueElement.CurrentState != LodestoneQueueState.Succeeded)
                {
                    continue;
                }

                queueElement.Dispose();
            }

            for (int i = _queueElements.Count - 1; i >= 0; i--)
            {
                LodestoneQueueElement queueElement = _queueElements[i];

                if (queueElement.CurrentState != LodestoneQueueState.Disposed)
                {
                    continue;
                }

                _queueElements.RemoveAt(i);
            }
        }
    }

    private void MoveToObtain(LodestoneQueueElement lodestoneQueueElement)
    {
        lodestoneQueueElement.SetState(LodestoneQueueState.Obtaining);

        string URL = $"https://na.finalfantasyxiv.com/lodestone/character/?q={HttpUtility.UrlEncode(lodestoneQueueElement.Entry.Name.Replace(" ", "+"))}&worldname={lodestoneQueueElement.Entry.HomeworldName}";

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, URL);

        if (lodestoneQueueElement.Cancelled)
        {
            lodestoneQueueElement.SetState(LodestoneQueueState.TimedOut);
            lodestoneQueueElement.Failure?.Invoke(new Exception());

            return;
        }

        _ = Task.Run(async () =>
        {
            await Execute(request, lodestoneQueueElement);
        }, 
        CancellationTokenSource.Token);
    }

    private async Task Execute(HttpRequestMessage request, LodestoneQueueElement element)
    {
        HttpResponseMessage? response = null;

        if (element.Cancelled)
        {
            element.SetState(LodestoneQueueState.TimedOut);
            element.Failure?.Invoke(new Exception());

            return;
        }

        try
        {
            response = await Client.SendAsync(request, element.CancellationToken);

            _ = response.EnsureSuccessStatusCode(); 
        }
        catch(Exception ex)
        {
            element.SetState(LodestoneQueueState.Error);
            element.Failure?.Invoke(ex);

            return;
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            element.SetState(LodestoneQueueState.Error);
            element.Failure?.Invoke(new Exception("Response not found."));

            return;
        }

        if (response == null)
        {
            element.SetState(LodestoneQueueState.Error);
            element.Failure?.Invoke(new Exception("Response is null."));

            return;
        }

        await CreateDocument(response, element);
    }

    private async Task CreateDocument(HttpResponseMessage responseMessage, LodestoneQueueElement element)
    {
        element.SetState(LodestoneQueueState.LoadDocument);

        try
        {
            HtmlDocument document = new HtmlDocument();

            document.LoadHtml(await responseMessage.Content.ReadAsStringAsync(element.CancellationToken));

            ParseDocument(document, element);
        }
        catch (Exception e)
        {
            element.SetState(LodestoneQueueState.Error);
            element.Failure?.Invoke(e);

            return;
        }
    }

    void ParseDocument(HtmlDocument document, LodestoneQueueElement element)
    {
        element.SetState(LodestoneQueueState.Parsing);

        HtmlNode rootNode = document.DocumentNode;

        if (element.Cancelled)
        {
            element.SetState(LodestoneQueueState.TimedOut);
            element.Failure?.Invoke(new Exception());

            return;
        }

        try
        {
            HtmlNode? listNode = GetNode(rootNode, "ldst__window");

            if (listNode == null)
            {
                element.SetState(LodestoneQueueState.Error);
                element.Failure?.Invoke(new Exception("List Node is not found in HTML document."));

                return;
            }
            HtmlNode? entryNode = GetNode(listNode, "entry");

            if (entryNode == null)
            {
                element.SetState(LodestoneQueueState.Error);
                element.Failure?.Invoke(new Exception("Entry Node is not found in HTML document."));

                return;
            }

            LodestoneSearchData data;

            try
            {
                data = new LodestoneSearchData(entryNode);
            }
            catch (Exception e)
            {
                element.SetState(LodestoneQueueState.Error);
                element.Failure?.Invoke(new Exception("Search Data unable to be made: " + e.Message));

                return;
            }

            Succeed(data, element);
        }
        catch (Exception e)
        {
            element.SetState(LodestoneQueueState.Error);
            element.Failure?.Invoke(new Exception("Search Data unable to be made: " + e.Message));

            return;
        }
    }

    private void Succeed(LodestoneSearchData searchData, LodestoneQueueElement element)
    {
        element.SetState(LodestoneQueueState.Succeeded);
        element.Success?.Invoke(element.Entry, searchData);
    }

    private HtmlNode? GetNode(HtmlNode baseNode, string nodeName)
    {
        foreach (HtmlNode childNode in baseNode.ChildNodes)
        {
            HtmlNode gottenNode = GetNode(childNode, nodeName)!;

            if (gottenNode != null)
            {
                return gottenNode;
            }

            if (!childNode.HasClass(nodeName))
            {
                continue;
            }

            return childNode;
        }

        return null;
    }   

    private void GetNodesRecursive(HtmlNode baseNode, string nodeName)
    {
        foreach (HtmlNode childNode in baseNode.ChildNodes)
        {
            GetNodesRecursive(childNode, nodeName);

            if (!childNode.HasClass(nodeName))
            {
                continue;
            }

            _nodes.Add(childNode);
        }
    }

    public bool IsBeingDownloaded(IPettableDatabaseEntry entry)
    {
        for (int i = 0; i < _queueElements.Count; i++)
        {
            LodestoneQueueElement element = _queueElements[i];

            if (element.Entry.Homeworld != entry.Homeworld)
            {
                continue;
            }

            if (element.Entry.Name != entry.Name)
            {
                continue;
            }

            return true;
        }

        return false;
    }

    public void Dispose()
    {
        CancellationTokenSource.Cancel();
        CancellationTokenSource.Dispose();

        foreach (LodestoneQueueElement queueElement in _queueElements)
        {
            queueElement.Dispose();
        }

        _queueElements.Clear();
    }
}
