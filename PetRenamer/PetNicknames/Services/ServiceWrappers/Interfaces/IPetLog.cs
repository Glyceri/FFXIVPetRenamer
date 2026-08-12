using System;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IPetLog
{
    void Log(object? message);
    void LogInfo(object? obj);
    void LogWarning(object? obj);
    void LogFatal(object? obj);
    void LogVerbose(object? obj);
    void LogError(Exception e, object? obj);
    void LogException(Exception e);


    void DevLog(object? message);
    void DevLogInfo(object? obj);
    void DevLogWarning(object? obj);
    void DevLogFatal(object? obj);
    void DevLogVerbose(object? obj);
    void DevLogError(Exception e, object? obj);
    void DevLogException(Exception e);
}
