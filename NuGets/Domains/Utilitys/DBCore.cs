using SqlSugar;

namespace Leftyer.Domains.Utilitys
{
    /*
* date:2020/08/22
* author:Leftyer  
* description:Good code doesn't need comments.
*/
    public class DBCore
    {
        public DBCore(string connection)
        {
            Connection = connection;
        }
        public string Connection { get; set; }
        public SqlSugarClient Db => new(new ConnectionConfig()
        {
            ConnectionString = Connection,
            DbType = DbType.SqlServer,
            IsAutoCloseConnection = true
        });
    }
}
