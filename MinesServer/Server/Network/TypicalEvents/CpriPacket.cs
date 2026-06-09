using MinesServer.Network.Constraints;
using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.TypicalEvents
{
    public readonly record struct CpriPacket(int LineId) : ITypicalPacket, IDataPart<CpriPacket>
    {

        public const string packetName = "Cpri";

        public string PacketName => packetName;

        public int Length => LineId.Digits();

        public static CpriPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(int.Parse(Encoding.UTF8.GetString(decodeFrom)));

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(LineId.ToString(), output);
    }
}
