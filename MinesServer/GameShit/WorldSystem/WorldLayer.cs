using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32.SafeHandles;
using MinesServer.Utils;
using Newtonsoft.Json.Linq;
using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.WorldSystem
{
    /// <summary>
    /// WorldLayer by Darkar25(i changed some parts but he showed way to better implementation)
    /// </summary>
    public class WorldLayer<T>(string filename, (int width, int height) chunks, int chunksize = 32) where T : unmanaged
    {
        private readonly int typesize = Marshal.SizeOf<T>();
        private readonly int Count = (int)Math.Pow(chunksize, 2);
        private BinaryStream _stream = new BinaryStream(new FileStream(filename, FileMode.OpenOrCreate));
        private readonly int amount = chunks.width * chunks.height;
        private T[]?[] _buffer = new T[chunks.width * chunks.height][];
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        protected readonly ConcurrentHashSet<(int chunkx, int chunky)> _updatedChunks = new();
        Mutex mut = new();
        /// <summary>
        /// <see cref='this[int, int]'/> writes and reades Cells by original pos in world
        /// </summary>
        public T? this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= chunks.width * chunksize || y < 0 || y >= chunks.height * chunksize) return null;
                var pos = GetChunkPos(x, y);
                T[] temp = null;
                do temp = Read(pos.x, pos.y);
                while (temp is null);
                return temp[GetCellIndex(x, y)];
            }
            set
            {
                if (x < 0 || x >= chunks.width * chunksize || y < 0 || y >= chunks.height * chunksize) return;
                var pos = GetChunkPos(x, y);
                var buffer = Read(pos.x, pos.y);
                buffer[GetCellIndex(x, y)] = value!.Value;
                _updatedChunks.Add(pos);
            }
        }
        /// <summary>
        /// <see cref='Unload'/> Unloads chunks from memory by chunk pos
        /// </summary>
        public void Unload(int chunkx, int chunky) => Unload(GetChunkIndex(chunkx, chunky));
        public void Unload(int chunkIndex)
        {
            if (chunkIndex < 0 || chunkIndex >= amount) return;
            _buffer[chunkIndex] = null;
        }
        /// <summary>
        /// <see cref='Commit'/> Commits World into file
        /// </summary>
        public void Commit()
        {
            foreach (var pos in _updatedChunks)
            { 
                var chunkindex = GetChunkIndex(pos.chunkx, pos.chunky);
                if (_buffer[chunkindex] is not null)
                {
                    Write(pos.chunkx, pos.chunky, _buffer[chunkindex]!);
                }
            }
            _updatedChunks.Clear();
        }
        public void Delete()
        {
            _stream.Close();
            File.Delete(filename);
        }
        private T[] Read(int index)
        {
            mut.WaitOne();
            _lock.EnterReadLock();
            try
            {
                var chunk = new T[Count];
                Span<byte> temp = stackalloc byte[Count * typesize];
                _stream.Position = index * Count * typesize;
                _stream.Read(temp);
                for (int i = 0, j = 0; i < temp.Length; i += typesize, j++)
                    chunk[j] = MemoryMarshal.Read<T>(temp[i..(i + typesize)]);
                return chunk;
            }
            finally { if (_lock.IsReadLockHeld) _lock.ExitReadLock();mut.ReleaseMutex(); }
        }
        private void Write(int index, T[] data)
        {
            mut.WaitOne();
            _lock.EnterReadLock();
            try
            {
                Span<byte> temp = stackalloc byte[data.Length * typesize];
                for (int i = 0, j = 0; i < temp.Length; i += typesize, j++)
                    MemoryMarshal.Write(temp[i..(i + typesize)], in data[j]);
                _stream.Position = index * Count * typesize;
                _stream.Write(temp);
            }
            finally { if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();mut.ReleaseMutex(); }
        }
        public T[] Read(int chunkx, int chunky)
        {
            var chunkindex = GetChunkIndex(chunkx, chunky);
            while (_buffer[chunkindex] is null)
                _buffer[chunkindex] ??= Read(chunkindex);
            return _buffer[chunkindex];
        }
        public void Write(int chunkx, int chunky, T[] data) => Write(GetChunkIndex(chunkx, chunky), data);
        private (int x, int y) GetChunkPos(int x, int y) => ((int)Math.Floor((float)x / 32), (int)Math.Floor((float)y / 32));
        private int GetChunkIndex(int chunkx, int chunky) => chunky + chunks.height * chunkx;
        private int GetCellIndex(int x, int y)
        {
            var chpos = GetChunkPos(x, y);
            return y - chpos.y * 32 + chunksize * (x - chpos.x * 32);
        }
    }
}
