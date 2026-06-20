using MinesServer.Network.Constraints;
using MinesServer.Network;
using System.Text;

namespace MinesServer.Server.Network
{
    public readonly record struct ConfigPacket(string content) : ITopLevelPacket, IDataPart<ConfigPacket>
    {
        public const string packetName = "#F";

        public string PacketName => packetName;

        public int Length => Encoding.UTF8.GetByteCount(content);

        public static ConfigPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(Encoding.UTF8.GetString(decodeFrom));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(content, output);
    }
}
