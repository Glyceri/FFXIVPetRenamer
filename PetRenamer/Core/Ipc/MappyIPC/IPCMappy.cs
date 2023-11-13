using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem.Pet;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.PetWindows;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Channels;

namespace PetRenamer.Core.Ipc.MappyIPC;

public static class IPCMappy
{
    static ICallGateSubscriber<uint, Vector2, uint, string, string, string>? AddWorldMarkerIpcFunction = null;
    static ICallGateSubscriber<string, bool>? RemoveMarkerIpcFunction = null;
    static ICallGateSubscriber<string, Vector2, bool>? UpdateMarkerIpcFunction = null;
    static ICallGateSubscriber<bool>? IsReadyIpcFunction = null;

    static bool initialized = false;

    public static bool MappyAvailable => mappyReady;

    public static void Init(ref DalamudPluginInterface pluginInterface)
    {
        AddWorldMarkerIpcFunction = pluginInterface.GetIpcSubscriber<uint, Vector2, uint, string, string, string>("Mappy.World.AddMarker");
        RemoveMarkerIpcFunction = pluginInterface.GetIpcSubscriber<string, bool>("Mappy.RemoveMarker");
        UpdateMarkerIpcFunction = pluginInterface.GetIpcSubscriber<string, Vector2, bool>("Mappy.UpdateMarker");
        IsReadyIpcFunction = pluginInterface.GetIpcSubscriber<bool>("Mappy.IsReady");
        initialized = true;
    }

    public static void DeInit() => ClearMappy();

    static List<string> ids = new List<string>();
    const int petIconID = 60961;
    static bool handleAsChanged = false;
    static bool changed = false;
    static bool lastMappyReady = false;
    static bool mappyReady = false;
    static double timer = timeBetweenChecks;
    const double timeBetweenChecks = 10;
    static bool oneTime = false;
    static bool pluginToggled = false;
    static int counter = 0;

    public static void Update(ref IFramework frameWork, ref PlayerCharacter player)
    {
        if (!initialized) return;
        HandleSettingChanged();
        if (!PluginLink.Configuration.enableMappyIntegration && oneTime) { changed = false; return; }
        HandleTenSeconds(ref frameWork);
        if (!mappyReady) return; 
        HandleMappyWindow();
        if (!PluginLink.Configuration.understoodWarningThirdPartySettings) return;
        HandlePartyListChangedCheck(ref frameWork, ref player);
        UpdatePetPositions();
    }

    unsafe static void UpdatePetPositions()
    {
        counter = 0;
        for (int i = 0; i < PartyUtils.instance.Length; i++)
        {
            PartyPlayer partyPlayer = PartyUtils.instance.members[i];
            foreach (nint pet in partyPlayer.ActivePets)
            {
                if (handleAsChanged) HandleChanged(pet);
                CallUpdatePos(pet);
            }
        }
    }

    unsafe static void HandleChanged(nint pet)
    {
        try
        {
            PetBase petBase = PluginLink.PettableUserHandler.GetPet(pet);
            string name = string.Empty;
            if (petBase != null) name = petBase.CustomName;
            if (name == string.Empty) name = Marshal.PtrToStringUTF8((IntPtr)((GameObject*)pet)->GetName())!;
            ids.Add(AddWorldMarkerIpcFunction!.InvokeFunc(petIconID, Vector2.Zero, 0, name, string.Empty));
        }
        catch { MappyFailed(); }
    }

    unsafe static void CallUpdatePos(nint pet)
    {
        try
        {
            Vector3 pos = ((GameObject*)pet)->Position;
            UpdateMarkerIpcFunction!.InvokeFunc(ids[counter], new Vector2(pos.X, pos.Z));
        }
        catch { MappyFailed(); }

        counter++;
    }


    static void HandleMappyWindow()
    {
        if (oneTime) return;
        oneTime = true;
        PluginLink.WindowHandler.GetWindow<MappyXPetNicknamesWindow>()?.TryOpen();
    }

    static void HandlePartyListChangedCheck(ref IFramework frameWork, ref PlayerCharacter player)
    {
        // Checks if the party list has changed
        if (PartyUtils.instance.Update(ref frameWork, ref player) || changed || pluginToggled)
        {
            ClearMappy();
            handleAsChanged = true;
        }
    }

    static bool lastSetting = false;
    static void HandleSettingChanged()
    {
        pluginToggled = false;
        bool enabled = PluginLink.Configuration.enableMappyIntegration;
        if (lastSetting == enabled && !changed) return;
        lastSetting = enabled;

        if (!PluginLink.Configuration.enableMappyIntegration)
        {
            ClearMappy();
            PluginLink.ChatHandler.AddBlacklistedChats(1);
            PluginHandlers.CommandManager.ProcessCommand("/mappy pets enable");
        }
        else
        {
            pluginToggled = true;
            PluginLink.ChatHandler.AddBlacklistedChats(1);
            PluginHandlers.CommandManager.ProcessCommand("/mappy pets disable");
        }
    }

    static void HandleTenSeconds(ref IFramework frameWork)
    {
        handleAsChanged = false;
        changed = false;
        timer += frameWork.UpdateDelta.TotalSeconds;
        if (timer <= timeBetweenChecks) return;

        timer = 0;
        mappyReady = CheckIfMappyReady();
        OnMappyStateChange();
    }

    static void ClearMappy()
    {
        foreach (string s in ids)
        {
            try
            {
                RemoveMarkerIpcFunction?.InvokeFunc(s);
            }
            catch { MappyFailed(); }
        }
        ids.Clear();
    }

    static bool CheckIfMappyReady()
    {
        try
        {
            return IsReadyIpcFunction?.InvokeFunc() ?? false;
        }
        catch { MappyFailed(); }
        return false;
    }

    static void MappyFailed()
    {
        mappyReady = false;
        OnMappyStateChange();
    }

    static void OnMappyStateChange()
    {
        if (mappyReady == lastMappyReady) return;
        changed = true;
        lastMappyReady = mappyReady;
    }
}
