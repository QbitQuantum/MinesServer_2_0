using MinesServer.Network.Constraints;
using System.Text;
using System.Text.RegularExpressions;

namespace MinesServer.Network.TypicalEvents
{
    public readonly struct PROGPacket : ITypicalPacket, IDataPart<PROGPacket>
    {
        // TODO: Perhaps chenge this to an actual prgram type?
        public readonly byte[] program;
        public readonly (int id, string source) prog
        {
            get
            {
                int len = BitConverter.ToInt32(program[0..4]);
                int id = BitConverter.ToInt32(program[4..8]);
                string source = Encoding.UTF8.GetString(program[(8 + len)..Length]);
                //source = Regex.Replace(source, @"[\u0000-\u0008\u000A-\u001F\u0100-\uFFFF]", "");
                return (id, source);
            }
        }

        public const string packetName = "PROG";

        public string PacketName => packetName;

        public PROGPacket(byte[] program) => this.program = program;

        public int Length => program.Length;

        public static PROGPacket Decode(ReadOnlySpan<byte> decodeFrom) => new(decodeFrom.ToArray());

        public int Encode(Span<byte> output)
        {
            var span = program.AsSpan();
            span.CopyTo(output);
            return span.Length;
        }
    }
}
