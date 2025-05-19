using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Policy;
using MinesServer.Server;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing;
using Azure;
using System.Net.Mime;
using System.Drawing.Imaging;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MinesServer
{
    public class ImgSpace
    {
        public ImgSpace() { Updater(); _serverspace.Start(); }
        static HttpListener _serverspace = new HttpListener();
        public static string UrlB(string urlend) => $"http://localhost/{urlend}/";
        public static string LocateChunks(string urlend,(int x, int y)start,(int x, int y) end)
        {
            var url = UrlB(urlend);
            var array = M3Compressor.CompressLarge((Bitmap)resizeImage(381, 381, Default.ConvertMapPart(start.x * 32, start.y * 32, end.x * 32, end.y * 32)));
            AddHandler(url, array);
            return url;
        }
        public static string LocateImg(string urlend,string url)
        {
            var result = UrlB(urlend);
            var array = M3Compressor.CompressLarge(Load(url));
            AddHandler(result, array);
            return result;
        }
        private static Bitmap Load(string url)
        {
            var client = new HttpClient();
            var x = client.GetStreamAsync(url).Result;
            return  (Bitmap)resizeImage(200,200,Image.FromStream(x));
        }
        public static Image resizeImage(int newWidth, int newHeight, Image imgPhoto)
        {

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            //Consider vertical pics
            if (sourceWidth < sourceHeight)
            {
                int buff = newWidth;

                newWidth = newHeight;
                newHeight = buff;
            }

            int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
            float nPercent = 0, nPercentW = 0, nPercentH = 0;

            nPercentW = ((float)newWidth / (float)sourceWidth);
            nPercentH = ((float)newHeight / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((newWidth -
                          (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((newHeight -
                          (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);


            Bitmap bmPhoto = new Bitmap(newWidth, newHeight,
                          PixelFormat.Format24bppRgb);

            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                         imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Black);
            grPhoto.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            imgPhoto.Dispose();
            return bmPhoto;
        }
        private void Updater()
        {
            Task.Run(() =>
            {
                while(true)
                {
                    if (handlers.Count > 0)
                        _serverspace.GetContextAsync().ContinueWith(i =>
                        {
                            var context = i.Result; var url = context.Request.Url.ToString();
                            Task.Run(() =>
                            {
                                //Thread.Sleep(200);
                                Emit(url, context);
                            });
                        });
                    Thread.Sleep(1);
                }
            });
        }
        private static void Emit(string url,HttpListenerContext context)
        {
            lock (dlock)
            {
                if (!handlers.ContainsKey(url) || !_serverspace.Prefixes.Contains(url)) return;
                var array = handlers[url];
                context.Response.KeepAlive = true;
                context.Response.ContentLength64 = array.Length;
                context.Response.OutputStream.WriteAsync(array, 0, array.Length).ContinueWith((a) => { context?.Response.Close(); _serverspace.Prefixes.Remove(url); handlers.Remove(url); });
            }
        }
        private static void AddHandler(string url,byte[] action)
        {
            lock(dlock)
            {
                handlers[url] = action;
                if (!_serverspace.Prefixes.Contains(url)) _serverspace.Prefixes.Add(url);
            }
        }
        readonly static object dlock = new();
        static Dictionary<string, byte[]> handlers = new();
        private static byte[] GetArrayFromChunks((int x,int y)start,(int x,int y)end) => M3Compressor.Compress(Default.ConvertMapPart(start.x * 32, start.y * 32, end.x * 32, end.y * 32));
    }
}
