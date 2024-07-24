﻿using PetRenamer.Legacy.LegacyStepper;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;
using System.IO;

namespace PetRenamer.PetNicknames.Services;

internal class PetServices : IPetServices
{
    public IPetLog PetLog { get; init; }
    public Configuration Configuration { get; init; }
    public IPetSheets PetSheets { get; init; }
    public IStringHelper StringHelper { get; init; }
    public IPetCastHelper PetCastHelper { get; init; }

    readonly DalamudServices DalamudServices;

    public PetServices(DalamudServices services) 
    {
        DalamudServices = services;

        PetLog = new PetLogWrapper(services.PluginLog);
        RenameOldConfig();
        Configuration = services.PetNicknamesPlugin.GetPluginConfig() as Configuration ?? new Configuration();
        StringHelper = new StringHelperWrapper();
        PetSheets = new SheetsWrapper(ref services, StringHelper);
        PetCastHelper = new PetCastWrapper();

        CheckConfigFailure();
    }

    void CheckConfigFailure()
    {
        if (Configuration.currentSaveFileVersion == Configuration.Version) return;
        _ = new LegacyStepper(Configuration, this);
    }

    void RenameOldConfig()
    {
        DirectoryInfo directory = DalamudServices.PetNicknamesPlugin.ConfigDirectory;
        if (directory == null) return;

        try
        {
            string? path = directory.Parent?.FullName;
            if (string.IsNullOrEmpty(path)) return;

            string oldPath = path + "\\PetRenamer.json";
            string newPath = path + "\\PetNicknames.json";

            File.Move(oldPath, newPath);
        } 
        catch (Exception e) 
        {
            DalamudServices.PluginLog.Debug(e.Message);
        }
    }
}
