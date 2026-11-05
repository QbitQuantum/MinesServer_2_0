using MinesServer.GameShit.Entities;
using MinesServer.Network.Chat;
using MinesServer.Server;

namespace MinesServer.GameShit.GChat
{
    public class Chat
    {
        private Chat() { }

        public Chat(string tag, string name)
        {
            this.tag = tag;
            this.Name = name;
            this.global = true;
            this.messages = [];
        }

        public int id { get; set; }
        public List<LineChat> messages { get; set; } = [];
        public string Name { get; init; } = "";
        public string tag { get; init; } = "";
        public bool global { get; set; }
        public int? toplayer { get; set; }
        public int? owner { get; set; }

        public static void CreateChat(string tag, string name)
        {
            using var db = new DataBase();
            db.chats.Add(new Chat(tag, name));
            db.SaveChanges();

        }
        public GCMessage[] GetMessages()
        {
            var sorted = new List<LineChat>(messages);
            sorted.Sort((a, b) => a.id.CompareTo(b.id));

            var result = new GCMessage[sorted.Count];
            for (int i = 0; i < sorted.Count; i++)
            {
                var line = sorted[i];
                result[i] = new GCMessage(
                    line.id, line.playercid,
                    1, line.time, line.playername,
                    line.message, line.playerid);
            }
            return result;
        }

        public void AddMessage(Player p, string msg)
        {
            using var db = new DataBase();

            var line = new LineChat
            {
                playerid = p.id,
                playername = p.name,
                playercid = p.cid,
                message = msg
            };

            db.Set<LineChat>().Add(line);
            db.SaveChanges();

            messages.Add(line);

            if (global)
            {
                var packet = new ChatMessagesPacket(tag,
                    [new GCMessage(line.id, p.cid, 1, line.time, p.name, msg, p.id)]);

                foreach (var i in DataBase.activeplayers)
                    i.connection?.SendU(packet);
            }
        }
    }
}