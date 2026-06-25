namespace MinesServer.Server
{
    public class Config
    {
        public Config(string WorldName) { 
            this.WorldName = WorldName;
        }
        public readonly string WorldName = "MinesServer";
    }
}
