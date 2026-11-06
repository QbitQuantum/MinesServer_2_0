using MinesServer.Network.Constraints;

namespace MinesServer.Network.World
{
    /// <summary>
    /// Auto-rem status (BR) — "0" or "1"
    /// </summary>
    public readonly struct AutoRemPacket : ITopLevelPacket, IDataPart<AutoRemPacket>
    {
        public readonly bool isEnabled;

        public const string packetName = "BR";

        public string PacketName => packetName;

        public AutoRemPacket(bool isEnabled) => this.isEnabled = isEnabled;

        public int Length => 1;

        public static AutoRemPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            if (!decodeFrom.SequenceEqual([(byte)'0']) && !decodeFrom.SequenceEqual([(byte)'1'])) throw new InvalidPayloadException("Payload does not match");
            return new(decodeFrom[0] == (byte)'1');
        }

        public int Encode(Span<byte> output)
        {
            Span<byte> span = isEnabled ? [(byte)'1'] : [(byte)'0'];
            span.CopyTo(output);
            return span.Length;
        }
    }
}
