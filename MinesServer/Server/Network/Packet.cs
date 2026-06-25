using System.Runtime.InteropServices;
using System.Text;
using MinesServer.Network.Auth;
using MinesServer.Network.ConnectionStatus;
using MinesServer.Network.Constraints;

namespace MinesServer.Network
{
    public delegate IDataPartBase PacketDecoder(ReadOnlySpan<byte> decodeFrom);
    public readonly record struct Packet(string dataType, ITopLevelPacket data) : IDataPart<Packet>
    {
        const int dataTypeLength = sizeof(byte);
        const int eventTypeLength = sizeof(byte) * 2;
        const int lengthLength = sizeof(int);

        public string PacketName => throw new NotImplementedException();

        public string EventType => data.PacketName;

        private static PacketDecoder? GetDecoder(string packetName) => packetName switch
        {
            TYPacket.packetName => x => TYPacket.Decode(x), // TY
            AUPacket.packetName => x => AUPacket.Decode(x), // AU
            PongPacket.packetName => x => PongPacket.Decode(x), // PO
            _ => null
        };

        public int Encode(Span<byte> output)
        {
            if (EventType.Length != eventTypeLength) throw new InvalidPayloadException($"Invalid event type length: Expected {eventTypeLength} but got {EventType.Length}");
            var length = Length;
            MemoryMarshal.Write(output, in length);
            var bytesWritten = lengthLength;
            var caret = 0;
            bytesWritten += Encoding.UTF8.GetBytes(dataType, output[(caret += lengthLength)..]);
            bytesWritten += Encoding.UTF8.GetBytes(EventType, output[(caret += dataTypeLength)..]);
            bytesWritten += data.Encode(output[(caret += EventType.Length)..]);
            return bytesWritten;
        }

        public static Packet Decode(ReadOnlySpan<byte> input)
        {
            int packetLength = MemoryMarshal.Read<int>(input);
            if (packetLength != input.Length) throw new InvalidPayloadException($"Invalid packet length: Expected {packetLength} but got {input.Length}");
            var caret = lengthLength;
            var dataType = Encoding.UTF8.GetString(input[caret..(caret += dataTypeLength)]);
            var eventType = Encoding.UTF8.GetString(input[caret..(caret += eventTypeLength)]);
            var decoder = GetDecoder(eventType) ?? throw new InvalidPayloadException($"Invalid event type: {eventType}");
            return new(dataType, (ITopLevelPacket)decoder(input[caret..packetLength]));
        }
        public static bool TryDecode(ReadOnlySpan<byte> input,out Packet result)
        {
            result = default;
            int packetLength = MemoryMarshal.Read<int>(input);
            //if (packetLength != input.Length) return false;
            var caret = lengthLength;
            var dataType = Encoding.UTF8.GetString(input[caret..(caret += dataTypeLength)]);
            var eventType = Encoding.UTF8.GetString(input[caret..(caret += eventTypeLength)]);
            var decoder = GetDecoder(eventType);
            if (decoder is null) return false;
            result = new(dataType, (ITopLevelPacket)decoder(input[caret..packetLength]));
            return true;
        }

        public int Length => lengthLength + dataTypeLength + eventTypeLength + data.Length;
    }
}
