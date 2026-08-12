using System;

namespace PetRenamer.PetNicknames.GroupHandling.Interfaces;

internal interface IHandlerGroup : IDisposable
{
    void SetGroupState(ref Configuration.GroupConfig groupConfig);
    ref Configuration.GroupConfig GetGroupConfig(Configuration configuration);
}