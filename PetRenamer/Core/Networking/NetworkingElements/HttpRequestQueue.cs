using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PetRenamer.Core.Networking.NetworkingElements;

public static class HttpRequestQueue
{
    static readonly HttpClient client = new HttpClient();
    static List<(HttpRequestMessage, Action<HttpResponseMessage>, Action<Exception> e)> queue = new List<(HttpRequestMessage, Action<HttpResponseMessage>, Action<Exception> e)> ();

    const double timerInterval = 1.0;
    static double timer = 0;

    public static void Update(ref IFramework frameWork, ref PlayerCharacter player)
    {
        if(timer <= 0 && queue.Count > 0)
        {
            timer = timerInterval;
            (HttpRequestMessage, Action<HttpResponseMessage>, Action<Exception>) element = queue[0];
            queue.Remove(element);
            try
            {
                Task.Run(async () => await Execute(element.Item1, element.Item2, element.Item3));
            }
            catch (Exception e) { element.Item3?.Invoke(e); }
        }

        if (timer > 0) timer -= frameWork.UpdateDelta.TotalSeconds;
    }

    public static void Dispose()
    {
        queue.Clear();
    }

    static async Task Execute(HttpRequestMessage request, Action<HttpResponseMessage> successCallback, Action<Exception> errorCallback)
    {
        HttpResponseMessage? response;

        try
        {
            response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            errorCallback?.Invoke(e);
            return;
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            errorCallback?.Invoke(new Exception("Response not found."));
            return;
        }

        successCallback?.Invoke(response);
    }

    public static void Enqueue(HttpRequestMessage httpRequestMessage, Action<HttpResponseMessage> succesResponse, Action<Exception> errorResponse)
    {
        queue.Add((httpRequestMessage, succesResponse, errorResponse));
    }
}
