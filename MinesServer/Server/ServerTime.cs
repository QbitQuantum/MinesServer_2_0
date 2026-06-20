using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.WorldSystem;

namespace MinesServer.Server
{
    public class ServerTime : IDisposable
    {
        public static DateTime Now => DateTime.UtcNow;

        private readonly Thread _updateThread;
        private volatile bool _running = true;

        // Таймеры (последнее время выполнения)
        private DateTime _lastChunksUpdate = Now;
        private DateTime _lastWorldUpdate = Now;
        private DateTime _lastPlayersUpdate = Now;
        private DateTime _lastProgBotSpotUpdate = Now;
        private DateTime _lastOrdersUpdate = Now;
        private DateTime _lastActionsUpdate = Now;
        private DateTime _lastCommitWorld = Now;

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
                    _lastChunksUpdate = now;
                }

                // === Обновление мира (не чанков) — раз 50 миллисекунд ===
                if ((now - _lastWorldUpdate).TotalMilliseconds >= 50)
                {
                    World.Update();
                    _lastWorldUpdate = now;
                }

                if ((now - _lastPlayersUpdate).TotalMilliseconds >= 100)
                {
                    foreach (var player in DataBase.activeplayers)
                    {
                        player.Update();
                    }
                    _lastPlayersUpdate = now;
                }

                if ((now - _lastProgBotSpotUpdate).TotalMilliseconds >= 100)
                {
                    foreach (var botspot in DataBase.botspotplayer)
                    {
                        botspot.Update();
                    }
                    _lastProgBotSpotUpdate = now;
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

                // === Сохранение мира на диск — раз в минуту ===
                if ((now - _lastCommitWorld).TotalMinutes >= 1)
                {
                    World.CommitWorld();
                    _lastCommitWorld = now;
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
        public void Dispose()
        {
            _running = false;
            _updateThread?.Join(2000); // ждём до 2 сек
        }
    }
}