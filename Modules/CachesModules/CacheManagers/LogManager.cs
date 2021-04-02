using CachesModules.Domains;
using CachesModules.Utilitys;
using Leftyer.Domains.Utilitys;
using System;
namespace CachesModules.Managers
{
    /*
* date:2020/08/22
* author:Leftyer  
* description:Good code doesn't need comments.
*/
    public class LogManager : DBContexts<LogRecord>
    {
        LogManager() : base(ConfigCore.Instance.DBCon) { }
        public static LogManager Instance => new();
        public void Log(string msg) => Log("Info", msg, msg);
        public bool Log(string name, string msg, string detail, string operatorName = "", string remark = "")
        {
            try
            {
                return CurrentDB.Insert(new LogRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    CTime = DateTime.Now,
                    UTime = DateTime.Now,
                    Creator = operatorName,
                    Modifier = operatorName,
                    IsDeleted = (int)Utilitys.EnumsCore.IsDeleted.NotDelete,
                    Remark = remark,
                    LogName = name,
                    LogMsg = msg,
                    LogDetail = detail
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
