using MinesServer.GameShit.Entities.PlayerStaff;
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
        public virtual List<LineChat> messages { get; set; }
        public string Name { get; init; }
        public string tag { get; init; }
        public bool global { get; set; }
        public int? toplayer { get; set; }
        public int? owner { get; set; }

        public GCMessage[] GetMessages()
        {
            List<GCMessage> l = [];
            int startIndex = Math.Max(0, messages.Count - 30);
            for (int i = startIndex; i < messages.Count; i++)
            {
                var line = messages[i];
                l.Add(new GCMessage(line.id, line.player.cid, 1, line.time, line.player.name, line.message, line.player.id));
            }
            return l.ToArray();
        }

        public void AddMessage(Player p, string msg)
        {
            using var db = new DataBase();

            var line = new LineChat() { player = p, message = msg};

            db.Attach(this);
            messages.Add(line);
            db.SaveChanges();

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
