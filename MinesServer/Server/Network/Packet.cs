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
        const int minPacketHeaderSize = lengthLength + dataTypeLength + eventTypeLength;

        private static readonly Encoding Utf8 = Encoding.UTF8;

        // Статический кэш декодеров (инициализируется один раз при загрузке класса)
        private static readonly Dictionary<string, PacketDecoder> DecoderMap = CreateDecoderMap();

        public string PacketName => throw new NotImplementedException();

        public string EventType => data.PacketName;

        public int Length => lengthLength + dataTypeLength + eventTypeLength + data.Length;

        /// <summary>
        /// Инициализирует словарь декодеров
        /// </summary>
        private static Dictionary<string, PacketDecoder> CreateDecoderMap()
        {
            var decoders = new Dictionary<string, PacketDecoder>
            {
                { TYPacket.packetName,      x => TYPacket.Decode(x) },      // TY
                { AUPacket.packetName,      x => AUPacket.Decode(x) },      // AU
                { PongPacket.packetName,    x => PongPacket.Decode(x) },    // PO
            };
            return decoders;
        }

        /// <summary>
        /// Получает декодер из кэша O(1) вместо switch выражения
        /// </summary>
        private static PacketDecoder? GetDecoder(string packetName) =>
            DecoderMap.TryGetValue(packetName, out var decoder) ? decoder : null;

        public int Encode(Span<byte> output)
        {
            // Валидация длины EventType
            if (EventType.Length != eventTypeLength)
                throw new InvalidPayloadException($"Invalid event type length: Expected {eventTypeLength} but got {EventType.Length}");

            // Проверка размера буфера
            var length = Length;
            if (output.Length < length)
                throw new ArgumentException($"Output buffer is too small. Required: {length}, available: {output.Length}", nameof(output));

            int bytesWritten = 0;

            // Записываем длину пакета
            MemoryMarshal.Write(output, in length);
            bytesWritten += lengthLength;

            // Записываем dataType
            bytesWritten += Utf8.GetBytes(dataType, output[bytesWritten..]);

            // Записываем EventType
            bytesWritten += Utf8.GetBytes(EventType, output[bytesWritten..]);

            // Записываем данные пакета
            bytesWritten += data.Encode(output[bytesWritten..]);

            return bytesWritten;
        }

        public static Packet Decode(ReadOnlySpan<byte> input)
        {
            if (!TryDecode(input, out var result))
                throw new InvalidPayloadException("Invalid packet");
            return result;
        }

        public static bool TryDecode(ReadOnlySpan<byte> input, out Packet result)
        {
            result = default;

            if (input.Length < minPacketHeaderSize)
                return false;

            int packetLength = MemoryMarshal.Read<int>(input);

            if (packetLength < minPacketHeaderSize || packetLength > input.Length)
                return false;

            return TryDecodeInternal(input[..packetLength], out result);
        }

        private static bool TryDecodeInternal(ReadOnlySpan<byte> input, out Packet result)
        {
            result = default;
            int caret = lengthLength;

            if (input.Length < caret + dataTypeLength)
                return false;

            var dataType = Utf8.GetString(input[caret..(caret += dataTypeLength)]);

            if (input.Length < caret + eventTypeLength)
                return false;

            var eventType = Utf8.GetString(input[caret..(caret += eventTypeLength)]);

            var decoder = GetDecoder(eventType);
            if (decoder == null)
                return false;

            try
            {
                var payloadData = (ITopLevelPacket)decoder(input[caret..]);
                result = new(dataType, payloadData);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}