using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using STS2RitsuLib.Networking.MessageExtensions;
using STS2RitsuLib.Patching.Core;
using STS2RitsuLib.Patching.Models;
using STS2_WineFox.Settings;

namespace STS2_WineFox.Network;

internal static class WineFoxStandeeWire
{
    private const string ExtensionId = "sts2.winefox.standee";
    private const int ExtensionVersion = 1;

    internal static readonly AccessTools.FieldRef<NetMessageBus, PacketWriter> WriterRef =
        AccessTools.FieldRefAccess<NetMessageBus, PacketWriter>("_writer");

    internal static readonly AccessTools.FieldRef<NetMessageBus, PacketReader> ReaderRef =
        AccessTools.FieldRefAccess<NetMessageBus, PacketReader>("_reader");

    private static readonly Lock Gate = new();
    private static bool _registered;

    [ThreadStatic] private static ulong _senderId;

    internal static ulong CurrentSenderId => _senderId;

    public static void RegisterExtensions()
    {
        lock (Gate)
        {
            if (_registered)
                return;

            RitsuNetMessageTailExtensions.RegisterBytes<ClientLobbyJoinRequestMessage>(
                ExtensionId,
                ExtensionVersion,
                static _ => WineFoxStandeeSync.EncodeLocal(),
                static (version, payload) => WineFoxStandeeSync.ApplyPayload(version, payload));

            RitsuNetMessageTailExtensions.RegisterBytes<ClientLobbyJoinResponseMessage>(
                ExtensionId,
                ExtensionVersion,
                static _ => WineFoxStandeeSync.EncodeAll(),
                static (version, payload) => WineFoxStandeeSync.ApplyPayload(version, payload));

            RitsuNetMessageTailExtensions.RegisterBytes<PlayerJoinedMessage>(
                ExtensionId,
                ExtensionVersion,
                static _ => WineFoxStandeeSync.EncodeLocal(),
                static (version, payload) => WineFoxStandeeSync.ApplyPayload(version, payload));

            RitsuNetMessageTailExtensions.RegisterBytes<LobbyBeginRunMessage>(
                ExtensionId,
                ExtensionVersion,
                static _ => WineFoxStandeeSync.EncodeAll(),
                static (version, payload) => WineFoxStandeeSync.ApplyPayload(version, payload));

            _registered = true;
        }
    }

    public static bool ApplyDynamicPatches(ModPatcher patcher)
    {
        RegisterExtensions();

        var serializeDefinition = AccessTools.DeclaredMethod(typeof(NetMessageBus), nameof(NetMessageBus.SerializeMessage));
        if (serializeDefinition is not { IsGenericMethodDefinition: true })
            return false;

        return patcher.ApplyDynamicPatches(
            [
                CreateSerializePatch("winefox_standee_join_request_tail", typeof(ClientLobbyJoinRequestMessage),
                    typeof(JoinRequestTail), serializeDefinition),
                CreateSerializePatch("winefox_standee_join_response_tail", typeof(ClientLobbyJoinResponseMessage),
                    typeof(JoinResponseTail), serializeDefinition),
                CreateSerializePatch("winefox_standee_player_joined_tail", typeof(PlayerJoinedMessage),
                    typeof(PlayerJoinedTail), serializeDefinition),
                CreateSerializePatch("winefox_standee_begin_run_tail", typeof(LobbyBeginRunMessage),
                    typeof(BeginRunTail), serializeDefinition),
            ],
            rollbackOnCriticalFailure: false);
    }

    private static DynamicPatchInfo CreateSerializePatch(
        string id,
        Type messageType,
        Type patchType,
        MethodInfo serializeDefinition)
    {
        var postfix = AccessTools.DeclaredMethod(patchType, "Postfix")
                      ?? throw new MissingMethodException(patchType.FullName, "Postfix");

        return new DynamicPatchInfo(
            id,
            serializeDefinition.MakeGenericMethod(messageType),
            postfix: new HarmonyMethod(postfix),
            description: $"Append the WineFox standee variant tail to {messageType.Name}");
    }

    private static void FinishTail<T>(NetMessageBus bus, ulong senderId, T message, ref int length,
        ref byte[] result)
        where T : INetMessage
    {
        var writer = WriterRef(bus);
        _senderId = senderId;
        try
        {
            RitsuNetMessageTailExtensions.Write(writer, message);
        }
        finally
        {
            _senderId = 0UL;
        }

        length = checked((int)(((long)writer.BitPosition + 7) / 8));
        result = writer.Buffer;
    }

    internal static class JoinRequestTail
    {
        internal static void Postfix(NetMessageBus __instance, ulong senderId, ClientLobbyJoinRequestMessage message,
            ref int length, ref byte[] __result)
        {
            FinishTail(__instance, senderId, message, ref length, ref __result);
        }
    }

    internal static class PlayerJoinedTail
    {
        internal static void Postfix(NetMessageBus __instance, ulong senderId, PlayerJoinedMessage message,
            ref int length, ref byte[] __result)
        {
            FinishTail(__instance, senderId, message, ref length, ref __result);
        }
    }

    internal static class JoinResponseTail
    {
        internal static void Postfix(NetMessageBus __instance, ulong senderId, ClientLobbyJoinResponseMessage message,
            ref int length, ref byte[] __result)
        {
            FinishTail(__instance, senderId, message, ref length, ref __result);
        }
    }

    internal static class BeginRunTail
    {
        internal static void Postfix(NetMessageBus __instance, ulong senderId, LobbyBeginRunMessage message,
            ref int length, ref byte[] __result)
        {
            FinishTail(__instance, senderId, message, ref length, ref __result);
        }
    }
}

public sealed class WineFoxStandeeTailReadPatch : IPatchMethod
{
    public static string PatchId => "winefox_standee_tail_read";
    public static bool IsCritical => false;

    public static string Description => "Read the WineFox standee variant tail from lobby messages";

    public static ModPatchTarget[] GetTargets()
    {
        return
        [
            new(typeof(NetMessageBus), nameof(NetMessageBus.TryDeserializeMessage),
                [typeof(byte[]), typeof(INetMessage).MakeByRefType(), typeof(ulong?).MakeByRefType()])
        ];
    }

    public static void Postfix(NetMessageBus __instance, bool __result, ref INetMessage? message)
    {
        if (!__result || message == null)
            return;

        var reader = WineFoxStandeeWire.ReaderRef(__instance);

        switch (message)
        {
            case ClientLobbyJoinRequestMessage:
                RitsuNetMessageTailExtensions.Read<ClientLobbyJoinRequestMessage>(reader);
                break;
            case ClientLobbyJoinResponseMessage:
                RitsuNetMessageTailExtensions.Read<ClientLobbyJoinResponseMessage>(reader);
                break;
            case PlayerJoinedMessage:
                RitsuNetMessageTailExtensions.Read<PlayerJoinedMessage>(reader);
                break;
            case LobbyBeginRunMessage:
                RitsuNetMessageTailExtensions.Read<LobbyBeginRunMessage>(reader);
                break;
        }
    }
}
