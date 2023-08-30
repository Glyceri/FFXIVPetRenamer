using Dalamud.Loc;
using PetRenamer.Core.Attributes;
using PetRenamer.Core.Handlers;
using System;
using Dalamud.Loc.Enums;
using System.IO;

namespace PetRenamer.Core.Translations;

public static class Translate
{
    public static string GetValue(string key)
    {
        try
        {
            return PluginLink.TranslationHandler?.Translate(key) ?? key;
        } catch { return key; }
    }
}

internal class TranslationHandler : IInitializable, IDisposable
{
    Localization localization = null!;

    public void Dispose()
    {
        localization?.Dispose();
    }

    public void Initialize()
    {
        localization = new Localization(PluginLink.DalamudPlugin);
        Language currentLanguage = LanguageFromConfig();
        localization.LoadLanguage(currentLanguage, JsonFromConfig());
        localization.CurrentLanguage = currentLanguage;
    }

    public string Translate(string key) => localization?.GetString(key) ?? key;

    string JsonFromConfig()
    {
        string path = @"Language\";
        string file = "English.json";

        if (PluginLink.Configuration.language == 1) file = "German.json";
        if (PluginLink.Configuration.language == 2) file = "French.json";
        if (PluginLink.Configuration.language == 3) file = "Japanese.json";
        if (PluginLink.Configuration.language == 4) file = "Dutch.json";

        string finalPath = Path.Combine(PluginLink.DalamudPlugin.AssemblyLocation.Directory?.FullName!, path + file);
        using (StreamReader r = new StreamReader(finalPath))
        {
            string json = r.ReadToEnd();
            return json;
        }
    }

    Language LanguageFromConfig()
    {
        if (PluginLink.Configuration.language == 1) return Language.German;
        if (PluginLink.Configuration.language == 2) return Language.French;
        if (PluginLink.Configuration.language == 3) return Language.Japanese;
        if (PluginLink.Configuration.language == 4) return Language.Norwegian;

        return Language.English;
    }
}
