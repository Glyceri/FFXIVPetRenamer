using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using PetRenamer.Core.Hooking.Attributes;
using System.Runtime.InteropServices;
using System.Text;
using System;
using PetRenamer.Core.Singleton;

namespace PetRenamer.Core.Hooking.Hooks;

// Code mostly from: https://github.com/goaaats/Dalamud.FindAnything/blob/add7d4f4f3f7b5d5a659dadf91dc8e17efbdd7f6/Dalamud.FindAnything/Command.cs#L43

[Hook]
internal class CommandHook : HookableElement, ISingletonBase<CommandHook>
{
    [Signature("48 89 5C 24 ?? 57 48 83 EC 20 48 8B FA 48 8B D9 45 84 C9", DetourName = nameof(ProcessChatBoxDelegate))]
    readonly Hook<Delegates.ProcessChatBoxDelegate>? processChatBox = null!;

    public static CommandHook instance { get; set; } = null!;

    internal override void OnInit() => processChatBox?.Enable();
    internal override void OnDispose() => processChatBox?.Dispose();

    unsafe void ProcessChatBoxDelegate(nint uiModule, nint message, nint unused, byte a4) => processChatBox!.Original(uiModule, message, unused, a4);
    public unsafe void SendChatUnsafe(string command)
    {
        if (processChatBox == null) return;

        nint uiModule = (nint)Framework.Instance()->GetUiModule();

        using ChatPayload payload = new ChatPayload(Encoding.UTF8.GetBytes($"{command}"));
        nint mem1 = Marshal.AllocHGlobal(400);
        Marshal.StructureToPtr(payload, mem1, false);

        ProcessChatBoxDelegate(uiModule, mem1, nint.Zero, 0);

        Marshal.FreeHGlobal(mem1);
    }
}

[StructLayout(LayoutKind.Explicit)]
internal readonly struct ChatPayload : IDisposable
{
    [FieldOffset(0)]
    private readonly nint textPtr;

    [FieldOffset(16)]
    private readonly ulong textLen;

    [FieldOffset(8)]
    private readonly ulong unk1;

    [FieldOffset(24)]
    private readonly ulong unk2;

    internal ChatPayload(byte[] stringBytes)
    {
        textPtr = Marshal.AllocHGlobal(stringBytes.Length + 30);
        Marshal.Copy(stringBytes, 0, textPtr, stringBytes.Length);
        Marshal.WriteByte(textPtr + stringBytes.Length, 0);

        textLen = (ulong)(stringBytes.Length + 1);

        unk1 = 64;
        unk2 = 0;
    }

    public void Dispose()
    {
        Marshal.FreeHGlobal(textPtr);
    }
}

