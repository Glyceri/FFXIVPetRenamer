using Dalamud.Game;
using Newtonsoft.Json;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;
using System.IO;

namespace PetRenamer.PetNicknames.TranslatorSystem;

// Trying to stay away from statics, but in this case it just made MUCH more sense.
internal static class Translator
{
    private static DalamudServices? DalamudServices = null;
    private static IPetServices?    PetServices     = null;

    private static readonly Dictionary<string, string> FallbackTranslations = new Dictionary<string, string>();
    private static readonly Dictionary<string, string> Translations         = new Dictionary<string, string>();
    
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
        
        if (FallbackTranslations.TryGetValue(identifier, out string? fTranslation))
        {
            if (PetServices?.Configuration.debugModeActive ?? false)
            {
                fTranslation = $"@@{fTranslation}@@";
            }
            
            return fTranslation;
        }
        
        return $"%%{identifier}%%";
    }
    
    private static readonly string[] FileNames =
        [
            "en_UK.json",
            "de_DE.json",
            "fr_FR.json",
            "ja_JP.json",
            "nl_NL.json",
        ];
    
    private static string GetFileName(PetNicknamesLanguage language)
    {
        if (language == PetNicknamesLanguage.Default)
        {
            ClientLanguage? cLanguage = DalamudServices?.ClientState.ClientLanguage;
                
            if (cLanguage == null)
            {
                return FileNames[0];
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
        
        return language switch
        {
            PetNicknamesLanguage.English  => FileNames[0],
            PetNicknamesLanguage.German   => FileNames[1],
            PetNicknamesLanguage.French   => FileNames[2],
            PetNicknamesLanguage.Japanese => FileNames[3],
            PetNicknamesLanguage.Dutch    => FileNames[4],
            _                             => FileNames[0],
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
            FileInfo[] files = new DirectoryInfo(DalamudServices.DalamudPlugin.AssemblyLocation.DirectoryName!).GetFiles( "*.json");
            
            FileInfo? foundFile = null;
            
            foreach (FileInfo file in files)
            {
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
            
            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(translationFile)
                                              ?? throw new($"Failed to parse translation file {foundFile.FullName}");
            
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
}
