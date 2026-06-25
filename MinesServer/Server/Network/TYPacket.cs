using MinesServer.Network.Constraints;
using MinesServer.Network.TypicalEvents;
using MinesServer.Server.Network.TypicalEvents;
using System.Runtime.InteropServices;
using System.Text;

namespace MinesServer.Network
{
    public readonly record struct TYPacket(uint EventTime, uint X, uint Y, ITypicalPacket Data) : ITopLevelPacket, IDataPart<TYPacket>
    {
        const int eventTypeLength = 4;
        const int eventTimeLength = sizeof(int);
        const int eventLocationLength = sizeof(uint);

        public const string packetName = "TY";
        public string PacketName => packetName;
        public string EventType => Data.PacketName;

        private static readonly Dictionary<string, PacketDecoder> DecoderMap = CreateDecoderMap();
        private static readonly Encoding Utf8 = Encoding.UTF8;

        private static Dictionary<string, PacketDecoder> CreateDecoderMap()
        {
            return new Dictionary<string, PacketDecoder>
            {
                { XmovPacket.packetName, x => XmovPacket.Decode(x) },
                { XdigPacket.packetName, x => XdigPacket.Decode(x) },
                { XgeoPacket.packetName, x => XgeoPacket.Decode(x) },
                { XheaPacket.packetName, x => XheaPacket.Decode(x) },
                { XbldPacket.packetName, x => XbldPacket.Decode(x) },
                { LoclPacket.packetName, x => LoclPacket.Decode(x) },
                { WhoiPacket.packetName, x => WhoiPacket.Decode(x) },
                { TADGPacket.packetName, x => TADGPacket.Decode(x) },
                { GUI_Packet.packetName, x => GUI_Packet.Decode(x) },
                { CpriPacket.packetName, x => CpriPacket.Decode(x) },
                { ChooPacket.packetName, x => ChooPacket.Decode(x) },
                { CmenPacket.packetName, x => CmenPacket.Decode(x) },
                { CsetPacket.packetName, x => CsetPacket.Decode(x) },
                { ChatPacket.packetName, x => ChatPacket.Decode(x) },
                { XhurPacket.packetName, x => XhurPacket.Decode(x) },
                { TAGRPacket.packetName, x => TAGRPacket.Decode(x) },
                { FINVPacket.packetName, x => FINVPacket.Decode(x) },
                { INVNPacket.packetName, x => INVNPacket.Decode(x) },
                { INUSPacket.packetName, x => INUSPacket.Decode(x) },
                { RESPPacket.packetName, x => RESPPacket.Decode(x) },
                { GDonPacket.packetName, x => GDonPacket.Decode(x) },
                { DPBXPacket.packetName, x => DPBXPacket.Decode(x) },
                { HelpPacket.packetName, x => HelpPacket.Decode(x) },
                { INCLPacket.packetName, x => INCLPacket.Decode(x) },
                { SettPacket.packetName, x => SettPacket.Decode(x) },
                { pRSTPacket.packetName, x => pRSTPacket.Decode(x) },
                { BldsPacket.packetName, x => BldsPacket.Decode(x) },
                { ClanPacket.packetName, x => ClanPacket.Decode(x) },
                { MisoPacket.packetName, x => MisoPacket.Decode(x) },
                { PDELPacket.packetName, x => PDELPacket.Decode(x) },
                { PCOPPacket.packetName, x => PCOPPacket.Decode(x) },
                { PRENPacket.packetName, x => PRENPacket.Decode(x) },
                { THIDPacket.packetName, x => THIDPacket.Decode(x) },
                { RndmPacket.packetName, x => RndmPacket.Decode(x) },
                { PopePacket.packetName, x => PopePacket.Decode(x) },
                { PROGPacket.packetName, x => PROGPacket.Decode(x) },
                { MissPacket.packetName, x => MissPacket.Decode(x) },
                { ChinPacket.packetName, x => ChinPacket.Decode(x) },
                { ADMNPacket.packetName, x => ADMNPacket.Decode(x) },
                { TAURPacket.packetName, x => TAURPacket.Decode(x) },
            };
        }

        private static PacketDecoder? GetDecoder(string packetName) =>
            DecoderMap.TryGetValue(packetName, out var decoder) ? decoder : null;

        public static TYPacket Decode(ReadOnlySpan<byte> input)
        {
            var caret = eventTypeLength;
            var eventType = Utf8.GetString(input[..caret]);
            var decoder = GetDecoder(eventType)
                ?? throw new InvalidPayloadException($"Invalid event type: {eventType}");

            var eventTime = MemoryMarshal.Read<uint>(input[caret..(caret += eventTimeLength)]);
            var x = MemoryMarshal.Read<uint>(input[caret..(caret += eventLocationLength)]);
            var y = MemoryMarshal.Read<uint>(input[caret..(caret += eventLocationLength)]);

            return new(eventTime, x, y, (ITypicalPacket)decoder(input[caret..]));
        }

        public int Encode(Span<byte> output)
        {
            if (EventType.Length != eventTypeLength)
                throw new InvalidPayloadException($"Invalid event type length: Expected {eventTypeLength} but got {EventType.Length}");

            if (GetDecoder(EventType) is null)
                throw new InvalidPayloadException($"Invalid event type: {EventType}");

            var bytesWritten = 0;

            // Записываем EventType
            bytesWritten += Utf8.GetBytes(EventType, output);

            // Записываем EventTime
            var et = EventTime;
            MemoryMarshal.Write(output[bytesWritten..], in et);
            bytesWritten += eventTimeLength;

            // Записываем X
            var tmpx = X;
            MemoryMarshal.Write(output[bytesWritten..], in tmpx);
            bytesWritten += eventLocationLength;

            // Записываем Y
            var tmpy = Y;
            MemoryMarshal.Write(output[bytesWritten..], in tmpy);
            bytesWritten += eventLocationLength;

            // Записываем данные
            bytesWritten += Data.Encode(output[bytesWritten..]);

            return bytesWritten;
        }

        public int Length => eventTypeLength + eventTimeLength + eventLocationLength * 2 + Data.Length;
    }
}