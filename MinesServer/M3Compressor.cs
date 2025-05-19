using Azure;
using Microsoft.EntityFrameworkCore.Query.Internal;
using MinesServer.GameShit.WorldSystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MinesServer
{
    public static class M3Compressor
    {
        public static byte[] Compress(Bitmap image)
        {
            Span<byte> temp = stackalloc byte[2 * 2 + 10 + (image.Width * image.Height) * 4];
            var width = Convert.ToUInt16(image.Width);
            var height = Convert.ToUInt16(image.Height);
            MemoryMarshal.Write(temp[0..2], in width);
            MemoryMarshal.Write(temp[2..4], in height);
            byte op = 4;
            for (int i = 0;i<10;i++)
            {
                
                MemoryMarshal.Write(temp[(4+i)..], in op);
                op = 0;
            }
            for(int y = 0;y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var p = image.GetPixel(x, y);
                    byte r = p.R, g = p.G, b = p.B, a = p.A;
                    var t = (14 + (4 * (y * image.Width + x)));
                    MemoryMarshal.Write(temp[t..], in r);
                    t++;
                    MemoryMarshal.Write(temp[t..], in g);
                    t++;
                    MemoryMarshal.Write(temp[t..], in b);
                    t++;
                    MemoryMarshal.Write(temp[t..], in a);
                }
            }
            return temp.ToArray();
        }
        public static byte[] CompressLarge(Bitmap image)
        {
            var temp = new byte[2 * 2 + 10 + (image.Width * image.Height) * 4];
            Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToUInt16(image.Width)), 0, temp, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToUInt16(image.Height)), 0, temp, 2, 2);
            byte op = 4;
            for (int i = 0; i < 10; i++)
            {
                temp[(4 + i)] = op;
                op = 0;
            }
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var p = image.GetPixel(x, y);
                    byte r = p.R, g = p.G, b = p.B, a = p.A;
                    var t = (14 + (4 * (y * image.Width + x)));
                    temp[t] = r; temp[t + 1] = g; temp[t + 2] = g; temp[t + 3] = a;
                }
            }
            return temp.ToArray();
        }
        public static byte[] CompressImageArray((byte r,byte g,byte b,byte a)[] args,int iwidth,int iheight)
        {
            Span<byte> temp = stackalloc byte[args.Length * 4];
            for (int y = 0; y < iheight; y++)
            {
                for (int x = 0; x < iwidth; x++)
                {
                    byte r = 0, g = 0, b = 0, a = 0;
                    var t = ((4 * (y * iwidth + x)));
                    MemoryMarshal.Write(temp[t..], in r);
                    t++;
                    MemoryMarshal.Write(temp[t..], in g);
                    t++;
                    MemoryMarshal.Write(temp[t..], in b);
                    t++;
                    MemoryMarshal.Write(temp[t..], in a);
                }
            }
            return temp.ToArray();
        }
        public static Bitmap ConvertMapPart(int fromx, int fromy, int tox, int toy)
        {
            var bitmap = new Bitmap(tox - fromx, toy - fromy);
            for (int x = 0; fromx + x < tox; x++)
            {
                for (int y = 0; fromy + y < toy; y++)
                {
                    bitmap.SetPixel(x, y, World.GetProp(fromx + x, fromy + y).isEmpty ? Color.Green : Color.CornflowerBlue);
                }
            }
            return bitmap;
        }
    }
}
