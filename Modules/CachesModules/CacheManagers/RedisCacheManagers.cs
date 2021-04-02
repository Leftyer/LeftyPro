using CachesModules.Utilitys;
using Leftyer.Domains.Utilitys;

namespace CachesModules.Managers
{

    public class RedisCacheManagers
    {
        RedisCacheManagers() { }
        public static RedisCacheManagers Instance => new();
        public object Get(string key) => RedisCore.Instance(ConfigCore.Instance.RedisDBDefault).Get(key);
        public T Get<T>(string key) => RedisCore.Instance(ConfigCore.Instance.RedisDBDefault).Get<T>(key);

        public void Set(string key, object val) => RedisCore.Instance(ConfigCore.Instance.RedisDBDefault).Set(key, val);

        public void Set<T>(string key, T val) => RedisCore.Instance(ConfigCore.Instance.RedisDBDefault).Set<T>(key, val);

        public void Del(string key) => RedisCore.Instance(ConfigCore.Instance.RedisDBDefault).Remove(key);
        public bool IsExists(string key) => RedisCore.Instance(ConfigCore.Instance.RedisDBDefault).Exists(key);

    }
}


