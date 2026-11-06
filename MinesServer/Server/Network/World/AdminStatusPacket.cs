using MinesServer.Network.Constraints;

namespace MinesServer.Network.World
{
    /// <summary>
    /// Admin/ban hammer status (SU) — "0" or "1"
    /// </summary>
    public readonly struct AdminStatusPacket : ITopLevelPacket, IDataPart<AdminStatusPacket>
    {
        public readonly bool isEnabled;

        public const string packetName = "SU";

        public string PacketName => packetName;

        public AdminStatusPacket(bool isEnabled) => this.isEnabled = isEnabled;

        public int Length => 1;

        public static AdminStatusPacket Decode(ReadOnlySpan<byte> decodeFrom)
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
