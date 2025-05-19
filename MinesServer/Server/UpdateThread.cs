using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Text;
using System.Linq;
using System.Net.Quic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.Server
{
    //fix this and think about threads
    public class UpdateThread<T> : Dictionary<T, Action> where T : notnull
    {
        public async Task processAll() => await Task.Run(() => { lock (qlock) while (Count > 0) Dequeue().body();});
        public void Enqueue(T key,Action body)
        {
            lock (qlock)
            {
                if (!ContainsKey(key))
                    Add(key, body);
            }
        }
        private (T key, Action body) Dequeue()
        {
            var key = Keys.First();
            var result = (key, this[key]);
            Remove(key);
            return result;
        }
        readonly object qlock = new();
    }
}
