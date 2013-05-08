using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GiacenzaCQRS.Core.Storage
{
    public sealed class FileDocumentReaderWriter<TKey, TEntity> : IDocumentReader<TKey, TEntity>,
                                                             IDocumentWriter<TKey, TEntity>
    {
        readonly IDocumentStrategy _strategy;
        readonly string _folder;
        private readonly static JsonSerializerSettings SerializerSettings
         = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };

        public FileDocumentReaderWriter(string directoryPath, IDocumentStrategy strategy)
        {
            _strategy = strategy;
            _folder = Path.Combine(directoryPath, strategy.GetEntityBucket<TEntity>());
        }

        public void InitIfNeeded()
        {
            Directory.CreateDirectory(_folder);
        }

        public bool TryGet(TKey key, out TEntity view)
        {
            view = default(TEntity);
            try
            {
                var name = GetName(key);

                if (!File.Exists(name))
                    return false;

                string fileString = File.ReadAllText(name);
                view = JsonConvert.DeserializeObject<TEntity>(fileString);
                
                return true;

                //using (var stream = File.Open(name, FileMode.Open, FileAccess.Read, FileShare.Read))
                //{
                //    if (stream.Length == 0)
                //        return false;
                //    stream.re
                //    byte[] buffer = new byte[stream.Length];
                //    stream.Read(buffer, 0, stream.Length);
                //    view = _strategy.Deserialize<TEntity>(stream);
                //    DataContractJsonSerializer.ReadObject
                //    view = JsonConvert.DeserializeObject<TEntity>(Encoding.UTF8.GetString(stream));

                //    return true;
                //}
            }
            catch (FileNotFoundException)
            {
                // if file happened to be deleted between the moment of check and actual read.
                return false;
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }
        }

        string GetName(TKey key)
        {
            return Path.Combine(_folder, _strategy.GetEntityLocation<TEntity>(key));
        }

        public TEntity AddOrUpdate(TKey key, Func<TEntity> addFactory, Func<TEntity, TEntity> update)
        {
            var name = GetName(key);

            try
            {
                // This is fast and allows to have git-style subfolders in atomic strategy
                // to avoid NTFS performance degradation (when there are more than 
                // 10000 files per folder). Kudos to Gabriel Schenker for pointing this out
                var subfolder = Path.GetDirectoryName(name);
                if (subfolder != null && !Directory.Exists(subfolder))
                    Directory.CreateDirectory(subfolder);


                // we are locking this file.
                using (var file = File.Open(name, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    byte[] initial = new byte[0];
                    TEntity result;
                    if (file.Length == 0)
                    {
                        result = addFactory();
                    }
                    else
                    {
                        Byte[] buffer = new byte[1000];
                        file.Read(buffer, 0, 1000);
                        string fileString = Encoding.UTF8.GetString(buffer);

                        var entity = JsonConvert.DeserializeObject<TEntity>(fileString);
                        result = update(entity);
                    }

                    // some serializers have nasty habbit of closing the
                    // underling stream
                    using (var mem = new MemoryStream())
                    {
                        var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result, SerializerSettings));

                        if (!data.SequenceEqual(initial))
                        {
                            file.Seek(0, SeekOrigin.Begin);
                            file.Write(data, 0, data.Length);
                            file.SetLength(data.Length);
                        }
                    }

                    return result;
                }
            }
            catch (DirectoryNotFoundException)
            {
                var s = string.Format(
                    "Container '{0}' does not exist.",
                    _folder);
                throw new InvalidOperationException(s);
            }
        }


        //private static object DeserializeEvent(byte[] metadata, byte[] data)
        //{
        //    var eventClrTypeName = JObject.Parse(Encoding.UTF8.GetString(metadata)).Property(EventClrTypeHeader).Value;
        //    return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), Type.GetType((string)eventClrTypeName));
        //}

        public bool TryDelete(TKey key)
        {
            var name = GetName(key);
            if (File.Exists(name))
            {
                File.Delete(name);
                return true;
            }
            return false;
        }
    }
}