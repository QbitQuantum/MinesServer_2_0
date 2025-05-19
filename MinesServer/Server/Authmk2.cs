using Azure;
using Microsoft.Identity.Client;
using MoreLinq;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.Server
{
    public class HResponse(string ContentType,byte[] buffer,int status = 200)
    {
        public byte[] encode => Encoding.Default.GetBytes($"HTTP/1.1 {status} OK\nContent-Type: {ContentType}\nContent-Length: {buffer.Length}\n\n").Concat(buffer).ToArray();
    }
    public class HRequest
    {
        public HRequest(byte[] buffer)
        {
            body = Encoding.Default.GetString(buffer);
            content = body.Split('\n');
            var first = content[0].Split(' ');
            type = first[0];sub = first[1];
            host = content[0].Split(' ')[1];
            ParseArgs(host);
        }
        void ParseArgs(string u)
        {
            if (u.Contains('?')) u = u[(u.IndexOf('?') + 1)..];
            var lines = u.Split('&');
            foreach (var i in lines)
            {
                var entry = i.Split('=');
                if (entry.Length > 1)
                    args[entry[0]] = entry[1];
            }
        }
        public string this[string key] => args[key];
        Dictionary<string, string> args = new();
        public override string ToString() => body;
        string body;
        string host;
        string sub;
        public string type { get; private set; }
        public string uri => $"{host}{sub}";

        string[] content;
    }
    public static class Authmk2
    {
        static TcpListener tcplis = new(IPAddress.Any,8050);
        public static void Start()
        {
            tcplis.Start();
            Updater();
        }
        static void Updater()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    tcplis.AcceptTcpClientAsync().ContinueWith(async i =>
                    {
                        var client = i.Result;
                        Console.WriteLine(client.Client.LocalEndPoint);
                        byte[] buffer = new byte[client.Client.ReceiveBufferSize];
                        var count = await client.GetStream().ReadAsync(buffer);
                        var request = new HRequest(buffer[..count]);
                        Console.WriteLine(request["sid"]);

                    });
                    Thread.Sleep(1);
                }
            });
        }
        static string mem => @"<!DOCTYPE html>
    <html>
        <head>
            <meta charset='utf8'>
            <title>adin</title>
        </head>
        <body>
            <h2>
                <input></input><button></button>
            </h2>
        </body>
    </html>";
    }
}
