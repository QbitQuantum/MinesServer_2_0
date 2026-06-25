using System.Buffers;
using System.Security.Cryptography;
using MinesServer.GameShit;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network;
using MinesServer.Network.Auth;
using MinesServer.Network.Chat;
using MinesServer.Network.ConnectionStatus;
using MinesServer.Network.Constraints;
using MinesServer.Network.GUI;
using MinesServer.Network.TypicalEvents;
using MinesServer.Network.World;
using MinesServer.Server.Network.TypicalEvents;
using NetCoreServer;

namespace MinesServer.Server
{
    public class Session : TcpSession
    {
        private const int SessionIdLength = 5;
        private const int PingDelayMs = 200;
        private const int PingOffset = 201;
        private static readonly char[] _idChars = "abcdefghijklmnoprtsuxyz0123456789".ToCharArray();

        private readonly ServerTime _serverTime;
        private bool _isCompleted = false;
        private string _sessionId = string.Empty;
        private DateTime _lastPong = ServerTime.Now;
        private int _nextExpected = 0;
        private Player? _player = null;
        private Auth? _auth = null;

        public Session(TcpServer server, ServerTime serverTime) : base(server)
        {
            _serverTime = serverTime;
        }

        // Безопасная генерация id
        private static string GenerateSessionId()
        {
            Span<char> buf = stackalloc char[SessionIdLength];
            for (int i = 0; i < SessionIdLength; i++)
            {
                int idx = RandomNumberGenerator.GetInt32(_idChars.Length);
                buf[i] = _idChars[idx];
            }
            return new string(buf);
        }

        #region server handlers

        protected override void OnConnected()
        {
            _sessionId = GenerateSessionId();
            Console.WriteLine($"Connected: {Socket.RemoteEndPoint}");
            // Нельзя await тут, поэтому отправляем fire-and-forget безопасно
            _ = SafeSendUAsync(new StatusPacket("твоей жопе"));
            _ = SafeSendUAsync(new AUPacket(_sessionId));
            _ = SafeSendUAsync(new PingPacket(0, 0, ""));
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            if (!Packet.TryDecode(buffer, out var result))
                return;

            try
            {
                switch (result.data)
                {
                    case AUPacket au:
                        HandleAU(au);
                        break;

                    case TYPacket ty:
                        _serverTime.AddAction(() => HandleTY(ty), _player);
                        break;

                    case PongPacket pong:
                        HandlePong(pong);
                        break;

                    default:
                        // Неизвестный верхнеуровневый пакет — игнорируем
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"invalid packet from {_player?.id}: {ex}");
            }
        }

        protected override void OnDisconnected()
        {
            if (_player is null) return;
            Console.WriteLine($"Disconnected: id:{_player.id} name:{_player.name}");
            try
            {
                _player.dOnDisconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during player disconnect: {ex}");
            }
            _player = null;
            Dispose();
        }

        #endregion

        #region handlers

        private void HandleAU(AUPacket p)
        {
            if (_isCompleted)
            {
                // Если сессия уже завершена — игнорируем AU и закрываем окошко
                _auth = null;
                CloseWindow();
                return;
            }

            _auth = new Auth(this);
            _auth.TryToAuthenticate(p, _sessionId);
        }

        private void HandleTY(TYPacket packet)
        {
            // pattern matching внутри switch — аккуратно и читабельно
            switch (packet.Data)
            {
                case XmovPacket xmov: MoveHandler(packet, xmov); break;
                case LoclPacket locl: LocalChatHandler(packet, locl); break;
                case XbldPacket xbld: BuildHandler(packet, xbld); break;
                case XdigPacket xdig: DigHandler(packet, xdig); break;
                case XgeoPacket xgeo: GeoHandler(packet, xgeo); break;
                case WhoiPacket whoi: WhoisHandler(packet, whoi); break;
                case TADGPacket tadg: AutoDiggHandler(packet, tadg); break;
                case GUI_Packet gui_: GUI(packet, gui_); break;
                case INCLPacket incl: Incl(packet, incl); break;
                case INUSPacket inus: Inus(packet, inus); break;
                case DPBXPacket dpbx: Dpbx(packet, dpbx); break;
                case SettPacket sett: Sett(packet, sett); break;
                case ADMNPacket admn: ADMN(packet, admn); break;
                case RESPPacket res: Res(packet, res); break;
                case ClanPacket clan: Clan(packet, clan); break;
                case PopePacket pp: Pope(packet, pp); break;
                case PROGPacket prog: PROG(packet, prog); break;
                case PDELPacket pdel: Pdel(packet, pdel); break;
                case pRSTPacket prst: Prst(packet, prst); break;
                case PRENPacket pren: Pren(packet, pren); break;
                case ChatPacket chat: Chat(packet, chat); break;
                case INVNPacket invn: Invn(packet, invn); break;
                case XheaPacket xhea: Xhea(packet, xhea); break;
                case ChinPacket chin: Chin(packet, chin); break;
                case BldsPacket blds: Blds(packet, blds); break;
                case TAGRPacket agr: Agr(packet, agr); break;
                case CmenPacket cmen: Cmen(packet, cmen); break;
                case ChooPacket choo: Choo(packet, choo); break;
                case CpriPacket cpri: Cpri(packet, cpri); break;
                case TAURPacket taur: Taur(packet, taur); break;
                default:
                    // Invalid event type
                    break;
            }
        }

