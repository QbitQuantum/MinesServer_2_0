using MinesServer.GameShit.Entities.PlayerStaff;

namespace MinesServer.GameShit.Programmator
{
    public class Program
    {
        private Program()
        {

        }
        public Program(Player owner, string name, string data)
        {
            this.owner = owner;
            this.name = name;
            this.data = data;
        }

        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string data { get; set; } = string.Empty;
        public Player? owner { get; set; } = null;

    }
}
