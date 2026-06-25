using Dalamud.Game;
using Newtonsoft.Json;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;
using System.IO;

namespace PetRenamer.PetNicknames.TranslatorSystem;

internal static class Translator
{
    private static DalamudServices? DalamudServices = null;
    private static IPetServices?    PetServices     = null;

    private static readonly Dictionary<string, string> FallbackTranslations = new Dictionary<string, string>();
    private static readonly Dictionary<string, string> Translations         = new Dictionary<string, string>();
    
    private static readonly string[] FileNames =
    [
        "en_UK.json",
        "de_DE.json",
        "fr_FR.json",
        "ja_JP.json",
        "nl_NL.json",
        "zh_CN.json",
    ];
    
    internal static void Initialise(DalamudServices dalamudServices, IPetServices petServices)
    {
        DalamudServices = dalamudServices;
        PetServices     = petServices;
        
        FillDictForLanguage(FallbackTranslations, PetNicknamesLanguage.English);
        
        UpdateLanguage();
    }

    internal static string GetLine(string identifier)
    {
        if (Translations.TryGetValue(identifier, out string? translation))
        {
            return translation;
        }
        
        if (!FallbackTranslations.TryGetValue(identifier, out string? fTranslation))
        {
            return $"%%{identifier}%%";
        }
        
        if (!(PetServices?.Configuration.debugModeActive ?? false))
        {
            return fTranslation;
        }
        
        if (!(PetServices?.Configuration.showFailedTranslations ?? false))
        {
            return fTranslation;
        }
        
        return $"@@{fTranslation}@@";
    }
    
    private static string GetFileName(PetNicknamesLanguage language)
    {
        if (language != PetNicknamesLanguage.Default)
        {
            return language switch
            {
                PetNicknamesLanguage.English  => FileNames[0],
                PetNicknamesLanguage.German   => FileNames[1],
                PetNicknamesLanguage.French   => FileNames[2],
                PetNicknamesLanguage.Japanese => FileNames[3],
                PetNicknamesLanguage.Dutch    => FileNames[4],
                PetNicknamesLanguage.Chinese  => FileNames[5],
                _                             => FileNames[0],
            };
        }

        ClientLanguage? cLanguage = DalamudServices?.ClientState.ClientLanguage;
                
        if (cLanguage == null)
        {
            return FileNames[0];
        }

        // CN/KR/TW are absent from Dalamud's ClientLanguage (it falls back to English), but the
        // Lumina game data language reports them correctly. These languages don't exist on global,
        // so checking it here only affects those regions and is safe for global clients.
        Lumina.Data.Language? excelLanguage = DalamudServices?.DataManager.GameData.Options.DefaultExcelLanguage;

        switch (excelLanguage)
        {
            case Lumina.Data.Language.ChineseSimplified:  return FileNames[5];
            // case Lumina.Data.Language.ChineseTraditional: return FileNames[6]; // Add once a zh_TW.json exists.
            // case Lumina.Data.Language.Korean:             return FileNames[7]; // Add once a ko_KR.json exists.
        }

        return cLanguage.Value switch
        {
            ClientLanguage.English  => FileNames[0],
            ClientLanguage.German   => FileNames[1],
            ClientLanguage.French   => FileNames[2],
            ClientLanguage.Japanese => FileNames[3],
            _                       => FileNames[0],
        };
    }
    
    private static void FillDictForLanguage(Dictionary<string, string> dictionary, PetNicknamesLanguage language)
    {
        if (DalamudServices == null)
        {
            return;
        }
        
        dictionary.Clear();
        
        string fileName = GetFileName(language);

        try
        {
            FileInfo[] files = new DirectoryInfo(Path.Combine(DalamudServices.DalamudPlugin.AssemblyLocation.DirectoryName!, "I18N")).GetFiles("*.json" );
            
            FileInfo? foundFile = null;
            
            foreach (FileInfo file in files)
            {
                if (PetServices?.Configuration.debugModeActive ?? false)
                {
                    PetServices.PetLog.LogVerbose(file.FullName);
                }
                
                if (file.Name != fileName)
                {
                    continue;
                }
                
                foundFile = file;
                
                DalamudServices.PluginLog.Info("File Found: " + file.Name);
                
                break;
            }

            if (foundFile == null)
            {
                DalamudServices.PluginLog.Error("Translation file not found");
                
                return;
            }
            
            string translationFile = File.ReadAllText(foundFile.FullName);
            
            Dictionary<string, string>? dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(translationFile);
            
            if (dict == null)
            {
                DalamudServices.PluginLog.Error("Json failed to deserialize properly.");
                
                return;
            }
            
            foreach ((string key, string value) in dict) 
            {
                dictionary[key] = value;
            }
        }
        catch (Exception e)
        {
            DalamudServices?.PluginLog.Error(e, "Error when loading language file");
        }
    }
    
    public static void UpdateLanguage()
    {
        PetNicknamesLanguage? pLanguage = PetServices?.Configuration.currentLanguage;
        
        if (pLanguage == null)
        {
            return;
        }
        
        FillDictForLanguage(Translations, pLanguage.Value);
    }
}

internal enum PetNicknamesLanguage
{
    Default,
    English,
    German,
    French,
    Japanese,
    Dutch,
    Chinese,
}