        private void Blds(TYPacket f, BldsPacket blds) => Task.Run(() => _player?.OpenMyBuildings());

        private void Agr(TYPacket f, TAGRPacket agr)
        {
            //changeAgr
        }

        private void Cmen(TYPacket f, CmenPacket cmen)
        {
            var chats = new List<GCChatEntry>();
            using var db = new DataBase();

            foreach (var chat in db.chats)
            {
                string lastMessageText = "";
                string lastMessageAuthor = "";

                if (chat.messages != null && chat.messages.Any())
                {
                    var last = chat.messages.Last();
                    lastMessageText = last.message ?? "";
                    lastMessageAuthor = last.player?.name ?? "";
                }

                chats.Add(new GCChatEntry(chat.tag, chat.Name, lastMessageAuthor, lastMessageText, false));
            }

            var packet = new ChatListPacket(chats.ToArray());
            _player?.connection?.SendU(packet);
        }

        private void Choo(TYPacket f, ChooPacket choo)
        {
            var selectChat = DataBase.GetChat(choo.tag);
            if (selectChat == null) return;
            _player!.currentchat = selectChat;
            _player.SendChat();
        }

        private void Cpri(TYPacket f, CpriPacket cpri)
        {
            var linechat = DataBase.GetLineChat(cpri.LineId);
            if (linechat == null) return;

            var isCurrentUser = linechat.playerid == _player!.id;
            var text = isCurrentUser ? "Избранное" : $"Написать сообщение игроку %{linechat.playerid}% {DataBase.NickName(linechat.playerid)}";

            var buttons = new List<MButton> { new MButton("Отправить сообщение", "SendMessage") };
            if (!isCurrentUser) buttons.Add(new MButton("Отправить предметы", "SendItem"));

            _player.win = new Window()
            {
                Title = text,
                Tabs =
                [
                    new Tab()
                    {
                        Action = "HandlerOpenViewChat",
                        Label = text,
                        InitialPage = new Page()
                        {
                            Buttons = buttons.ToArray()
                        }
                    }
                ]
            };
            _player.SendWindow();
        }

        private void Taur(TYPacket f, TAURPacket t) { /* TODO */ }

        private void Chin(TYPacket f, ChinPacket chin) => Console.WriteLine(chin.message);

        private void Pren(TYPacket f, PRENPacket pren) => _player?.Rename(pren.Id);

        private void Prst(TYPacket f, pRSTPacket prst) => _player?.UpdateUIProgramm();

        private void PROG(TYPacket f, PROGPacket p) => _player?.StartedProg(p.prog);

        private void Pope(TYPacket f, PopePacket p) => _player?.OpenGuiProgramm();

        private void Pdel(TYPacket f, PDELPacket pdel) => DataBase.DeleteProg(pdel.Id);

        private void Xhea(TYPacket f, XheaPacket heal) => _player?.Heal();

        private void Clan(TYPacket f, ClanPacket p) => _player?.OpenClan();

        private void Res(TYPacket f, RESPPacket p) => _player?.Death();

        private void Inus(TYPacket f, INUSPacket inus) => _player?.inventory.Use(_player);

        private void Sett(TYPacket f, SettPacket p) => _player?.settings.SendSettingsGUI(_player);

        private void Invn(TYPacket f, INVNPacket invn)
        {
            if (_player == null) return;
            _player.inventory.minv = !_player.inventory.minv;
            _player.SendInventory();
        }

        private void Chat(TYPacket f, ChatPacket chat)
        {
            if (!Default.def.IsMatch(chat.message.Replace("\n", ""))) return;
            _player?.currentchat?.AddMessage(_player, chat.message.Replace("\n", ""));
        }

        private void ADMN(TYPacket f, ADMNPacket p)
        {
            if (_player?.win is null) return;
            _player.win.AdminButton();
            _player.SendWindow();
            _player.win.ShowTabs = true;
        }


