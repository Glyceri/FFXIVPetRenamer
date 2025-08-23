using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using Dalamud.Utility;
using Newtonsoft.Json.Linq;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.WritingAndParsing.Enums;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System;

#pragma warning disable IDE0130 // Namespace does not match folder structure (Named like this for easier IPC access)
namespace PetRenamer;
#pragma warning restore IDE0130 // Namespace does not match folder structure

internal unsafe class PenumbraIPC : IPenumbraIPC
{
    private const uint      CurrentPCPVersion   = 1;
    private const string    PCPIdentifier       = "PetNicknames";
    private const string    VersionHeader       = "Version";
    private const string    DataHeader          = "Data";

    private readonly IPetServices               PetServices;
    private readonly IDalamudPluginInterface    PetNicknamesPlugin;
    private readonly IDataWriter                DataWriter;
    private readonly IDataParser                DataParser;

    private readonly ICallGateSubscriber<JObject, string, Guid, object>?    _pcpParsed;
    private readonly ICallGateSubscriber<JObject, ushort, string, object>?  _pcpCreated;

    public PenumbraIPC(IPetServices petServices, IDalamudPluginInterface petNicknamesPlugin, IDataWriter dataWriter, IDataParser dataParser)
    {
        PetNicknamesPlugin  = petNicknamesPlugin;
        PetServices         = petServices;
        DataWriter          = dataWriter;
        DataParser          = dataParser;

        try
        { 
            _pcpParsed      = PetNicknamesPlugin.GetIpcSubscriber<JObject, string, Guid, object>("Penumbra.ParsingPcp");
            _pcpCreated     = PetNicknamesPlugin.GetIpcSubscriber<JObject, ushort, string, object>("Penumbra.CreatingPcp");
        } 
        catch (Exception ex) 
        {
            PetServices.PetLog.LogError(ex, "Pet Nicknames was unable to find Penumbra Subscribers");

            return;
        }

        AssignSubcribers();
    }

    private void AssignSubcribers()
    {
        _pcpParsed?.Subscribe(OnPcpParsed);
        _pcpCreated?.Subscribe(OnPcpCreated);
    }

    private void OnPcpParsed(JObject jsonObject, string modDirectory, Guid collection)
    {
        PetServices.PetLog.Log("Pet Nicknames is parsing PCP data for: " + modDirectory + ", " + collection.ToString());

        if (!PetServices.Configuration.readFromPCP)
        {
            PetServices.PetLog.LogWarning("Settings disallow adding Pet Nicknames data from a PCP file.");

            return;
        }

        if (jsonObject["Actor"] is not JObject { })
        {
            PetServices.PetLog.LogWarning("Actor is not a valid JObject. I assume the JsonObject is invalid.");

            return;
        }

        if (jsonObject[PCPIdentifier] is not JObject { } petNicknamesObject)
        {
            PetServices.PetLog.LogWarning($"{PCPIdentifier} is not a valid JObject. I assume the JsonObject is invalid.");

            return;
        }
        
        JToken? versionToken = petNicknamesObject[VersionHeader];

        if (versionToken == null)
        {
            PetServices.PetLog.LogWarning("versionToken is null.");

            return;
        }

        int parsedVersion = versionToken.ToObject<int>();

        PetServices.PetLog.Log($"Version is equal to: {parsedVersion}");

        if (parsedVersion != 1) // If there are more version make this better... obviously
        {
            PetServices.PetLog.LogFatal($"{parsedVersion} is NOT supported by your current install of Pet Nicknames.");

            return;
        }

        JToken? dataToken = petNicknamesObject[DataHeader];

        if (dataToken == null)
        {
            PetServices.PetLog.LogWarning("dataToken is null.");

            return;
        }

        string? data = dataToken.ToObject<string>();

        if (data.IsNullOrWhitespace())
        {
            PetServices.PetLog.LogWarning("The acquired data is empty.");

            return;
        }

        PetServices.PetLog.Log("Pet Nicknames is about to parse the Data object.");

        IDataParseResult parseResult = DataParser.ParseData(data);

        PetServices.PetLog.Log("Pet Nicknames is about to apply the parsed data.");

        bool parseWasSuccess = DataParser.ApplyParseData(parseResult, ParseSource.PCP);

        if (!parseWasSuccess)
        {
            PetServices.PetLog.LogWarning("Applying the parsed data has failed.");

            return;
        }

        PetServices.PetLog.LogInfo("Pet Nicknames has successfully imported data from the PCP file.");
    }

    private void OnPcpCreated(JObject jsonObject, ushort objectIndex, string collection)
    {
        PetServices.PetLog.Log("Pet Nicknames is adding data to PCP for: " + objectIndex);

        if (objectIndex != 0)
        {
            PetServices.PetLog.LogWarning("You can only add your own data to a PCP file.");

            return;
        }

        if (!PetServices.Configuration.attachToPCP)
        {
            PetServices.PetLog.LogWarning("Settings disallow adding Pet Nicknames to the PCP file.");

            return;
        }

        if (jsonObject["Actor"] is not JObject { })
        {
            PetServices.PetLog.LogWarning("Actor is not a valid JObject. I assume the JsonObject is invalid.");

            return;
        }

        string petNicknamesData = DataWriter.WriteData();

        if (petNicknamesData.IsNullOrWhitespace())
        {
            PetServices.PetLog.LogWarning("Pet Nicknames would be writing nothing to the PCP file, so it aborted early.");

            return;
        }

        jsonObject[PCPIdentifier] = new JObject()
        {
            [VersionHeader] = CurrentPCPVersion,
            [DataHeader]    = petNicknamesData
        };

        PetServices.PetLog.LogInfo("Pet Nicknames has successfully added data to the PCP JsonObject.");

#if DEBUG
        PetServices.PetLog.LogVerbose(jsonObject.ToString());
#endif
    }

    public void Dispose()
    {
        _pcpParsed?.Unsubscribe(OnPcpParsed);
        _pcpCreated?.Unsubscribe(OnPcpCreated);
    }
}
