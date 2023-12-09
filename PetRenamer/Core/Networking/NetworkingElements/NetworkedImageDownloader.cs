using PetRenamer.Core.Networking.Attributes;
using PetRenamer.Core.Singleton;
using System.IO;
using System.Net.Http;
using System.Threading;
using System;
using System.Threading.Tasks;
using PetRenamer.Utilization.UtilsModule;

namespace PetRenamer.Core.Networking.NetworkingElements;

[Networked]
public class NetworkedImageDownloader : NetworkingElement, ISingletonBase<NetworkedImageDownloader>
{
    public static NetworkedImageDownloader instance { get; set; } = null!;

    public async void AsyncDownload(string URL, (string, uint) character, Action callbackSucces = null!, Action<Exception> callbackError = null!) => AsyncDownload(URL, MakeTexturePath(RemapCharacterData(ref character)), callbackSucces, callbackError);
    public async void AsyncDownload(string URL, string savePath, Action callbackSucces = null!, Action<Exception> callbackError = null!)
    {
        try
        {
            CancellationToken cancellationToken = CancellationToken.None;
            using HttpResponseMessage response = await client.GetAsync(URL);
            if (response == null)
            {
                callbackError?.Invoke(new Exception("Response is Null for: " + URL + " at: " + savePath));
                return;
            }
            response.EnsureSuccessStatusCode();

            // Thank DarkArchon for this code :D

            FileStream fileStream = File.Create(savePath);
            await using (fileStream.ConfigureAwait(false))
            {
                int bufferSize = response.Content.Headers.ContentLength > 1024 * 1024 ? 4096 : 1024;
                byte[] buffer = new byte[bufferSize];

                int bytesRead = 0;
                while ((bytesRead = await (await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false)).ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
                }
            }
        }
        catch (Exception e)
        {
            callbackError?.Invoke(e);
            return;
        }

        callbackSucces?.Invoke();
    }

    public (string, string) RemapCharacterData(ref (string, uint) characterData) => (characterData.Item1, SheetUtils.instance.GetWorldName((ushort)characterData.Item2));
    public string MakeTexturePath((string, string) characterData) => Path.Combine(Path.GetTempPath(), $"PetNicknames_{characterData.Item1.Replace(" ", "_")}_{characterData.Item2}.jpg");
}
