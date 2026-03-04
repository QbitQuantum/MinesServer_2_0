using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.WorldSystem;
using System;
using System.Threading;

namespace MinesServer.Server
{
    public class ServerTime : IDisposable
    {
        private readonly Thread _updateThread;
        private volatile bool _running = true;

        // Таймеры (последнее время выполнения)
        private DateTime _lastChunksUpdate = DateTime.UtcNow;
        private DateTime _lastPlayersUpdate = DateTime.UtcNow;
        private DateTime _lastProgUpdate = DateTime.UtcNow;
        private DateTime _lastOrdersUpdate = DateTime.UtcNow;
        private DateTime _lastActionsUpdate = DateTime.UtcNow;

        // Очередь действий
        public readonly Queue<(Action action, Player initiator)> gameActions = new();

        private DateTime _directActionDelay = DateTime.MinValue;

        public ServerTime()
        {
            _updateThread = new Thread(RunUpdateLoop)
            {
                IsBackground = true,
                Name = "Server Update Loop"
            };
            _updateThread.Start();
        }

        private void RunUpdateLoop()
        {
            while (_running)
            {
                var now = DateTime.UtcNow;

                // === Обновление мира (чанков) — раз в секунду ===
                if ((now - _lastChunksUpdate).TotalSeconds >= 1)
                {
                    World.ChunkUpdate();
                    World.Update();
                    World.CommitWorld();
                    _lastChunksUpdate = now;
                }

                // === Обновление игроков — 10 раз в секунду ===
                if ((now - _lastPlayersUpdate).TotalMilliseconds >= 100)
                {
                    foreach (var player in DataBase.activeplayers)
                    {
                        player?.Update();
                    }
                    _lastPlayersUpdate = now;
                }

                // === Программаторы — 10 раз в секунду ===
                if ((now - _lastProgUpdate).TotalMilliseconds >= 100)
                {
                    var players = DataBase.activeplayers;
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (players[i]?.programsData.ProgRunning == true)
                        {
                            players[i].ProgrammatorUpdate();
                        }
                    }
                    _lastProgUpdate = now;
                }

                // === Заказы — раз в 5 секунд ===
                if ((now - _lastOrdersUpdate).TotalSeconds >= 5)
                {
                    using var db = new DataBase();
                    foreach (var order in db.orders)
                    {
                        order.CheckReady();
                    }
                    db.SaveChanges();
                    _lastOrdersUpdate = now;
                }

                // === Очередь действий — 10 раз в секунду ===
                if ((now - _lastActionsUpdate).TotalMilliseconds >= 100)
                {
                    while (gameActions.Count > 0)
                    {
                        var (action, initiator) = gameActions.Dequeue();
                        if (action != null)
                        {
                            try
                            {
                                action();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{initiator?.name}[{initiator?.id}] caused {ex}");
                            }
                        }
                    }
                    _lastActionsUpdate = now;
                }

                // Небольшая пауза, чтобы не грузить CPU на 100%
                Thread.Sleep(1);
            }
        }

        public void AddAction(Action action, Player p)
        {
            if (DateTime.UtcNow < _directActionDelay) return;
            gameActions.Enqueue((action, p));
            _directActionDelay = DateTime.UtcNow.AddMicroseconds(5);
        }

        public static DateTime Now => DateTime.UtcNow;

        public void Dispose()
        {
            _running = false;
            _updateThread?.Join(2000); // ждём до 2 сек
        }
    }
}