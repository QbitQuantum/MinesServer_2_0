namespace MinesServer.Server
{
    public class TickAction
    {
        public TickAction(Action a,string n) { body = a;name = n; }
        private Action body;
        string name;
        public void Call()
        {
            if (completed)
            {
                completed = false;
                Task.Run(() =>
                {
                   body();
                   completed = true;
                });
            }
        }
        private bool completed = true;
    }
}