        private void Dpbx(TYPacket f, DPBXPacket p)
        {
            _player!.win = _player.crys.OpenBoxGui(_player);
            _player.SendWindow();
        }

        private void HandlePong(PongPacket p)
        {
            _lastPong = ServerTime.Now;
            if (_nextExpected == 0)
                _nextExpected = p.CurrentTime;

            // асинхронная отложенная отправка Ping (без блокировки потоков)
            _ = Task.Run(async () =>
            {
                await Task.Delay(PingDelayMs).ConfigureAwait(false);
                var delta = p.CurrentTime - (_nextExpected - PingOffset);
                SendU(new PingPacket(52, p.CurrentTime + 1, $"{delta} "));
            });

            _nextExpected = p.CurrentTime + PingOffset;
        }

        private void Incl(TYPacket f, INCLPacket incl)
        {
            if (!incl.selection.HasValue || _player is null) return;
            _player.inventory.Choose(incl.selection.Value, _player);
        }

        private void DigHandler(TYPacket parent, XdigPacket packet) => _player?.TryAct(() =>
        {
            _player.Move(_player.x, _player.y, DirectionTypeExt.ToDirection(packet.Direction));
            _player.Bz();
        }, 200);

        private void GeoHandler(TYPacket parent, XgeoPacket packet) => _player?.TryAct(_player.Geo, 200);

        private void BuildHandler(TYPacket parent, XbldPacket packet) => _player?.TryAct(() => _player.Build(packet.BlockType), 200);

        private void AutoDiggHandler(TYPacket parent, TADGPacket packet)
        {
            if (_player == null) return;
            _player.autoDig = !_player.autoDig;
            SendU(new AutoDiggPacket(_player.autoDig));
        }

        private void MoveHandler(TYPacket parent, XmovPacket packet)
            => _player?.TryAct(() => _player.Move((int)parent.X, (int)parent.Y, DirectionTypeExt.ToDirection(packet.Direction)), _player.ServerPause);

        private void WhoisHandler(TYPacket parent, WhoiPacket packet)
            => SendU(new NickListPacket(packet.BotIds.ToDictionary(x => x, DataBase.NickName)));

        private void LocalChatHandler(TYPacket parent, LoclPacket packet)
        {
            if (_player == null || _player.win != null || packet.Length == 0) return;

            var msg = packet.Message;
            if (msg == "console" || (msg.Length > 1 && msg[0] == '>'))
            {
                MConsole.ShowConsole(_player);
                return;
            }

            if (!string.IsNullOrWhiteSpace(msg))
            {
                World.W.SendLocalMsg(_player.id, _player.x, _player.y, msg);
            }
        }

        public void GUI(TYPacket p, GUI_Packet ty)
        {
            var button = ty.Button;
            if (button == null) return;

            if (_auth != null)
            {
                _auth.ProcessAction(button);
                return;
            }

            _serverTime.AddAction(() =>
            {
                // exit handlers
                var s = button.ToString();
                if (s == "exit" || s == "exit:0")
                {
                    CloseWindow();
                    _player?.skillslist.InstallSlot(-1);
                    return;
                }

                _player?.CallWinAction(button);
                _player?.SendWindow();
            }, _player);
        }

        #endregion

        #region senders

        public void SendWorldInfo() => SendU(World.WorldMapInfoPacket());

        public void SendWin(string win) => SendU(new GUIPacket(win));

        public void SendU(ITopLevelPacket data) => Send(new("U", data));

        public void SendB(ITopLevelPacket data) => Send(new("B", data));

        public void SendJ(ITopLevelPacket data) => Send(new("J", data));

        public void Send(Packet p)
        {
            if (p == default) return;

            var buffer = ArrayPool<byte>.Shared.Rent(p.Length);
            try
            {
                // Кодирование в выделенный буфер
                p.Encode(buffer.AsSpan(0, p.Length));
                SendAsync(new ReadOnlySpan<byte>(buffer, 0, p.Length));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        private Task SafeSendUAsync(ITopLevelPacket data)
        {
            try
            {
                SendU(data);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send packet: {ex}");
                return Task.CompletedTask;
            }
        }

        public void CloseWindow()
        {
            if (_player != null)
            {
                _player.win = null;
            }
            SendU(new GuPacket());
        }

        public void CreateSession(Player? authPlayer)
        {
            if (authPlayer == null) return;

            _isCompleted = true;
            _auth = null;
            _player = authPlayer;
            _player.connection = this;
            _player.Init();
            SendU(new AHPacket(_player.id, _player.hash));
        }

        #endregion
    }
}
