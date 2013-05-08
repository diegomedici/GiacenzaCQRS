﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace GiacenzaCQRS.Core.Storage
{
    public sealed class MemoryDocumentReaderWriter<Guid, TEntity> : IDocumentReader<Guid, TEntity>,
                                                                    IDocumentWriter<Guid, TEntity>
    {
        private readonly IDocumentStrategy _strategy;
        private readonly ConcurrentDictionary<string, byte[]> _store;

        public MemoryDocumentReaderWriter(IDocumentStrategy strategy, ConcurrentDictionary<string, byte[]> store)
        {
            _store = store;
            _strategy = strategy;
        }

        string GetName(Guid key)
        {
            return _strategy.GetEntityLocation<TEntity>(key);
        }

        public bool TryGet(Guid key, out TEntity entity)
        {
            var name = GetName(key);
            byte[] bytes;
            if (_store.TryGetValue(name, out bytes))
            {
                using (var mem = new MemoryStream(bytes))
                {
                    entity = _strategy.Deserialize<TEntity>(mem);
                    return true;
                }
            }
            entity = default(TEntity);
            return false;
        }


        public TEntity AddOrUpdate(Guid key, Func<TEntity> addFactory, Func<TEntity, TEntity> update)
        {
            var result = default(TEntity);
            _store.AddOrUpdate(GetName(key), s =>
                {
                    result = addFactory();
                    using (var memory = new MemoryStream())
                    {
                        _strategy.Serialize(result, memory);
                        return memory.ToArray();
                    }
                }, (s2, bytes) =>
                    {
                        TEntity entity;
                        using (var memory = new MemoryStream(bytes))
                        {
                            entity = _strategy.Deserialize<TEntity>(memory);
                        }
                        result = update(entity);
                        using (var memory = new MemoryStream())
                        {
                            _strategy.Serialize(result, memory);
                            return memory.ToArray();
                        }
                    });
            return result;
        }


        public bool TryDelete(Guid key)
        {
            byte[] bytes;
            return _store.TryRemove(GetName(key), out bytes);
        }
    }
}