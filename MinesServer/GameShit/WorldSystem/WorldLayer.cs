using System.Runtime.InteropServices;

namespace MinesServer.GameShit.WorldSystem
{
    /// <summary>
    /// WorldLayer by Darkar25 (i changed some parts but he showed way to better implementation).
    ///
    /// Optimized:
    ///  - no Mutex; correct ReaderWriterLockSlim usage;
    ///  - zero-copy MemoryMarshal.AsBytes + Stream.ReadExactly/Write (no stackalloc, no per-element loop);
    ///  - LRU eviction with dirty-chunk protection;
    ///  - no busy-wait, no race on cache fill;
    ///  - dirty tracking via HashSet&lt;int&gt; under the same lock (no ConcurrentHashSet).
    /// </summary>
    public sealed class WorldLayer<T> : IDisposable where T : unmanaged
    {
        private readonly int _typeSize;
        private readonly int _chunkSize;
        private readonly int _count;            // cells per chunk
        private readonly int _amount;           // total chunks
        private readonly int _maxChunksInMemory;
        private readonly string _filename;
        private readonly (int width, int height) _chunks;

        private readonly FileStream _stream;
        private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.NoRecursion);

        // LRU: list + index->node + index->array.
        private readonly Dictionary<int, T[]> _loadedChunks;
        private readonly Dictionary<int, LinkedListNode<int>> _lruIndexMap;
        private readonly LinkedList<int> _lruList;

        // Dirty chunk indices. Guarded by _lock (write mode).
        private readonly HashSet<int> _dirtyChunks;

        private volatile bool _disposed;

        /// <summary>
        /// <see cref='this[int, int]'/> reads/writes cells by world position.
        /// </summary>
        public T? this[int x, int y]
        {
            get
            {
                if (!IsInsideWorld(x, y))
                {
                    return null;
                }

                var pos = Chunk.GetChunkPosByCoords(x, y);
                T[] chunk = Read(pos.x, pos.y);
                return chunk[GetCellIndex(x, y)];
            }
            set
            {
                if (!IsInsideWorld(x, y))
                {
                    return;
                }

                var pos = Chunk.GetChunkPosByCoords(x, y);
                int chunkIndex = GetChunkIndex(pos.x, pos.y);

                // Read materializes the chunk and takes the necessary lock.
                T[] chunk = Read(pos.x, pos.y);
                int cellIndex = GetCellIndex(x, y);

                if (EqualityComparer<T>.Default.Equals(chunk[cellIndex], value!.Value))
                {
                    return;
                }

                _lock.EnterWriteLock();
                try
                {
                    chunk[cellIndex] = value.Value;
                    _dirtyChunks.Add(chunkIndex);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        public WorldLayer(
            string filename,
            (int width, int height) chunks,
            int chunksize = 32,
            int maxChunksInMemory = 1000)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("World layer file path is required.", nameof(filename));
            }

            if (chunks.width <= 0 || chunks.height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(chunks), "World layer dimensions must be positive.");
            }

            if (chunksize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(chunksize), "Chunk size must be positive.");
            }

