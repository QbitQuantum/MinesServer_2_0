using MinesServer.Utils;
using System.Text;

namespace MinesServer.Network.Chat
{
    public readonly record struct GCMessage(int LineId, int ClanId, int Color, int Time, string Nickname, string Text, int Gid) : IDataPart<GCMessage>
    {
        public string PacketName => throw new NotImplementedException();

        public int Length => 7 * 2 + Color.Digits() + ClanId.Digits() + Time.Digits() + Encoding.UTF8.GetByteCount(Nickname) + Encoding.UTF8.GetByteCount(Text) + LineId.Digits() + Gid.Digits();

        public static GCMessage Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var parts = Encoding.UTF8.GetString(decodeFrom).Split('±');
            if (parts.Length != 7) throw new InvalidPayloadException($"Expected {7} parts but got {parts.Length}");
            return new(
                int.Parse(parts[0]), // LineId
                int.Parse(parts[1]), // ClanId  
                int.Parse(parts[2]), // Color
                int.Parse(parts[3]), // Time
                parts[4],            // Nickname
                parts[5],            // Text
                int.Parse(parts[6])  // UserId
            );
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes($"{LineId}±{ClanId}±{Color}±{Time}±{Nickname}±{Text}±{Gid}", output);
    }
}