﻿using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin.Services;
using Dalamud.Plugin;

namespace PetRenamer.PetNicknames.Services;

internal class DalamudServices
{
                    internal IDalamudPluginInterface         PetNicknamesPlugin          { get; private set; } = null!;
    [PluginService] internal ICommandManager                 CommandManager              { get; private set; } = null!;
    [PluginService] internal IFramework                      Framework                   { get; private set; } = null!;
    [PluginService] internal IDataManager                    DataManager                 { get; private set; } = null!;
    [PluginService] internal IClientState                    ClientState                 { get; private set; } = null!;
    [PluginService] internal IGameGui                        GameGui                     { get; private set; } = null!;
    [PluginService] internal ITargetManager                  TargetManager               { get; private set; } = null!;
    [PluginService] internal IObjectTable                    ObjectTable                 { get; private set; } = null!;
    [PluginService] internal IChatGui                        ChatGui                     { get; private set; } = null!;
    [PluginService] internal IFlyTextGui                     FlyTextGui                  { get; private set; } = null!;
    [PluginService] internal ITextureProvider                TextureProvider             { get; private set; } = null!;
    [PluginService] internal IPluginLog                      PluginLog                   { get; private set; } = null!;
    [PluginService] internal IGameInteropProvider            Hooking                     { get; private set; } = null!;
    [PluginService] internal IAddonLifecycle                 AddonLifecycle              { get; private set; } = null!;
    [PluginService] internal IPartyList                      PartyList                   { get; private set; } = null!;
    [PluginService] internal IContextMenu                    ContextMenu                 { get; private set; } = null!;

    public static DalamudServices Create(ref IDalamudPluginInterface plugin)
    {
        DalamudServices service = plugin.Create<DalamudServices>()!;
        service.PetNicknamesPlugin = plugin;
        return service;
    }
}
