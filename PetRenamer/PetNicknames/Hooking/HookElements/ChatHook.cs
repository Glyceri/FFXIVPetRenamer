using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Utility;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.Log;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class ChatHook : HookableElement
{
    private delegate nint GetLogMessageRawDelegate(LogModule* logModule, int index, nint unk3);
    private delegate nint ClearLogDelegate(LogModule* logModule);
    
    [Signature("E8 ?? ?? ?? ?? 48 8B F8 48 85 C0 0F 84 ?? ?? ?? ?? 49 8B 9E", DetourName = nameof(GetLogMessageRawDetour))]
    private readonly Hook<GetLogMessageRawDelegate>? GetLogMessageRawHook;
    
    [Signature("E8 ?? ?? ?? ?? 49 8B CD E8 ?? ?? ?? ?? 45 84 E4", DetourName = nameof(ClearLogDetour))]
    private readonly Hook<ClearLogDelegate>? ClearLogHook;
    
    private readonly Hook<RaptureLogModule.Delegates.FormatLogMessage> FormatLogMessageHook;
    
   
    
    private readonly IPettableDatabase Database;
    private readonly IPronounHook      PronounHook;
    
    private int  _lastIndex         = -1;
    private uint _lastIdentifier    = 0;
    private uint _lastSubIdentifier = 0;
    private bool _myCall = false;
    
    private NameType _replaceNameType = NameType.Action;
    private IPetSheetData? _replaceData = null;
    
    private readonly List<PlayerElement> _players = [];
    private readonly List<PetElement>    _pets    = [];
    private readonly List<ChatElement>   _chats   = [];
    
    public ChatHook(DalamudServices services, IPetServices petServices, IPettableDatabase database, IPronounHook pronounHook) 
        : base(services, petServices)
    {
        Database = database;
        PronounHook = pronounHook;
        
        FormatLogMessageHook = DalamudServices.Hooking.HookFromAddress<RaptureLogModule.Delegates.FormatLogMessage>(RaptureLogModule.MemberFunctionPointers.FormatLogMessage, FormatLogMessageDetour);
    }

    public override void Init()
    {
        FormatLogMessageHook.Enable();
        GetLogMessageRawHook?.Enable();
        ClearLogHook?.Enable();
        
        DalamudServices.ChatGui.LogMessage  += OnChatLog;
        DalamudServices.ChatGui.ChatMessage += OnChatMessage;
    }

    protected override void OnDispose()
    {
        DalamudServices.ChatGui.LogMessage  -= OnChatLog;
        DalamudServices.ChatGui.ChatMessage -= OnChatMessage;
        
        ClearLogHook?.Dispose();
        FormatLogMessageHook.Dispose();
        GetLogMessageRawHook?.Dispose();
    }
    
    private void OnChatLog(ILogMessage logMessage)
    {
        uint        messageId = GetMessageIdentifier(ChatKind.Log);
        XivChatType chatType  =  (XivChatType)logMessage.GameData.Value.LogKind.RowId;
        
        PetServices.PetLog.LogWarning(messageId + " : " + logMessage.LogMessageId);
        
        DebugChat(ChatKind.Log, messageId, chatType);
        
        if (PetServices.Configuration.debugModeActive)
        {
            PetServices.PetLog.Log("OnChatLog: '" + PetServices.PetSheets.GetLogMessage(logMessage.LogMessageId)?.Text.ToDalamudString().TextValue + "', " + messageId);
        }
        
        _replaceNameType = NameType.Raw;
        _replaceData = null;
        
        PlayerElement? sourcePlayer = ParsePlayer(logMessage.SourceEntity);
        PlayerElement? targetPlayer = ParsePlayer(logMessage.TargetEntity);
        PetElement?    sourcePet    = ParsePet(chatType, logMessage.LogMessageId, sourcePlayer, logMessage.SourceEntity);
        PetElement?    targetPet    = ParsePet(chatType, logMessage.LogMessageId, sourcePlayer, logMessage.TargetEntity);

        PetServices.PetLog.LogWarning("SOURCE PLAYER: " + sourcePlayer);
        PetServices.PetLog.LogWarning("TARGET PLAYER: " + targetPlayer);
        PetServices.PetLog.LogWarning("SOURCE PET: " + sourcePet);
        PetServices.PetLog.LogWarning("TARGET PET: " + targetPet);
        
        if (_replaceData == null)
        {
            return;
        }
        
        if (_replaceNameType == NameType.Pronoun)
        {
            _chats.Add(new ChatElement(messageId, chatType, sourcePlayer, targetPlayer, sourcePet, targetPet));
        }
        else
        {
            string? replaceString = PetServices.NameService.GetName(_replaceNameType, _replaceData);
            
            if (replaceString.IsNullOrWhitespace())
            {
                return;
            }
            
            _chats.Add(new ChatElement(messageId, chatType, replaceString, sourcePlayer, targetPlayer, sourcePet, targetPet));
        }
    }
    
    private void OnChatMessage(IHandleableChatMessage chatMessage)
    {
        uint        messageId = GetMessageIdentifier(ChatKind.Chat);
        XivChatType chatType  = chatMessage.LogKind;
        
        DebugChat(ChatKind.Chat, messageId, chatType);
        
        if (PetServices.Configuration.debugModeActive)
        {
            PetServices.PetLog.Log("OnChatMessage: '" + chatMessage.Message.TextValue + "', " + messageId);
        }
        
        if (chatType != XivChatType.StandardEmote)
        {
            return;
        }
        
        ChatElement? foundElement = GetChatElement((int)messageId);
        
        if (foundElement == null)
        {
            return;
        }
        
        _chats.Remove(foundElement);
        
        if (foundElement.TargetPet == null)
        {
            return;
        }
        
        IPetSheetData? data = PetServices.PetSheets.GetPet(foundElement.TargetPet.Pet);
        
        if (data == null)
        {
            return;
        }
        
        string? replaceString = PetServices.NameService.GetName(NameType.Pronoun, data);
            
        if (replaceString.IsNullOrWhitespace())
        {
            return;
        }
        
        foundElement.SetReplaceString(replaceString);
        
        _chats.Add(foundElement);
    }
    
    private void DebugChat(ChatKind chatKind, uint messageId, XivChatType chatType)
    {
        PetServices.PetLog.LogFatal($"ChatDebug: {_lastIndex}, {chatKind}, {messageId}, {chatType}.");
    }

    private uint GetMessageIdentifier(ChatKind chatKind)
    {
        int start = *(int*)((nint)RaptureLogModule.Instance() + 0x18);
        int count = RaptureLogModule.Instance()->LogMessageCount - start;
        
        uint currentValue = (uint)(count - start);
        
        if (_lastIndex == -1)
        {
            _lastIndex = (int)currentValue - 1;
        }
        
        if (currentValue == _lastIdentifier && chatKind == ChatKind.Log)
        {
            _lastSubIdentifier++;
        }
        else
        {
            _lastSubIdentifier = 0;
        }
        
        _lastIdentifier = currentValue;
        
        return (uint)(count - start) + _lastSubIdentifier;
    }
    
    private PetElement? ParsePet(XivChatType chatType, uint messageId, PlayerElement? playerElement, ILogMessageEntity? logMessageEntity)
    {
        if (logMessageEntity == null)
        {
            return null;
        }
        
        if (chatType == XivChatType.Action)
        {
            if (PetServices.PetCastHelper.LastCastDealer is IPettablePet pet)
            {
                return HandleActionAsPet(pet);
            }
        
            if (PetServices.PetCastHelper.LastCastDealer is IPettableUser user)
            {
                return HandleActionAsUser(user);
            }
        }

        if (chatType == XivChatType.StandardEmote)
        {
            return HandleActionAsEmote(playerElement); 
        }
        
        if (chatType == XivChatType.SystemError || chatType == XivChatType.SystemMessage)
        {
            return HandleAsSystem(messageId);
        }
        
        return null;
    }
    
    private PetElement? HandleActionAsPet(IPettablePet pet)
    {
        if (pet.Owner == null)
        {
            return null;
        }
        
        _replaceNameType = NameType.Action;
        _replaceData = pet.PetData;
        
        return MakePetElement(pet.SkeletonId, MakePlayerChatElement(pet.Owner.DataBaseEntry.Name, pet.Owner.DataBaseEntry.Homeworld));
    }
    
    private PetElement? HandleActionAsUser(IPettableUser user)
    {
        IPetSheetData? petData = PetServices.PetSheets.GetPetFromAction((uint)PetServices.PetCastHelper.LastCastId);
        
        if (petData == null)
        {
            return null;
        }
        
        petData = PetServices.PetSheets.MakeSoft(user, petData);
        
        _replaceNameType = NameType.Action;
        _replaceData = petData;
        
        return MakePetElement(petData.Model, MakePlayerChatElement(user.DataBaseEntry.Name, user.DataBaseEntry.Homeworld));
    }
    
    private PetElement? HandleAsSystem(uint messageId)
    {
        // notebooks: 4500, 4504
        // logchats:  640, 642, 3840, 3841
        
        if (messageId != 640 && messageId != 642 && messageId != 3840 && messageId != 3841)
        {
            return null;
        }
        
        if (PetServices.UserList.LocalPlayer == null)
        {
            return null;
        }
        
        IPettablePet? pettablePet = PetServices.UserList.LocalPlayer.GetYoungestPet(SkeletonType.BattlePet);
        
        if (pettablePet == null)
        {
            return null;
        }
        
        _replaceNameType = NameType.Raw;
        _replaceData = pettablePet.PetData;
        
        return MakePetElement(pettablePet.SkeletonId, MakePlayerChatElement(PetServices.UserList.LocalPlayer.DataBaseEntry.Name, PetServices.UserList.LocalPlayer.DataBaseEntry.Homeworld));
    }
    
    private PetElement? HandleActionAsEmote(PlayerElement? playerElement)
    {   
        if (playerElement == null)
        {
            return null;
        }
        
        if (playerElement.ElementType != ElementType.StrongElement)
        {
            return null;
        }
        
        IPettableUser? user = PetServices.UserList.GetUserFromContentId(playerElement.ContentId);
        
        if (user == null)
        {
            return null;
        }
        
        IPettableEntity? target = PetServices.TargetManager.GetLeadingTarget(user);
        
        if (target is not IPettablePet pet)
        {
            return null;
        }
        
        if (pet.Owner == null)
        {
            return null;
        }
        
        _replaceNameType = NameType.Pronoun;
        _replaceData     = pet.PetData;
        
        return MakePetElement(pet.SkeletonId, MakePlayerChatElement(pet.Owner.DataBaseEntry.Name, pet.Owner.DataBaseEntry.Homeworld));
    }
    
    private PlayerElement? ParsePlayer(ILogMessageEntity? logMessageEntity)
    {
        if (logMessageEntity == null)
        {
            return null;
        }
        
        if (!logMessageEntity.IsPlayer)
        {
            return null;
        }
        
        string playerName = logMessageEntity.Name.ExtractText();
        ushort homeworld  = logMessageEntity.HomeWorldId;
            
        return MakePlayerChatElement(playerName, homeworld);
    }
    
    private PlayerElement? FindPlayerElement(string playerName, ushort homeworld)
    {
        foreach (PlayerElement player in _players)
        {
            if (homeworld != player.Homeworld)
            {
                continue;
            }
            
            if (!string.Equals(playerName, player.PlayerName, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }
            
            return player;
        }
        
        return null;
    }
    
    private PetElement? FindPetElement(PetSkeleton id, PlayerElement owner)
    {
        foreach (PetElement pet in _pets)
        {
            if (id != pet.Pet)
            {
                continue;
            }
            
            if (owner.Homeworld != pet.Owner.Homeworld)
            {
                continue;
            }
            
            if (!string.Equals(owner.PlayerName, pet.Owner.PlayerName, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }
            
            return pet;           
        }
        
        return null;
    }
    
    private PlayerElement MakePlayerChatElement(string playerName, ushort homeworld)
    {
        PlayerElement? foundElement = FindPlayerElement(playerName, homeworld);
        
        IPettableDatabaseEntry? entry = Database.GetEntry(playerName, homeworld, false);
            
        if (entry != null)
        {
            foundElement ??= new PlayerElement(entry.ContentId, entry.Name, entry.Homeworld);
            
            foundElement.MakeStrong(entry.ContentId);
        }
            
        foundElement ??= new PlayerElement(playerName, homeworld);
        
        foundElement.UpdatePlayerData(playerName, homeworld);
        
        _players.Remove(foundElement);
        _players.Add(foundElement);
        
        return foundElement;
    }
    
    private PetElement MakePetElement(PetSkeleton petSkeleton, PlayerElement owner)
    {
        PetElement? foundElement = FindPetElement(petSkeleton, owner);
        
        foundElement ??= new PetElement(petSkeleton, owner);
        
        _pets.Remove(foundElement);
        _pets.Add(foundElement);
        
        return foundElement;
    }
    
    private nint ClearLogDetour(LogModule* logModule)
    {
        if (PetServices.Configuration.debugModeActive)
        {
            PetServices.PetLog.LogWarning($"Clear LogModule.");
        }
        
        _pets.Clear();
        _players.Clear();
        _chats.Clear();    
        
        _lastIndex = -1;
        
        return ClearLogHook!.OriginalDisposeSafe(logModule);
    }
    
    private nint GetLogMessageRawDetour(LogModule* logModule, int index, nint unk3)
    {
        _lastIndex = index;
        
        nint returner = GetLogMessageRawHook!.OriginalDisposeSafe(logModule, index, unk3);

        return returner;
    }
    
    private ChatElement? GetChatElement(int id)
    {
        int chatsLength = _chats.Count;
        
        for (int i = 0; i < chatsLength; i++)
        {
            ChatElement currentElement = _chats[i];
            
            if (currentElement.MessageId != id)
            {
                continue;
            }
            
            return currentElement;
        }
        
        return null;
    }
    
    private uint FormatLogMessageDetour(RaptureLogModule* thisPtr, uint logKindId, Utf8String* sender, Utf8String* message, int* timestamp, void* a6, Utf8String* a7, int chatTabIndex)
    {
        if (_myCall)
        {
            return FormatLogMessageHook.OriginalDisposeSafe(thisPtr, logKindId, sender, message, timestamp, a6, a7, chatTabIndex);
        }
        
        if (PetServices.Configuration.debugModeActive)
        {
            PetServices.PetLog.Log($"Trying to handle LogMessage for index: '{_lastIndex}'.");
        }
        
        ChatElement? chatElement = GetChatElement(_lastIndex);
        
        if (chatElement == null)
        {
            return FormatLogMessageHook.OriginalDisposeSafe(thisPtr, logKindId, sender, message, timestamp, a6, a7, chatTabIndex);
        }
        
        PetElement? pet = chatElement.TargetPet ?? chatElement.SourcePet;
        
        if (pet == null)
        {
            return FormatLogMessageHook.OriginalDisposeSafe(thisPtr, logKindId, sender, message, timestamp, a6, a7, chatTabIndex);
        }
        
        IPettableUser? user = PetServices.UserList.GetUser(pet.Owner.PlayerName, pet.Owner.Homeworld);
        
        if (user == null)
        {
            return FormatLogMessageHook.OriginalDisposeSafe(thisPtr, logKindId, sender, message, timestamp, a6, a7, chatTabIndex);
        }
        
        PetServices.PetLog.LogFatal("FOUND RIGHT CHAT ELEMENT: " + chatElement);
        
        using Utf8String editableString = new Utf8String();
        
        editableString.Copy(message);
        
        SeString editableSeString = SeString.Parse(editableString.AsReadOnlySeString());
        
        if (!PetServices.StringHelper.ReplaceSeString(PetServices.Configuration.ShowInBattleChatColour, ref editableSeString, pet.Pet, chatElement.ReplaceString, user))
        {
            return FormatLogMessageHook.OriginalDisposeSafe(thisPtr, logKindId, sender, message, timestamp, a6, a7, chatTabIndex);
        }
        
        using Utf8String finalString = new Utf8String(editableSeString.EncodeWithNullTerminator());
        
        _myCall = true;
        
        uint returner = FormatLogMessageHook.OriginalDisposeSafe(thisPtr, logKindId, sender, &finalString, timestamp, a6, a7, chatTabIndex);
        
        _myCall = false;
        
        return returner;
    }
    
    
    
    enum ElementType
    {
        WeakElement,
        StrongElement,
    }
    
    enum ChatKind
    {
        Log,
        Chat,
    }
    
    class ChatElement
    {
        public uint           MessageId     { get; private set; }
        public XivChatType    ChatType      { get; private set; }
        public string         ReplaceString { get; private set; } = string.Empty;
        public PlayerElement? SourcePlayer  { get; private set; }
        public PlayerElement? TargetPlayer  { get; private set; }
        public PetElement?    SourcePet     { get; private set; }
        public PetElement?    TargetPet     { get; private set; }
        
        public ChatElement(uint messageId, XivChatType chatType, PlayerElement? sourcePlayer, PlayerElement? targetPlayer, PetElement? sourcePet, PetElement? targetPet)
        {
            MessageId     = messageId;
            ChatType      = chatType;
            SourcePlayer  = sourcePlayer;
            TargetPlayer  = targetPlayer;
            SourcePet     = sourcePet;
            TargetPet     = targetPet;
        }
        
        public ChatElement(uint messageId, XivChatType chatType, string replaceString, PlayerElement? sourcePlayer, PlayerElement? targetPlayer, PetElement? sourcePet, PetElement? targetPet)
            : this (messageId, chatType, sourcePlayer, targetPlayer, sourcePet, targetPet)
        {
            ReplaceString = replaceString;
        }
        
        public void SetReplaceString(string replaceString)
            => ReplaceString = replaceString;
        
        public override string ToString()
            => $"ChatElement: {MessageId}, {ChatType}, ['{ReplaceString}'], [{SourcePlayer?.PlayerName}@{SourcePlayer?.Homeworld}], [{TargetPlayer?.PlayerName}@{TargetPlayer?.Homeworld}], [{SourcePet?.Pet}: {SourcePet?.Owner.PlayerName}@{SourcePet?.Owner.Homeworld}], [{TargetPet?.Pet}: {TargetPet?.Owner.PlayerName}@{TargetPet?.Owner.Homeworld}].";
    }
    
    class PlayerElement
    {
        public ElementType ElementType { get; private set; }
        public string      PlayerName  { get; private set; }
        public ushort      Homeworld   { get; private set; }
        public ulong       ContentId   { get; private set; }
        
        public PlayerElement(string playerName, ushort homeworld)
        {
            ElementType = ElementType.WeakElement;
            ContentId   = 0;
            PlayerName  = playerName;
            Homeworld   = homeworld;
        }
        
        public PlayerElement(ulong contentId, string playerName, ushort homeworld)
            : this(playerName, homeworld)
        {
            ElementType = ElementType.StrongElement;
            ContentId   = contentId;
        }
        
        public void MakeStrong(ulong contentId)
        {
            ElementType = ElementType.StrongElement;
            ContentId   = contentId;
        }
        
        public void UpdatePlayerData(string playerName, ushort homeworld)
        {
            PlayerName = playerName;
            Homeworld  = homeworld;
        }
    }
    
    class PetElement
    {
        public PetSkeleton   Pet   { get; private set; }
        public PlayerElement Owner { get; private set; }
        
        public PetElement(PetSkeleton pet, PlayerElement owner)
        {
            Pet   = pet;
            Owner = owner;
        }
    }
}