            if (maxChunksInMemory <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxChunksInMemory), "RAM cache size must be positive.");
            }

            _filename = filename;
            _chunks = chunks;
            _chunkSize = chunksize;
            _count = chunksize * chunksize;
            _amount = chunks.width * chunks.height;
            _maxChunksInMemory = maxChunksInMemory;
            _typeSize = Marshal.SizeOf<T>();

            _loadedChunks = new Dictionary<int, T[]>(maxChunksInMemory);
            _lruIndexMap = new Dictionary<int, LinkedListNode<int>>(maxChunksInMemory);
            _lruList = new LinkedList<int>();
            _dirtyChunks = new HashSet<int>();

            _stream = new FileStream(
                filename,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.None,
                bufferSize: 4096,
                FileOptions.RandomAccess);
        }

        public int ChunkSize => _chunkSize;
        public int WidthChunks => _chunks.width;
        public int HeightChunks => _chunks.height;
        public int MaxChunksInMemory => _maxChunksInMemory;

        public int LoadedCount
        {
            get
            {
                _lock.EnterReadLock();
                try { return _loadedChunks.Count; }
                finally { _lock.ExitReadLock(); }
            }
        }

        public int DirtyCount
        {
            get
            {
                _lock.EnterReadLock();
                try { return _dirtyChunks.Count; }
                finally { _lock.ExitReadLock(); }
            }
        }

        /// <summary>Unload a chunk from memory by chunk coords.</summary>
        public void Unload(int chunkx, int chunky) => Unload(GetChunkIndex(chunkx, chunky));

        public void Unload(int chunkIndex)
        {
            if (chunkIndex < 0 || chunkIndex >= _amount)
            {
                return;
            }

            _lock.EnterWriteLock();
            try
            {
                // Don't drop a dirty chunk: its changes would be lost before Commit.
                if (_dirtyChunks.Contains(chunkIndex))
                {
                    return;
                }

                RemoveFromCache(chunkIndex);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>Writes all dirty chunks to disk and flushes the stream.</summary>
        public void Commit()
        {
            ThrowIfDisposed();

            _lock.EnterWriteLock();
            try
            {
                foreach (int chunkIndex in _dirtyChunks)
                {
                    if (_loadedChunks.TryGetValue(chunkIndex, out T[]? chunk) && chunk != null)
                    {
                        WriteChunkInternal(chunkIndex, chunk);
                    }
                }

                _dirtyChunks.Clear();
                _stream.Flush();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Delete()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            _lock.EnterWriteLock();
            try
            {
                _loadedChunks.Clear();
                _lruIndexMap.Clear();
                _lruList.Clear();
                _dirtyChunks.Clear();
                _stream.Dispose();
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            if (File.Exists(_filename))
            {
                File.Delete(_filename);
            }
        }

        // --- I/O primitives ---

        private T[] Read(int index)
        {
            var chunk = new T[_count];
            var bytes = MemoryMarshal.AsBytes(chunk.AsSpan());

            long required = (long)(index + 1) * _count * _typeSize;
            if (_stream.Length < required)
                return chunk;
            _stream.Position = (long)index * _count * _typeSize;
            _stream.ReadExactly(bytes);
            return chunk;
        }

        private void WriteChunkInternal(int index, T[] data)
        {
            if (data.Length != _count)
            {
                throw new ArgumentException(
                    $"Chunk data length {data.Length} does not match chunk area {_count}.",
                    nameof(data));
            }

            var bytes = MemoryMarshal.AsBytes(data.AsSpan());
            _stream.Position = (long)index * _count * _typeSize;
            _stream.Write(bytes);
        }

        // --- public chunk API (kept from original) ---

        /// <summary>Reads a chunk by chunk coords, materializing it into the LRU cache.</summary>
        public T[] Read(int chunkx, int chunky)
        {
            ThrowIfDisposed();

            int chunkIndex = GetChunkIndex(chunkx, chunky);
            if (chunkIndex < 0 || chunkIndex >= _amount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(chunkx),
                    "Chunk coordinates are outside the world layer.");
            }

            _lock.EnterUpgradeableReadLock();
            try
            {
                if (_loadedChunks.TryGetValue(chunkIndex, out T[]? cached) && cached != null)
                {
                    TouchLRU(chunkIndex);
                    return cached;
                }

                _lock.EnterWriteLock();
                try
                {
                    // Double-check: another thread may have loaded it while we waited.
                    if (_loadedChunks.TryGetValue(chunkIndex, out cached) && cached != null)
                    {
                        TouchLRU(chunkIndex);
                        return cached;
                    }

                    T[] chunk = Read(chunkIndex);
                    AddToCache(chunkIndex, chunk);
                    return chunk;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }

        public void Write(int chunkx, int chunky, T[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            ThrowIfDisposed();

            int chunkIndex = GetChunkIndex(chunkx, chunky);
            if (chunkIndex < 0 || chunkIndex >= _amount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(chunkx),
                    "Chunk coordinates are outside the world layer.");
            }

            _lock.EnterWriteLock();
            try
            {
                AddToCache(chunkIndex, data);
                _dirtyChunks.Add(chunkIndex);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        // --- LRU internals ---

        private void AddToCache(int chunkIndex, T[] chunk)
        {
            if (_loadedChunks.ContainsKey(chunkIndex))
            {
                _loadedChunks[chunkIndex] = chunk;
                TouchLRU(chunkIndex);
                return;
            }

            EvictIfNeeded();

            _loadedChunks[chunkIndex] = chunk;
            var node = _lruList.AddFirst(chunkIndex);
            _lruIndexMap[chunkIndex] = node;
        }

        private void TouchLRU(int chunkIndex)
        {
            if (_lruIndexMap.TryGetValue(chunkIndex, out var node))
            {
                _lruList.Remove(node);
                _lruList.AddFirst(node);
            }
        }

        private void RemoveFromCache(int chunkIndex)
        {
            if (_lruIndexMap.TryGetValue(chunkIndex, out var node))
            {
                _lruList.Remove(node);
                _lruIndexMap.Remove(chunkIndex);
            }

            _loadedChunks.Remove(chunkIndex);
        }

        private void EvictIfNeeded()
        {
            while (_loadedChunks.Count >= _maxChunksInMemory)
            {
                int? candidate = FindEvictionCandidate();
                if (candidate == null)
                {
                    // Everything is dirty; the cache is allowed to grow above
                    // capacity until Commit.
                    return;
                }

                RemoveFromCache(candidate.Value);
            }
        }

        /// <summary>
        /// Walks the LRU from the tail and returns the first non-dirty index.
        /// Dirty chunks must not be evicted: their changes aren't on disk yet.
        /// </summary>
        private int? FindEvictionCandidate()
        {
            LinkedListNode<int>? node = _lruList.Last;
            while (node != null)
            {
                if (!_dirtyChunks.Contains(node.Value))
                {
                    return node.Value;
                }

                node = node.Previous;
            }

            return null;
        }

        private int GetChunkIndex(int chunkx, int chunky) => chunky + _chunks.height * chunkx;

        private int GetCellIndex(int x, int y)
        {
            int chunkX = x / _chunkSize;
            int chunkY = y / _chunkSize;
            int localX = x - chunkX * _chunkSize;
            int localY = y - chunkY * _chunkSize;
            return localY + localX * _chunkSize;
        }

        private bool IsInsideWorld(int x, int y)
        {
            return x >= 0 && y >= 0 &&
                   x < _chunks.width * _chunkSize &&
                   y < _chunks.height * _chunkSize;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(WorldLayer<T>));
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                Commit();
            }
            catch (IOException)
            {
                // Persist failure during dispose; the stream close below still runs.
            }

            _disposed = true;

            _lock.EnterWriteLock();
            try
            {
                _loadedChunks.Clear();
                _lruIndexMap.Clear();
                _lruList.Clear();
                _dirtyChunks.Clear();
                _stream.Dispose();
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            _lock.Dispose();
        }
    }
}