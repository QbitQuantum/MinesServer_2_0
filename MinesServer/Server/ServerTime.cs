using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.WorldSystem;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace MinesServer.Server
{
    public class ServerTime : IDisposable
    {
        public delegate void GameAction();
        public Queue<(GameAction action,Player initiator)> gameActions;
        CancellationTokenSource s = new();
        public ServerTime()
        {
            Now = DateTime.Now;
            gameActions = new Queue<(GameAction,Player)>();
            StartTimeUpdate();
            StupidUpdate(() =>
            {
                for (int i = 0; i < gameActions.Count; i++)
                {
                    var item = gameActions.Dequeue();
                    /*try
                    {*/
                    if (item.action is not null)
                    {
                        item.action();
                    }
                    /*}
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{item.initiator.name}[{item.initiator.id}] caused {ex}");
                    }*/
                }
            },10);
            StupidUpdate(() =>
            {
                var players = DataBase.activeplayers;
                for (int i = 0; i < players.Count; i++)
                {
                    players[i]?.Update();
                }
            },10);
            StupidUpdate(() =>
            {
                using var db = new DataBase();
                foreach (var order in db.orders)
                {
                    order.CheckReady();
                }
                db.SaveChanges();
            },1000);
            ChunksUpdateSlised();
            programmatorUpdate();
        }
        private void StupidUpdate(Action a,double delay)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    a();
                    Thread.Sleep(TimeSpan.FromMilliseconds(delay));
                }
            },s.Token);
        }
        public void AddAction(GameAction action,Player p)
        {
            if (ServerTime.Now < directactiondelay) return;
            gameActions.Enqueue((action,p));
            directactiondelay = Now + TimeSpan.FromMicroseconds(5);
        }
        private DateTime directactiondelay = ServerTime.Now;
        public static int offset;
        public static DateTime Now { get; private set; }
        public void StartTimeUpdate()
        {

            Task.Run(() =>
            {
                while (true)
                {
                    var d = DateTime.Now;
                    offset = (int)(Now-d).TotalMicroseconds;
                    Now = d;
                    Thread.Sleep(TimeSpan.FromMicroseconds(50));
                }
            }, s.Token);
        }
        public void programmatorUpdate()
        {
            Task.Run(() =>
            {
                while(true)
                { 
                    var players = DataBase.activeplayers.Where(i => i.programsData.ProgRunning);
                    for (int i = 0; i < players.Count(); i++)
                    {
                        players.ElementAt(i)?.ProgrammatorUpdate();
                    }
                    Thread.Sleep(1);
                }
            }, s.Token);
        }
        public void ChunksUpdateSlised()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var first = Task.Run(() =>
                    {
                        for (int y = 0; y < World.ChunksH; y++)
                        {
                            for (int x = 0; x < World.ChunksW; x++)
                            {
                               if (x % 2 == 0) World.W.chunks[x, y].Update();
                                World.CommitWorld();
                            }
                        }
                    });
                    var second = Task.Run(() =>
                    {
                        for (int y = 0; y < World.ChunksH; y++)
                        {
                            for (int x = 0; x < World.ChunksW; x++)
                            {
                                if (x % 2 == 1) World.W.chunks[x, y].Update();
                                World.CommitWorld();
                            }
                        }
                    });

                    Task.WaitAll(first, second);
                    World.Update();
                    World.CommitWorld();
                    Thread.Sleep(1);
                }
            },s.Token);
        }
        public void Update()
        {
            if (!MServer.started)
                return;
            for (int i = 0; i < gameActions.Count; i++)
            {
                var item = gameActions.Dequeue();
                /*try
                {*/
                if (item.action != null)
                {
                    item.action();
                }
                /*}
                catch (Exception ex)
                {
                    Console.WriteLine($"{item.initiator.name}[{item.initiator.id}] caused {ex}");
                }*/
            }
            var players = DataBase.activeplayers;
            for (int i = 0; i < players.Count; i++)
            {
                players[i]?.Update();
            }
            using var db = new DataBase();
            foreach (var order in db.orders)
            {
                order.CheckReady();
            }
            db.SaveChanges();
            Thread.Sleep(1);
        }

        public void Dispose()
        {
            s.CancelAsync().Wait();
            s.Dispose();
        }
    }
}
