namespace LivesModules.Utilitys
{
    /*
* date:2020/08/22
* author:Leftyer  
* description:Good code doesn't need comments.
*/
    public class ConfigCore
    {
        ConfigCore() { }
        public static ConfigCore Instance => new();
        string Root => "192.168.159.133";
        public string RedisDBDefault => $"{Root}:6379,password=zhangxueliangvip,connectTimeout=1000,connectRetry=1,syncTimeout=1000";
        public string DBCon => $@"Data Source={Root};Initial Catalog=DBSystems;User ID=sa;PassWord=sqlserverDB2019";
    }
}
