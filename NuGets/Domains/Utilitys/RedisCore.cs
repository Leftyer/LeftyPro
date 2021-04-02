using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace Leftyer.Domains.Utilitys
{
    /*
* date:2020/08/22
* author:Leftyer  
* description:Good code doesn't need comments.
*/
    public class RedisCore
    {
        RedisCore(string redisDBDefault)
        {

            this.DefaultTimeout = 600;
            Redis = ConnectionMultiplexer.Connect(redisDBDefault);
            Db = Redis.GetDatabase();
        }
        public static RedisCore Instance(string redisDBDefault) => new(redisDBDefault);
        int DefaultTimeout { get; set; }
        ConnectionMultiplexer Redis { get; set; }
        IDatabase Db { get; set; }
        JsonSerializerSettings jsonConfig => new() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore };

        class CacheObject<T>
        {
            public int ExpireTime { get; set; }
            public bool ForceOutofDate { get; set; }
            public T Value { get; set; }
        }
        public int TimeOut
        {
            get
            {
                return DefaultTimeout;
            }
            set
            {
                DefaultTimeout = value;
            }
        }

        public object Get(string key) => Get<object>(key);

        public T Get<T>(string key)
        {
            var cacheValue = Db.StringGet(key);
            var value = default(T);
            if (!cacheValue.IsNull)
            {
                var cacheObject = JsonConvert.DeserializeObject<CacheObject<T>>(cacheValue, jsonConfig);
                if (!cacheObject.ForceOutofDate)
                    Db.KeyExpire(key, new TimeSpan(0, 0, cacheObject.ExpireTime));
                value = cacheObject.Value;
            }
            return value;

        }

        public void Set(string key, object data) => Db.StringSet(key, GetJsonData(data, TimeOut, false));

        public void Set(string key, object data, int cacheTime) => Db.StringSet(key, GetJsonData(data, cacheTime, true), TimeSpan.FromSeconds(cacheTime));

        public void Set(string key, object data, DateTime cacheTime) => Db.StringSet(key, GetJsonData(data, TimeOut, true), cacheTime - DateTime.Now);

        public void Set<T>(string key, T data) => Db.StringSet(key, GetJsonData<T>(data, TimeOut, false));

        public void Set<T>(string key, T data, int cacheTime) => Db.StringSet(key, GetJsonData<T>(data, cacheTime, true), TimeSpan.FromSeconds(cacheTime));

        public void Set<T>(string key, T data, DateTime cacheTime) => Db.StringSet(key, GetJsonData<T>(data, TimeOut, true), cacheTime - DateTime.Now);

        string GetJsonData(object data, int cacheTime, bool forceOutOfDate) => JsonConvert.SerializeObject(new CacheObject<object>() { Value = data, ExpireTime = cacheTime, ForceOutofDate = forceOutOfDate }, jsonConfig);

        string GetJsonData<T>(T data, int cacheTime, bool forceOutOfDate) => JsonConvert.SerializeObject(new CacheObject<T>() { Value = data, ExpireTime = cacheTime, ForceOutofDate = forceOutOfDate }, jsonConfig);

        public void Remove(string key) => Db.KeyDelete(key);
        public bool Exists(string key) => Db.KeyExists(key);
    }

}
