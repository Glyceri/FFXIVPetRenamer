using PetRenamer.PetNicknames.Services;
using System;

namespace PetRenamer.PetNicknames.GroupHandling.Interfaces;

internal interface IHandlerGroup : IDisposable
{
    string GetHandlerTitle();
    string GetTitle(EnabledState enabledState);
    string GetDescription(EnabledState enabledState);
    
    void SetGroupState(ref Configuration.GroupConfig groupConfig);
    ref Configuration.GroupConfig GetGroupConfig(Configuration configuration);
}