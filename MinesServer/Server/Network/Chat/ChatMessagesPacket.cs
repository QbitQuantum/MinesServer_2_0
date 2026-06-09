using MinesServer.Network.Constraints;
using SimpleJSON;
using System.Text;

namespace MinesServer.Network.Chat
{
    public readonly record struct ChatMessagesPacket(string Channel, GCMessage[] Messages) : ITopLevelPacket, IDataPart<ChatMessagesPacket>
    {
        public const string packetName = "mU";

        public string PacketName => packetName;

        public int Length => 16 + Encoding.Default.GetByteCount(Channel) + Messages.Sum(x => x.Length + 2) + Messages.Length - 1;

        public static ChatMessagesPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var obj = JSON.Parse(Encoding.UTF8.GetString(decodeFrom));
            var messages = new GCMessage[obj["h"].Count];
            for (int i = 0; i < messages.Length; i++)
                messages[i] = GCMessage.Decode(Encoding.UTF8.GetBytes(obj["h"][i].Value));
            return new(obj["ch"], messages);
        }

        public int Encode(Span<byte> output)
        {
            var messageStrings = new string[Messages.Length];
            for (int i = 0; i < Messages.Length; i++)
            {
                messageStrings[i] = $"" +
                    $"{Messages[i].LineId}±" +
                    $"{Messages[i].Color}±" +
                    $"{Messages[i].ClanId}±" +
                    $"{Messages[i].Time}±" +
                    $"{Messages[i].Nickname}±" +
                    $"{Messages[i].Text}±" +
                    $"{Messages[i].Gid}";
            }
            string _message = string.Join("\",\"", messageStrings);

            string json = $"{{\"ch\":\"{Channel}\",\"h\":[\"{_message}\"]}}";

            byte[] bytes = Encoding.UTF8.GetBytes(json);
            bytes.CopyTo(output);

            return bytes.Length;
        }

    }
}
