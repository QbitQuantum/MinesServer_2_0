using Microsoft.EntityFrameworkCore;
using MinesServer.GameShit;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Programmator;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network;
using MinesServer.Network.Auth;
using MinesServer.Network.ConnectionStatus;
using MinesServer.Network.Constraints;
using MinesServer.Network.GUI;
using MinesServer.Network.HubEvents;
using MinesServer.Network.Programmator;
using MinesServer.Network.TypicalEvents;
using MinesServer.Network.World;
using MinesServer.Server.Network.TypicalEvents;
using NetCoreServer;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MinesServer.Server
{
    public class Session : TcpSession
    {
        #region fields

        MServer father;
        public Player? player;
        public Auth auth;
        public Session(TcpServer server) : base(server) { father = server as MServer; }
        public string sid { get; set; }
        #endregion


        #region server handlers
        protected override void OnConnected()
        {
            sid = Auth.GenerateSessionId();
            Console.WriteLine($"Connected__\npoint:{Socket.RemoteEndPoint}");
            SendU(new StatusPacket("черный хуй в твоей жопе"));
            SendU(new AUPacket(sid));
            SendU(new PingPacket(0, 0, ""));
        }
        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            if (Packet.TryDecode(buffer, out var result))
            {
                try
                {
                    switch (result.data)
                    {
                        case AUPacket au: AU(au); break;
                        case TYPacket ty: father.time.AddAction(() => TY(ty), player); break;
                        case PongPacket ping: Ping(ping); break;
                        default:
                            // Invalid packet
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"invalid packet from {player?.id} {ex}");
                }
            }
        }
        protected override void OnDisconnected()
        {
            if (player is null) return;
            Console.WriteLine($"Disconnected__\nid:{player.id}\nname:{player.name}");
            player.dOnDisconnect();
            player = null;
            Dispose();
        }
        #endregion



        #region handlers

        private void AU(AUPacket p)
        {
            auth = new Auth(this);
            auth.TryToAuth(p, sid);
        }
        private void TY(TYPacket packet)
        {
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
                case PDELPacket pdel: Pdel(packet, pdel);break;
                case pRSTPacket prst: Prst(packet, prst); break;
                case PRENPacket pren: Pren(packet, pren); break;
                case ChatPacket chat: Chat(packet, chat);break;
                case INVNPacket invn: Invn(packet, invn); break;
                case XheaPacket xhea: Xhea(packet, xhea); break;
                case ChinPacket chin: Chin(packet, chin);break;
                case BldsPacket blds: Blds(packet, blds);break;
                case TAGRPacket agr:Agr(packet, agr);break;

                    /////FIX THIS SH
                case TAURPacket taur: Taur(packet, taur);break;
                default:
                    // Invalid event type
                    break;
            }
        }
        private void Blds(TYPacket f, BldsPacket blds) => Task.Run(() => player?.OpenMyBuildings());
        private void Agr(TYPacket f,TAGRPacket agr)
        {
            //changeAgr
        }
        private void Taur(TYPacket f,TAURPacket t)
        {

        }
        private void Chin(TYPacket f,ChinPacket chin)
        {
           
        }
        private void Invn(TYPacket f,INVNPacket invn)
        {
            player.inventory.minv = !player.inventory.minv;
            player.SendInventory();
        }
        private void Chat(TYPacket f,ChatPacket chat)
        {
            if (Default.def.IsMatch(chat.message.Replace("\n", "")))
            {
                player?.currentchat?.AddMessage(player, chat.message.Replace("\n", ""));
            }
        }
        private void Pren(TYPacket f,PRENPacket pren)
        {
            StaticGUI.Rename(player, pren.Id);
        }
        private void Prst(TYPacket f, pRSTPacket prst)
        {
            var p = player.programsData.selected;
            if (p != null && !player.programsData.ProgRunning)
                player.OpenProg(p);
            if (player.programsData.ProgRunning)
                player.RunProgramm();
            player.ProgStatus();
        }
        private void Pdel(TYPacket f,PDELPacket pdel)
        {
            StaticGUI.DeleteProg(player, pdel.Id);
        }
        private void PROG(TYPacket f, PROGPacket p)
        {
            StaticGUI.StartedProg(player, p.prog);
            player?.ProgStatus();
        }
        private void Xhea(TYPacket f,XheaPacket heal) => player?.Heal();
        private void Pope(TYPacket f, PopePacket p)
        {
            StaticGUI.OpenGui(player);
        }
        private void Clan(TYPacket f, ClanPacket p) => player?.OpenClan();
        private void Res(TYPacket f, RESPPacket p) => player?.Death();
        private void ADMN(TYPacket f, ADMNPacket p)
        {
            if (player.win is not null)
            {
                player.win.AdminButton();
                player.SendWindow();
                player.win.ShowTabs = true;
            }
        }
        private void Sett(TYPacket f, SettPacket p) => player?.settings.SendSettingsGUI(player);
        private void Dpbx(TYPacket f, DPBXPacket p)
        {
            player.win = player.crys.OpenBoxGui(player);
            player.SendWindow();
        }
        private DateTime lastpong = ServerTime.Now;
        public void CheckDisconnected()
        {
            if (ServerTime.Now - lastpong > TimeSpan.FromSeconds(30)) Disconnect();
            else if (ServerTime.Now - lastpong > TimeSpan.FromSeconds(10)) Ping(new PongPacket(52, nextexpected));
        }
        private void Ping(PongPacket p)
        {
            lastpong = ServerTime.Now;
            if (nextexpected == 0)
                nextexpected = p.CurrentTime;
            Task.Run(() =>
            { 
                Thread.Sleep(200);
                SendU(new PingPacket(52, p.CurrentTime + 1, $"{p.CurrentTime - (nextexpected - 201)} "));
            });
            nextexpected = p.CurrentTime + 201;
        }
        private int nextexpected;
        private void Inus(TYPacket f, INUSPacket inus) => player?.inventory.Use(player);
        private void Incl(TYPacket f, INCLPacket incl)
        {
            if (incl.selection.HasValue)
            {
                if (incl.selection == -1)
                {
                    player.inventory.Choose(-1, player);
                }
                else
                {
                    player.inventory.Choose(incl.selection.Value, player);
                    player.SendInventory();
                }
            }
        }
        private void DigHandler(TYPacket parent, XdigPacket packet) => player?.TryAct(() =>{
                    player.Move(player.x, player.y, packet.Direction);
                    player.Bz();
            }, 200);
        
        private void GeoHandler(TYPacket parent, XgeoPacket packet) => player?.TryAct(player.Geo , 200);
        private void BuildHandler(TYPacket parent, XbldPacket packet) => player?.TryAct(() => player.Build(packet.BlockType), 200);
        
        private void AutoDiggHandler(TYPacket parent, TADGPacket packet) => SendU(new AutoDiggPacket(player.autoDig = !player.autoDig));
        private void MoveHandler(TYPacket parent, XmovPacket packet) => player?.TryAct(() => player.Move((int)parent.X, (int)parent.Y, packet.Direction), player.ServerPause);
        
        private void WhoisHandler(TYPacket parent, WhoiPacket packet) => SendU(new NickListPacket(packet.BotIds.ToDictionary(x => x, x => DataBase.GetPlayer(x)?.name)));
        private void LocalChatHandler(TYPacket parent, LoclPacket packet)
        {
            if (player != null && player.win == null && packet.Length > 0)
            {
                if (packet.Message == "console")
                {
                    MConsole.ShowConsole(player);
                }
                else if (packet.Message[0] == '>' && packet.Message.Length > 1)
                {
                    MConsole.ShowConsole(player);
                }
                else if (!string.IsNullOrWhiteSpace(packet.Message))
                {
                    this.player.SendLocalMsg(packet.Message);
                }
            }

        }
        public void GUI(TYPacket p, GUI_Packet ty)
        {
            var button = ty.Button;
            if (button == null)
            {
                return;
            }
            if ((auth != null && !auth.complited))
            {
                auth.CallAction(button);
                return;
            }
            father.time.AddAction(() =>
            {
                if (button.ToString() is "exit" or "exit:0")
                {
                    CloseWindow();
                    return;
                }
                player.CallWinAction(button);
                player.SendWindow();
            },player);
        }
        #endregion
        #region senders
        public void SendWorldInfo() => SendU(new WorldInfoPacket(World.W.name, World.CellsWidth, World.CellsHeight, 123, "COCK", "http://pi.door/", "ok"));
        public void SendWin(string win) => SendU(new GUIPacket(win));
        public void SendU(ITopLevelPacket data) => Send(new("U", data));

        public void SendB(ITopLevelPacket data) => Send(new("B", data));

        public void SendJ(ITopLevelPacket data) => Send(new("J", data));
        public void Send(Packet p)
        {
            if (p == default) return;
            Span<byte> span = stackalloc byte[p.Length];
            p.Encode(span);
            //Console.WriteLine(Encoding.UTF8.GetString(span));
            SendAsync(span);
        }

        public void SendCell(int x, int y, byte cell)
        {
            SendB(new HBPacket([new HBMapPacket(x, y, 1, 1, [cell])]));
        }
        public void CloseWindow()
        {
            player.win = null;
            player.skillslist.selectedslot = -1;
            SendU(new GuPacket());
        }
        #endregion
    }
}
